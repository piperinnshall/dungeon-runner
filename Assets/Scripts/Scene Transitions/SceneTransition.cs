using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Fades to black, loads another scene, then fades back in (the Skyrim load-door effect).
/// You don't place this in any scene: it creates itself the first time a SceneDoor uses it and
/// survives scene changes, which is what lets it finish the fade after the old scene is gone.
/// </summary>
public class SceneTransition : MonoBehaviour
{
    private static SceneTransition instance;

    /// <summary>The SpawnPoint id the player should appear at in the scene that is loading.</summary>
    public static string PendingSpawnId { get; private set; }

    private float alpha;
    private bool busy;

    public static void Go(string sceneName, string spawnId, float fadeDuration = 0.5f)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"SceneTransition: scene '{sceneName}' can't be loaded. " +
                           "Check the spelling and add it to the scene list in File > Build Profiles.");
            return;
        }

        if (instance == null)
        {
            GameObject host = new GameObject("SceneTransition");
            DontDestroyOnLoad(host);
            instance = host.AddComponent<SceneTransition>();
        }

        if (instance.busy) return;
        instance.StartCoroutine(instance.Run(sceneName, spawnId, fadeDuration));
    }

    /// <summary>Called by a SpawnPoint once it has placed the player.</summary>
    public static void ClearPendingSpawn()
    {
        PendingSpawnId = null;
    }

    private IEnumerator Run(string sceneName, string spawnId, float fadeDuration)
    {
        busy = true;
        PendingSpawnId = spawnId;

        yield return Fade(0f, 1f, fadeDuration);

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone) yield return null;

        // Wait a frame so the new scene's Start() methods run (that's when SpawnPoints place the
        // player), then a moment longer so the camera can settle before the scene is revealed.
        yield return null;
        yield return new WaitForSecondsRealtime(0.15f);

        if (!string.IsNullOrEmpty(PendingSpawnId))
        {
            Debug.LogWarning($"SceneTransition: no SpawnPoint with id '{PendingSpawnId}' found in '{sceneName}'. " +
                             "The player stays wherever that scene placed them.");
            PendingSpawnId = null;
        }

        yield return Fade(1f, 0f, fadeDuration);
        busy = false;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        alpha = to;
    }

    // A plain black rectangle over the whole screen. Simple on purpose; it can be swapped for a
    // proper UI image later without changing how the doors work.
    private void OnGUI()
    {
        if (alpha <= 0f) return;

        GUI.depth = -1000;
        GUI.color = new Color(0f, 0f, 0f, alpha);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
    }
}
