using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Remembers changes to the world for the whole play session, so they survive scene transitions:
/// destroyed boulders, picked-up keys, opened locks and lit altars. Nothing needs to be placed in a scene.
///
/// Each object is identified by its scene name + its path in the Hierarchy (e.g. "Dungeon Level 2:Rocks/Boulder (3)"),
/// so objects that should be remembered separately must not have the SAME name under the SAME parent.
/// (Duplicating with Ctrl+D gives unique names like "Boulder (1)", "Boulder (2)" automatically.)
/// </summary>
public static class WorldState
{
    /// <summary>Tag of objects bombs can destroy (must match the Bomb's "Explodable Tag").</summary>
    public const string ExplodableTag = "Explodable";

    private static readonly HashSet<string> flags = new HashSet<string>();

    // Fresh memory every time you press Play.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        flags.Clear();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static bool Has(string id) => flags.Contains(id);
    public static void Set(string id) => flags.Add(id);

    /// <summary>Unique name for an object: scene name + Hierarchy path.</summary>
    public static string IdOf(GameObject go)
    {
        string path = go.name;
        for (Transform t = go.transform.parent; t != null; t = t.parent)
            path = t.name + "/" + path;
        return go.scene.name + ":" + path;
    }

    /// <summary>Called by the Bomb just before it destroys an Explodable object.</summary>
    public static void MarkDestroyed(GameObject go) => Set("destroyed:" + IdOf(go));

    // When a level loads, remove the Explodable objects that were already blown up earlier.
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject[] explodables;
        try
        {
            explodables = GameObject.FindGameObjectsWithTag(ExplodableTag);
        }
        catch (UnityException)
        {
            return; // the tag doesn't exist in this project
        }

        foreach (GameObject obj in explodables)
        {
            if (obj.scene == scene && Has("destroyed:" + IdOf(obj)))
                Object.Destroy(obj);
        }
    }
}
