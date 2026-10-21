using UnityEngine;

/// <summary>
/// An altar the player can light. When the player stands inside the altar's trigger zone and presses
/// the interact key (E by default) the fire turns on. One use only.
/// Each SlidingDoor assigned (the "Door" field plus any "Extra Doors") opens once ALL altars pointing at it are lit.
/// So one altar can open several doors, and one door can wait for several altars.
///
/// Setup:
///   Altar (this script + BoxCollider with "Is Trigger" ticked, big enough to stand in front of)
///   |-- altar model (with its own solid collider)
///   Fire objects (the fire particle effect and the point light) - drag them into "Light Up Objects".
///   Jail door - put the SlidingDoor script on it and drag it into "Door".
/// </summary>
public class Altar : MonoBehaviour
{
    [Header("Fire")]
    [Tooltip("Everything that turns on when the altar is lit (the fire particles, the point light...). They are switched off when the game starts.")]
    [SerializeField] private GameObject[] lightUpObjects;

    [Header("Door")]
    [Tooltip("The SlidingDoor this altar helps to open. Leave empty for an altar that only lights a fire.")]
    [SerializeField] private SlidingDoor door;
    [Tooltip("More doors this altar helps to open. Leave empty if it only has one door.")]
    [SerializeField] private SlidingDoor[] extraDoors;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInRange;
    private bool isLit;

    public bool IsLit => isLit;

    private void Awake()
    {
        // The altar starts unlit.
        foreach (GameObject obj in lightUpObjects)
            if (obj != null) obj.SetActive(false);

        // Tell each door this altar exists so it knows to wait for it.
        if (door != null) door.Register(this);
        foreach (SlidingDoor extra in extraDoors)
            if (extra != null) extra.Register(this);
    }

    // NOTE: this uses Unity's classic Input class. If Unity throws an error about it, set
    // Project Settings > Player > Active Input Handling to "Both".
    private void Update()
    {
        if (playerInRange && !isLit && Input.GetKeyDown(interactKey))
            Light();
    }

    public void Light()
    {
        if (isLit) return;
        isLit = true;

        foreach (GameObject obj in lightUpObjects)
            if (obj != null) obj.SetActive(true);

        // Fire audio, if a FireSound component is on the altar.
        FireSound fireSound = GetComponent<FireSound>();
        if (fireSound != null) fireSound.Ignite();

        if (door != null) door.AltarLit(this);
        foreach (SlidingDoor extra in extraDoors)
            if (extra != null) extra.AltarLit(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Inventory>() != null || other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Inventory>() != null || other.CompareTag("Player"))
            playerInRange = false;
    }
}
