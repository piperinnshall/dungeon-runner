using UnityEngine;

/// <summary>
/// Guiding Talisman. Put this on PlayerMain. If the player is carrying a talisman (bought from the merchant),
/// pressing the use key (T) uses it up and warps them back to the start of the game.
///
/// Setup: in the starting scene, place a SpawnPoint where the player should appear and give it the same id
/// as "Start Spawn Id" below. The starting scene must be in File > Build Profiles.
/// </summary>
public class Talisman : MonoBehaviour
{
    [SerializeField] private KeyCode useKey = KeyCode.T;
    [Tooltip("Exact name of the scene the game starts in.")]
    [SerializeField] private string startScene = "Dungeon Level 1";
    [Tooltip("Id of the SpawnPoint in that scene where the player should appear.")]
    [SerializeField] private string startSpawnId = "GameStart";
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private AudioClip useSound;

    // NOTE: uses Unity's classic Input class. If Unity throws an error about it, set
    // Project Settings > Player > Active Input Handling to "Both".
    private void Update()
    {
        if (!Input.GetKeyDown(useKey)) return;
        if (Merchant.ShopOpen || Time.timeScale <= 0f) return; // not while paused or shopping

        if (!PlayerItems.HasTalisman)
        {
            Debug.Log("You don't have a Guiding Talisman.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(startScene))
        {
            Debug.LogError("Talisman: can't load '" + startScene + "'. Check the name and File > Build Profiles.", this);
            return;
        }

        PlayerItems.UseTalisman();
        if (useSound != null) AudioSource.PlayClipAtPoint(useSound, transform.position);
        SceneTransition.Go(startScene, startSpawnId, fadeDuration);
    }
}
