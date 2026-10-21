using UnityEngine;

/// <summary>
/// Caps the game's frame rate. Runs automatically when the game starts, so it doesn't need to be on any object.
/// Without a cap a fast PC can run the build at hundreds of FPS, and the CharacterController then misses the
/// ground (tiny per-frame moves), so the player counts as "not grounded" and only plays the idle animation.
/// </summary>
public static class FrameRateCap
{
    private const int TargetFps = 60;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Apply()
    {
        QualitySettings.vSyncCount = 0;          // let targetFrameRate decide (works the same on every monitor)
        Application.targetFrameRate = TargetFps;
    }
}
