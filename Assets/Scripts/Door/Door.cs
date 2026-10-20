using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A hinged door. When the player is inside the interaction zone and presses the interact key
/// (F by default), the door swings 90 degrees AWAY from the player. Pressing it again closes it.
///
/// Recommended hierarchy:
///   Door_Root        <- empty, just keeps things tidy
///   |-- DoorFrame    <- frame model (never moves)
///   |-- DoorPivot    <- placed exactly at the hinge. THIS script, a kinematic Rigidbody and a
///       |               BoxCollider with "Is Trigger" ticked all live here, so the
///       |               interaction zone swings together with the door.
///       |-- Door     <- door model + its solid (non-trigger) collider
/// </summary>
public class Door : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("The object that rotates around the hinge. Leave empty if this script is on the DoorPivot itself.")]
    [SerializeField] private Transform doorPivot;

    [Header("Behaviour")]
    [SerializeField] private float openAngle = 90f;
    [Tooltip("Seconds the door takes to swing open (and to swing closed).")]
    [SerializeField] private float openDuration = 0.8f;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    public enum LockType { None, Gold, Silver }

    [Header("Locks (optional)")]
    [Tooltip("LOCK 1: None = no lock. Gold / Silver = any key of that type opens it (and is used up).")]
    [SerializeField] private LockType lockType = LockType.None;
    [Tooltip("LOCK 1's model. It is hidden when this lock is opened. Optional.")]
    [SerializeField] private GameObject lockObject;
    [Tooltip("Need more than one key? Add extra locks here, each with its own key type and lock model. Leave empty for a single-lock door.")]
    [SerializeField] private List<ExtraLock> extraLocks = new List<ExtraLock>();
    [SerializeField] private string lockedMessage = "It's locked.";
    [Tooltip("For double gates: the OTHER leaf. Once this door is fully unlocked, that one is unlocked too. Set it on both leaves.")]
    [SerializeField] private Door linkedDoor;

    [System.Serializable]
    public class ExtraLock
    {
        public KeyType keyType = KeyType.Gold;
        public GameObject lockObject;
    }

    // Runtime state of each lock on this door.
    private class LockSlot
    {
        public string key;
        public GameObject model;
        public bool open;
    }
    private readonly List<LockSlot> locks = new List<LockSlot>();

    private bool IsLocked
    {
        get
        {
            foreach (LockSlot slot in locks)
                if (!slot.open) return true;
            return false;
        }
    }

    private bool isOpen;
    private bool isMoving;
    private bool playerInRange;
    private Transform player;
    private Quaternion closedRotation;

    private void Awake()
    {
        // If no pivot was assigned, assume this script is sitting on the pivot itself.
        if (doorPivot == null) doorPivot = transform;

        // Remember the closed pose so Close() returns to exactly this rotation.
        closedRotation = doorPivot.rotation;

        // Build the list of locks. Lock 1 is the Lock Type field (None = skipped), the rest come from
        // Extra Locks. No locks at all = a normal unlocked door.
        if (lockType == LockType.Gold) AddLock(KeyType.Gold, lockObject);
        else if (lockType == LockType.Silver) AddLock(KeyType.Silver, lockObject);
        else if (lockObject != null) lockObject.SetActive(false); // no lock, so don't show a lock model

        foreach (ExtraLock extra in extraLocks)
            AddLock(extra.keyType, extra.lockObject);
    }

    private void AddLock(KeyType type, GameObject model)
    {
        locks.Add(new LockSlot { key = KeyPickup.ItemName(type), model = model, open = false });
        if (model != null) model.SetActive(true);
    }

    // Tries every lock that is still closed against the player's Inventory. For each lock, if the
    // player carries a key of that type, the lock opens, its model disappears and ONE key of that
    // type is removed. Returns true only when ALL locks are open.
    private bool TryUnlock(Transform user)
    {
        Inventory inventory = user != null ? user.GetComponentInParent<Inventory>() : null;

        List<string> missingKeys = new List<string>();
        foreach (LockSlot slot in locks)
        {
            if (slot.open) continue;

            if (inventory != null && inventory.HasItem(slot.key))
            {
                slot.open = true;
                if (slot.model != null) slot.model.SetActive(false);
                inventory.RemoveItem(slot.key); // the key is used up, so it can't open another lock
            }
            else
            {
                missingKeys.Add(slot.key);
            }
        }

        if (missingKeys.Count == 0)
        {
            if (linkedDoor != null) linkedDoor.Unlock();
            return true;
        }

        Debug.Log(lockedMessage + " " + missingKeys.Count + " lock(s) left (needs: " + string.Join(", ", missingKeys) + ")");
        return false;
    }

    /// <summary>Opens every lock on this door, hides the lock models, and unlocks the linked leaf (if any).</summary>
    public void Unlock()
    {
        if (!IsLocked) return; // also stops the two linked leaves calling each other forever

        foreach (LockSlot slot in locks)
        {
            slot.open = true;
            if (slot.model != null) slot.model.SetActive(false);
        }

        if (linkedDoor != null) linkedDoor.Unlock();
    }

    // NOTE: this uses Unity's classic Input class. If Unity throws an error about it, either set
    // Project Settings > Player > Active Input Handling to "Both", or replace the check below with
    // Keyboard.current.fKey.wasPressedThisFrame (needs: using UnityEngine.InputSystem;).
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            Toggle(player);
        }
    }

    /// <summary>
    /// Opens the door if it's closed, closes it if it's open. Ignored while the door is mid-swing.
    /// Public so another system (e.g. the player's interaction script) can call it
    /// instead of relying on the built-in key check above.
    /// </summary>
    public void Toggle(Transform user)
    {
        if (isMoving) return;

        // Locked doors only open once the player has every key (locks open one by one as keys are found).
        if (IsLocked && !TryUnlock(user)) return;

        if (isOpen) Close();
        else Open(user);
    }

    public void Open(Transform opener)
    {
        if (isOpen || isMoving) return;

        Quaternion openRotation = Quaternion.AngleAxis(GetSwingAngle(opener), Vector3.up) * closedRotation;
        StartCoroutine(SwingTo(openRotation, true));
    }

    public void Close()
    {
        if (!isOpen || isMoving) return;

        StartCoroutine(SwingTo(closedRotation, false));
    }

    // Works out whether to rotate +90 or -90 so the door swings away from whoever opened it.
    // Only called while the door is fully closed, so the panel's position is its resting position.
    private float GetSwingAngle(Transform opener)
    {
        Renderer panel = doorPivot.GetComponentInChildren<Renderer>();
        if (panel == null)
        {
            Debug.LogError("Door: no Renderer found under DoorPivot. Is the door model a child of the pivot?", this);
            return openAngle;
        }

        // Direction from the hinge to the middle of the door panel, flattened onto the ground plane.
        Vector3 hingeToPanel = panel.bounds.center - doorPivot.position;
        hingeToPanel.y = 0f;

        if (hingeToPanel.sqrMagnitude < 0.0001f)
        {
            Debug.LogWarning("Door: DoorPivot is sitting in the middle of the door. Move it to the hinge edge.", this);
            return openAngle;
        }

        // A positive rotation around world-up swings the panel along this direction.
        Vector3 swingDirection = Vector3.Cross(Vector3.up, hingeToPanel).normalized;

        Vector3 hingeToPlayer = opener.position - doorPivot.position;
        hingeToPlayer.y = 0f;

        // If a positive swing would head toward the player, swing the other way instead.
        return Vector3.Dot(swingDirection, hingeToPlayer) > 0f ? -openAngle : openAngle;
    }

    private IEnumerator SwingTo(Quaternion target, bool opening)
    {
        isMoving = true;
        Quaternion startRotation = doorPivot.rotation;

        float elapsed = 0f;
        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / openDuration);
            doorPivot.rotation = Quaternion.Slerp(startRotation, target, t);
            yield return null;
        }

        doorPivot.rotation = target; // snap exactly to the final angle
        isOpen = opening;
        isMoving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        player = other.transform;
        playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
    }

    // Select the door in the Scene view to see a yellow sphere where the hinge is.
    private void OnDrawGizmosSelected()
    {
        Transform pivot = doorPivot != null ? doorPivot : transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pivot.position, 0.1f);
    }
}
