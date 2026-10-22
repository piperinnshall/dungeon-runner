using TMPro;
using UnityEngine;

/// <summary>
/// The merchant. Walk up, press E: the shop window opens and the game pauses. Each ShopRow in the window
/// shows an item with its price and a Buy button. Press E again (or a Close button) to leave.
///
/// Setup: put this on the merchant with a BoxCollider ("Is Trigger" ticked, big enough to stand in front of).
/// Drag the shop window panel (in a Canvas) into "Shop Window". Prices are set below.
/// </summary>
public class Merchant : MonoBehaviour
{
    public enum ShopItem { Bombs, Boomerang, Lantern, Heart, Talisman }

    public struct Offer
    {
        public string title;
        public int price;
        public bool available;
        public string status; // shown instead of the price when it can't be bought
    }

    [Header("Shop window")]
    [Tooltip("The shop panel (inside a Canvas). It is hidden when the game starts.")]
    [SerializeField] private GameObject shopWindow;
    [Tooltip("Optional text in the window for messages like 'Not enough coins!'.")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Prices (coins)")]
    [SerializeField] private int bombPrice = 10;
    [Tooltip("Price of the 1st and 2nd bomb bag upgrades (+5 bombs each).")]
    [SerializeField] private int[] bombUpgradePrices = { 20, 35 };
    [SerializeField] private int boomerangPrice = 25;
    [SerializeField] private int lanternPrice = 20;
    [Tooltip("Price of the 1st, 2nd and 3rd heart upgrades (+2 hearts each).")]
    [SerializeField] private int[] heartPrices = { 15, 25, 40 };
    [SerializeField] private int talismanPrice = 10;

    [Header("Sound (optional)")]
    [SerializeField] private AudioClip buySound;
    [SerializeField] private AudioClip cantBuySound;

    [Header("Item tutorials (optional)")]
    [Tooltip("A full-screen Raw Image (last child of the Canvas, so it draws over the shop). Shows the tutorial until clicked.")]
    [SerializeField] private UnityEngine.UI.RawImage tutorialDisplay;
    [SerializeField] private Texture bombTutorial;
    [SerializeField] private Texture boomerangTutorial;
    [SerializeField] private Texture lanternTutorial;
    [SerializeField] private Texture heartTutorial;
    [SerializeField] private Texture talismanTutorial;
    [Tooltip("On = each tutorial only shows the first time that item is bought (not for every bomb bag / heart upgrade).")]
    [SerializeField] private bool tutorialOnlyFirstTime = true;
    [Tooltip("Ignore clicks for this long after a tutorial appears, so the Buy click doesn't close it straight away.")]
    [SerializeField] private float tutorialClickCooldown = 0.3f;

    /// <summary>True while any shop window is open (the pause menu and talisman check this).</summary>
    public static bool ShopOpen { get; private set; }

    /// <summary>The frame the shop was last closed on (the pause menu ignores Esc on that frame).</summary>
    public static int ClosedOnFrame { get; private set; } = -1;

    // Which tutorials have been seen this play session (shared by all merchants, survives scene changes).
    private static readonly System.Collections.Generic.HashSet<ShopItem> seenTutorials =
        new System.Collections.Generic.HashSet<ShopItem>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSeenTutorials() => seenTutorials.Clear();

    private bool tutorialShowing;
    private float tutorialShownAt;

    private bool isOpen;
    private bool playerInRange;
    private Movement playerMovement;
    private Health playerHealth;
    private ShopRow[] rows;
    private CursorLockMode previousLockState;
    private bool previousCursorVisible;
    private AudioSource audioSource;

    private void Awake()
    {
        if (shopWindow != null)
        {
            rows = shopWindow.GetComponentsInChildren<ShopRow>(true);
            foreach (ShopRow row in rows) row.Bind(this);
            shopWindow.SetActive(false);
        }
        else
        {
            rows = new ShopRow[0];
            Debug.LogWarning("Merchant: no Shop Window assigned.", this);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.ignoreListenerPause = true; // still plays while the game is paused

        if (tutorialDisplay != null) tutorialDisplay.gameObject.SetActive(false);
    }

    // NOTE: uses Unity's classic Input class. If Unity throws an error about it, set
    // Project Settings > Player > Active Input Handling to "Both".
    private void Update()
    {
        // A tutorial is on screen: any click / key closes it, and nothing else happens meanwhile.
        if (tutorialShowing)
        {
            if (Time.unscaledTime - tutorialShownAt >= tutorialClickCooldown && Input.anyKeyDown)
            {
                tutorialShowing = false;
                tutorialDisplay.gameObject.SetActive(false);
            }
            return;
        }

        if (isOpen)
        {
            // E or Esc leaves the shop.
            if (Input.GetKeyDown(interactKey) || Input.GetKeyDown(KeyCode.Escape)) Close();
            return;
        }

        if (playerInRange && !ShopOpen && Time.timeScale > 0f && Input.GetKeyDown(interactKey))
            Open();
    }

    public void Open()
    {
        if (shopWindow == null || isOpen) return;
        isOpen = true;
        ShopOpen = true;

        shopWindow.SetActive(true);
        Time.timeScale = 0f;

        previousLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Stop clicks on the shop buttons from also making the player attack.
        if (playerMovement != null) playerMovement.enabled = false;

        ShowMessage("");
        RefreshRows();
    }

    /// <summary>Closes the shop. Hook a "Close" button's OnClick to this if you want one.</summary>
    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        ShopOpen = false;
        ClosedOnFrame = Time.frameCount; // so the same Esc press doesn't also open the pause menu

        tutorialShowing = false;
        if (tutorialDisplay != null) tutorialDisplay.gameObject.SetActive(false);

        shopWindow.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = previousLockState;
        Cursor.visible = previousCursorVisible;

        if (playerMovement != null) playerMovement.enabled = true;
    }

    public Offer GetOffer(ShopItem item)
    {
        switch (item)
        {
            case ShopItem.Bombs:
                int level = PlayerItems.BombLevel;
                if (level == 0) return Available("Bombs (carry 5)", bombPrice);
                if (level < PlayerItems.MaxBombLevel)
                    return Available("Bomb Bag (carry " + (PlayerItems.BombCapacity + PlayerItems.BombsPerLevel) + ")",
                                     PriceAt(bombUpgradePrices, level - 1));
                return Unavailable("Bomb Bag (carry " + PlayerItems.BombCapacity + ")", "Sold out");

            case ShopItem.Boomerang:
                return PlayerItems.HasBoomerang ? Unavailable("Boomerang", "Sold out") : Available("Boomerang", boomerangPrice);

            case ShopItem.Lantern:
                return PlayerItems.HasLantern ? Unavailable("Lantern", "Sold out") : Available("Lantern", lanternPrice);

            case ShopItem.Heart:
                int upgrades = PlayerItems.HeartUpgrades;
                string heartTitle = "Heart Upgrade (" + upgrades + "/" + PlayerItems.MaxHeartUpgrades + ")";
                return upgrades < PlayerItems.MaxHeartUpgrades
                    ? Available(heartTitle, PriceAt(heartPrices, upgrades))
                    : Unavailable(heartTitle, "Sold out");

            default: // Talisman
                return PlayerItems.HasTalisman ? Unavailable("Guiding Talisman", "Already carrying one")
                                               : Available("Guiding Talisman", talismanPrice);
        }
    }

    public void Buy(ShopItem item)
    {
        Offer offer = GetOffer(item);

        if (!offer.available)
        {
            ShowMessage(offer.status);
            Play(cantBuySound);
            return;
        }

        if (!TreasureManager.Shared.TrySpend(offer.price))
        {
            ShowMessage("Not enough coins!");
            Play(cantBuySound);
            return;
        }

        switch (item)
        {
            case ShopItem.Bombs: PlayerItems.UpgradeBombs(); break;
            case ShopItem.Boomerang: PlayerItems.UnlockBoomerang(); break;
            case ShopItem.Lantern: PlayerItems.UnlockLantern(); break;
            case ShopItem.Heart:
                PlayerItems.AddHeartUpgrade();
                if (playerHealth != null)
                {
                    playerHealth.maxHealth = PlayerItems.MaxHealth;
                    playerHealth.health = Mathf.Min(playerHealth.health + PlayerItems.HeartsPerUpgrade, playerHealth.maxHealth);
                }
                break;
            case ShopItem.Talisman: PlayerItems.GiveTalisman(); break;
        }

        ShowMessage("Bought " + offer.title + "!");
        Play(buySound);
        RefreshRows();
        ShowTutorial(item);
    }

    private void ShowTutorial(ShopItem item)
    {
        if (tutorialDisplay == null) return;
        if (tutorialOnlyFirstTime && seenTutorials.Contains(item)) return;

        Texture page = item switch
        {
            ShopItem.Bombs => bombTutorial,
            ShopItem.Boomerang => boomerangTutorial,
            ShopItem.Lantern => lanternTutorial,
            ShopItem.Heart => heartTutorial,
            _ => talismanTutorial,
        };
        if (page == null) return;

        seenTutorials.Add(item);
        tutorialDisplay.texture = page;
        tutorialDisplay.gameObject.SetActive(true);
        tutorialShowing = true;
        tutorialShownAt = Time.unscaledTime;
    }

    private void RefreshRows()
    {
        foreach (ShopRow row in rows) row.Refresh();
    }

    private void ShowMessage(string message)
    {
        if (messageText != null) messageText.text = message;
    }

    private void Play(AudioClip clip)
    {
        if (clip != null) audioSource.PlayOneShot(clip);
    }

    private static int PriceAt(int[] prices, int index)
    {
        if (prices == null || prices.Length == 0) return 0;
        return prices[Mathf.Clamp(index, 0, prices.Length - 1)];
    }

    private static Offer Available(string title, int price) =>
        new Offer { title = title, price = price, available = true, status = "" };

    private static Offer Unavailable(string title, string status) =>
        new Offer { title = title, price = 0, available = false, status = status };

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        playerMovement = other.GetComponentInParent<Movement>();
        playerHealth = other.GetComponentInParent<Health>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

    // Never leave the game frozen if this object is removed (e.g. a scene change) while the shop is open.
    private void OnDestroy()
    {
        if (isOpen)
        {
            Time.timeScale = 1f;
            ShopOpen = false;
        }
    }
}
