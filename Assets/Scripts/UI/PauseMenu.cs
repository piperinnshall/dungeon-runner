using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Esc opens the pause menu and freezes the game; Esc again (or the Resume button) carries on exactly
/// where it was.
///
/// Put this on an object that is ALWAYS active (for example the Canvas, or an empty "PauseManager"),
/// NOT on the pause panel itself: a disabled object doesn't run Update, so it could never reopen.
/// Then hook the panel's buttons up to Resume(), GoToMainMenu() and Quit() in each button's OnClick.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("The panel that holds the pause menu buttons. It is hidden when the game starts.")]
    [SerializeField] private GameObject pausePanel;

    [Header("Controls")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("Optional")]
    [Tooltip("Scripts to switch off while paused, e.g. the player's Movement and the camera controller, so the player can't move or look around behind the menu.")]
    [SerializeField] private Behaviour[] disableWhilePaused;
    [Tooltip("Exact name of the main menu scene (must be in File > Build Profiles). Needed for the Main Menu button.")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool isPaused;
    private CursorLockMode previousLockState;
    private bool previousCursorVisible;

    public bool IsPaused => isPaused;

    private void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // NOTE: this uses Unity's classic Input class. If Unity throws an error about it, set
    // Project Settings > Player > Active Input Handling to "Both".
    private void Update()
    {
        // The shop has its own pause; don't open the pause menu on top of it, or with the Esc that closed it.
        if (Merchant.ShopOpen || Merchant.ClosedOnFrame == Time.frameCount) return;
        if (DeathScreen.IsShowing) return; // no pausing on the death screen

        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;

        Time.timeScale = 0f;      // freezes movement, animation, physics, timers
        AudioListener.pause = true;

        // Remember the cursor settings so Resume can put them back exactly as they were.
        previousLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetScriptsEnabled(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;

        if (pausePanel != null) pausePanel.SetActive(false);
        SetScriptsEnabled(true);

        Cursor.lockState = previousLockState;
        Cursor.visible = previousCursorVisible;

        AudioListener.pause = false;
        Time.timeScale = 1f;
    }

    public void GoToMainMenu()
    {
        UnfreezeEverything();
        SceneManager.LoadScene(mainMenuScene);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetScriptsEnabled(bool enabled)
    {
        foreach (Behaviour script in disableWhilePaused)
            if (script != null) script.enabled = enabled;
    }

    // Time.timeScale survives scene loads, so always put it back before leaving.
    private void UnfreezeEverything()
    {
        isPaused = false;
        AudioListener.pause = false;
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        UnfreezeEverything();
    }
}
