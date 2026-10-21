using System;
using UnityEngine;

/// <summary>
/// Everything the player has bought from the merchant, plus how many bombs they carry. It's static, so it
/// survives scene transitions (like the coins in TreasureManager.Shared). Nothing needs to be placed in a scene.
///
///   Bombs:     BombLevel 0 = none, 1 = carry 5, 2 = carry 10, 3 = carry 15. TryUseBomb() / RefillBombs()
///   Hearts:    MaxHealth = 3 + 2 per heart upgrade (max 3 upgrades = 9 hearts)
///   Boomerang / Lantern: unlocked once bought
///   Talisman:  only one can be carried at a time; UseTalisman() uses it up
///   Changed:   fired on any change, the HUD and shop listen to it
/// </summary>
public static class PlayerItems
{
    public const int BombsPerLevel = 5;
    public const int MaxBombLevel = 3;          // 5, 10, 15

    public const int BaseHearts = 3;
    public const int HeartsPerUpgrade = 2;
    public const int MaxHeartUpgrades = 3;      // 3 -> 5 -> 7 -> 9

    public static int BombLevel { get; private set; }
    public static bool HasBombs => BombLevel > 0;
    public static int BombCapacity => BombLevel * BombsPerLevel;
    public static int MaxBombs => BombCapacity;  // kept for older code
    public static int Bombs { get; private set; }

    public static bool HasBoomerang { get; private set; }
    public static bool HasLantern { get; private set; }
    public static bool HasTalisman { get; private set; }

    public static int HeartUpgrades { get; private set; }
    public static int MaxHealth => BaseHearts + HeartUpgrades * HeartsPerUpgrade;

    public static event Action Changed;

    // Runs every time you press Play (even if Unity's domain reload is turned off): the player starts
    // with nothing and has to buy everything from the merchant.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetToStart()
    {
        BombLevel = 0;
        Bombs = 0;
        HasBoomerang = false;
        HasLantern = false;
        HasTalisman = false;
        HeartUpgrades = 0;
        Changed = null;
    }

    // ---- Bombs ----
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
        Bombs = BombCapacity;
        Changed?.Invoke();
    }

    /// <summary>First purchase unlocks bombs (carry 5), each one after adds 5 more capacity. Fills the bag.</summary>
    public static bool UpgradeBombs()
    {
        if (BombLevel >= MaxBombLevel) return false;
        BombLevel++;
        Bombs = BombCapacity;
        Changed?.Invoke();
        return true;
    }

    public static void UnlockBombs()
    {
        if (!HasBombs) UpgradeBombs();
    }

    // ---- Boomerang / Lantern ----
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

    // ---- Hearts ----
    public static bool AddHeartUpgrade()
    {
        if (HeartUpgrades >= MaxHeartUpgrades) return false;
        HeartUpgrades++;
        Changed?.Invoke();
        return true;
    }

    // ---- Talisman ----
    public static bool GiveTalisman()
    {
        if (HasTalisman) return false;
        HasTalisman = true;
        Changed?.Invoke();
        return true;
    }

    public static bool UseTalisman()
    {
        if (!HasTalisman) return false;
        HasTalisman = false;
        Changed?.Invoke();
        return true;
    }
}
