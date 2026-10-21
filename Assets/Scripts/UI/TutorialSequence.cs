using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Shows a series of full-screen tutorial images, one after another. Each click (or key press) moves to the
/// next one; after the last image, "On Finished" runs (e.g. whatever your Play button used to do to start the game).
///
/// Setup (main menu scene):
///   - Put this on an always-active object (the Canvas is fine).
///   - Make a full-screen Raw Image as the LAST child of the Canvas and drag it into "Display".
///   - Drag the tutorial textures into "Pages" in order.
///   - Play button OnClick: call TutorialSequence.Begin() instead of starting the game.
///   - Move the Play button's old OnClick entry (the one that starts the game) into "On Finished".
/// </summary>
public class TutorialSequence : MonoBehaviour
{
    [SerializeField] private RawImage display;
    [Tooltip("Tutorial images in the order they are shown.")]
    [SerializeField] private Texture[] pages;
    [Tooltip("What happens after the last page is clicked, e.g. start the game.")]
    [SerializeField] private UnityEvent onFinished;
    [Tooltip("Ignore clicks for this long after a page appears, so one click doesn't skip two pages.")]
    [SerializeField] private float clickCooldown = 0.25f;

    private int index = -1;
    private bool showing;
    private float shownAt;

    private void Awake()
    {
        if (display != null) display.gameObject.SetActive(false);
    }

    /// <summary>Hook the Play button's OnClick to this.</summary>
    public void Begin()
    {
        if (showing) return;

        if (display == null || pages == null || pages.Length == 0)
        {
            onFinished.Invoke();
            return;
        }

        index = 0;
        ShowCurrent();
    }

    private void ShowCurrent()
    {
        display.texture = pages[index];
        display.gameObject.SetActive(true);
        showing = true;
        shownAt = Time.unscaledTime;
    }

    // NOTE: uses Unity's classic Input class. If Unity throws an error about it, set
    // Project Settings > Player > Active Input Handling to "Both".
    private void Update()
    {
        if (!showing) return;
        if (Time.unscaledTime - shownAt < clickCooldown) return;

        // Any mouse click or key press moves on.
        if (Input.anyKeyDown) Next();
    }

    private void Next()
    {
        index++;
        if (index < pages.Length)
        {
            ShowCurrent();
            return;
        }

        // Last page done. Leave the final image on screen while the game loads, so the menu doesn't flash.
        showing = false;
        onFinished.Invoke();
    }
}
