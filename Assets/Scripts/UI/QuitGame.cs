using UnityEngine;

/// <summary>
/// Put this on any object in the main menu (an empty "MenuActions" object is fine), then hook the Quit
/// button's OnClick to QuitGame(). Works in a built game, and also stops Play mode in the Editor
/// (Application.Quit() does nothing inside the Editor).
/// </summary>
public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
