using System;
using UnityEngine;

/// <summary>
/// Which items the player has unlocked and how many bombs they carry. It's static, so it survives
/// scene transitions (like the coins in TreasureManager.Shared). Nothing needs to be placed in a scene.
///
///   PlayerItems.TryUseBomb()      - Movement calls this when throwing; false = no bombs left / not unlocked
///   PlayerItems.RefillBombs()     - the campfire will call this (back to MaxBombs)
///   PlayerItems.UnlockBombs() / UnlockBoomerang() / UnlockLantern() - the shop will call these
///   PlayerItems.Changed           - fired on any change, the HUD listens to it
/// </summary>
public static class PlayerItems
{
    public const int MaxBombs = 10;

    // ---- STARTING LOADOUT ----------------------------------------------------------------------
    // While there is no shop yet, the player starts with bombs and the boomerang so the game still
    // plays like before. Once the shop exists, set these to false.
    private const bool StartWithBombs = true;
    private const bool StartWithBoomerang = true;
    private const bool StartWithLantern = false;
    // ---------------------------------------------------------------------------------------------

    public static bool HasBombs { get; private set; }
    public static int Bombs { get; private set; }
    public static bool HasBoomerang { get; private set; }
    public static bool HasLantern { get; private set; }

    public static event Action Changed;

    // Runs every time you press Play (even if Unity's domain reload is turned off), so each play
    // session starts from the starting loadout above.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetToStartingLoadout()
    {
        HasBombs = StartWithBombs;
        Bombs = StartWithBombs ? MaxBombs : 0;
        HasBoomerang = StartWithBoomerang;
        HasLantern = StartWithLantern;
        Changed = null;
    }

    public static bool TryUseBomb()
    {
        if (!HasBombs || Bombs <= 0) return false;
        Bombs--;
        Changed?.Invoke();
        return true;
    }

    public static void RefillBombs()
    {
        if (!HasBombs) return;
        Bombs = MaxBombs;
        Changed?.Invoke();
    }

    public static void UnlockBombs()
    {
        HasBombs = true;
        Bombs = MaxBombs;
        Changed?.Invoke();
    }

    public static void UnlockBoomerang()
    {
        HasBoomerang = true;
        Changed?.Invoke();
    }

    public static void UnlockLantern()
    {
        HasLantern = true;
        Changed?.Invoke();
    }
}
