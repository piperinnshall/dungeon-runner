using System;
using UnityEngine;

/// <summary>
/// The lantern's light around the player. Put this on PlayerMain and drag in a Point Light that is a child of
/// the player (and the lantern model, if you have one). They switch on once the lantern is bought, but only
/// in the scenes listed under "Dark Scenes" (the underground levels).
/// </summary>
public class PlayerLantern : MonoBehaviour
{
    [SerializeField] private Light lanternLight;
    [Tooltip("Optional: a lantern model on the player, shown together with the light.")]
    [SerializeField] private GameObject lanternModel;
    [Tooltip("Exact scene names where the lantern lights up.")]
    [SerializeField] private string[] darkScenes = { "Dungeon Level 2", "Dungeon Level 3" };

    private void OnEnable()
    {
        PlayerItems.Changed += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        PlayerItems.Changed -= Refresh;
    }

    private void Refresh()
    {
        bool inDarkScene = Array.IndexOf(darkScenes, gameObject.scene.name) >= 0;
        bool on = PlayerItems.HasLantern && inDarkScene;

        if (lanternLight != null) lanternLight.enabled = on;
        if (lanternModel != null) lanternModel.SetActive(on);
    }
}
