using System.Collections;
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
