using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Shows the death screen when the player dies. The Respawn button sends the player back to the start of the
/// first level, puts every coin back in the chest it came from (chests close and refill), and keeps everything
/// else: items bought from the merchant, carried keys, opened locks, destroyed boulders.
///
/// Setup: put this on the Canvas root (an always-active object, like PauseMenu), drag in the death panel,
/// and hook the Respawn button's OnClick to DeathScreen > Respawn().
/// </summary>
public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;
    [Tooltip("Seconds after dying before the death screen appears.")]
    [SerializeField] private float delayBeforeShowing = 1.5f;

    [Header("Respawn")]
    [Tooltip("Exact name of the first level scene (must be in File > Build Profiles).")]
    [SerializeField] private string startScene = "Dungeon Level 1";
    [Tooltip("Id of the SpawnPoint in that scene (the same one the Guiding Talisman uses).")]
    [SerializeField] private string startSpawnId = "GameStart";
    [SerializeField] private float fadeDuration = 0.8f;

    [Header("Optional")]
    [Tooltip("Exact name of the main menu scene, for a 'Main Menu' button.")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private AudioClip deathSound;

    /// <summary>True while the death screen is up (the pause menu checks this).</summary>
    public static bool IsShowing { get; private set; }

    private void Awake()
    {
        if (deathPanel != null) deathPanel.SetActive(false);
        IsShowing = false;
    }

    private void OnEnable() => Health.PlayerDied += OnPlayerDied;
    private void OnDisable() => Health.PlayerDied -= OnPlayerDied;

    private void OnPlayerDied()
    {
        if (deathSound != null && Camera.main != null)
            AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);
        StartCoroutine(ShowAfterDelay());
    }

    private IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeShowing);

        IsShowing = true;
        if (deathPanel != null) deathPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>Hook the Respawn button's OnClick to this.</summary>
    public void Respawn()
    {
        if (!IsShowing) return;

        if (!Application.CanStreamedLevelBeLoaded(startScene))
        {
            Debug.LogError("DeathScreen: can't load '" + startScene + "'. Check the name and File > Build Profiles.", this);
            return;
        }

        // All the treasure goes back where it came from; bought items, keys and world changes are kept.
        TreasureManager.Shared.ResetAllTreasure();

        IsShowing = false;
        if (deathPanel != null) deathPanel.SetActive(false);
        Time.timeScale = 1f;

        SceneTransition.Go(startScene, startSpawnId, fadeDuration);
    }

    /// <summary>Optional: hook a 'Main Menu' button to this.</summary>
    public void GoToMainMenu()
    {
        IsShowing = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    private void OnDestroy()
    {
        IsShowing = false;
    }
}
