using UnityEngine;

/// <summary>
/// A "load door" (like in Skyrim). When the player is inside the trigger zone and presses the
/// interact key, the screen fades out, the target scene loads, and the player appears at the
/// SpawnPoint in that scene whose id matches "Target Spawn Id".
///
/// Setup: put this on the door object, add a BoxCollider with "Is Trigger" ticked that covers the
/// floor in front of the door, and make sure the player is tagged "Player".
/// </summary>
public class SceneDoor : MonoBehaviour
{
    [Header("Destination")]
    [Tooltip("Exact name of the scene to load. It must be in the scene list under File > Build Profiles.")]
    [SerializeField] private string targetScene;

    [Tooltip("Id of the SpawnPoint in the target scene where the player should appear.")]
    [SerializeField] private string targetSpawnId;

    [Header("Behaviour")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [Tooltip("Seconds to fade out, and again to fade back in.")]
    [SerializeField] private float fadeDuration = 0.5f;

    private bool playerInRange;

    // NOTE: this uses Unity's classic Input class. If Unity throws an error about it, either set
    // Project Settings > Player > Active Input Handling to "Both", or replace the check below with
    // Keyboard.current.fKey.wasPressedThisFrame (needs: using UnityEngine.InputSystem;).
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            Use();
        }
    }

    /// <summary>
    /// Public so another system (e.g. the player's interaction script) can call this
    /// instead of relying on the built-in key check above.
    /// </summary>
    public void Use()
    {
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogError("SceneDoor: no Target Scene set on this door.", this);
            return;
        }

        SceneTransition.Go(targetScene, targetSpawnId, fadeDuration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}
