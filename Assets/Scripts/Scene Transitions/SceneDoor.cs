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

    [Header("Sounds (optional)")]
    [Tooltip("Plays as the player uses the door (while the screen fades out).")]
    [SerializeField] private AudioClip openSound;
    [Tooltip("Plays as the new scene fades in, like the door shutting behind the player.")]
    [SerializeField] private AudioClip closeSound;
    [Range(0f, 1f)] [SerializeField] private float soundVolume = 1f;

    private bool playerInRange;
    private bool used;

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

        if (used) return; // ignore extra presses while the screen is fading
        used = true;

        PlaySurvivingSceneLoad(openSound, 0f);
        // Roughly when the next scene starts fading in (fade out + loading + fade in start).
        PlaySurvivingSceneLoad(closeSound, fadeDuration + 0.3f);

        SceneTransition.Go(targetScene, targetSpawnId, fadeDuration);
    }

    // The door (and its scene) is destroyed when the next scene loads, which would cut a normal sound
    // off. So play it from a temporary object that survives the scene change, then delete itself.
    // 2D, because the listener (camera) changes scene too.
    private void PlaySurvivingSceneLoad(AudioClip clip, float delay)
    {
        if (clip == null) return;

        GameObject holder = new GameObject("SceneDoorSound");
        DontDestroyOnLoad(holder);

        AudioSource source = holder.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = soundVolume;
        source.spatialBlend = 0f;
        source.ignoreListenerPause = true;
        source.PlayDelayed(delay);

        Destroy(holder, delay + clip.length + 0.1f);
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
