using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD row for one item (bombs, boomerang or lantern). While the item is locked, the row is hidden
/// (or the icon greyed out). For bombs it also shows how many are left, e.g. "x7".
///
/// Setup: put this on the item's ROW object (e.g. "BombRow"). Pick the Item, drag in the row's icon
/// Image and, for bombs, the counter text.
/// </summary>
public class ItemDisplay : MonoBehaviour
{
    public enum Item { Bombs, Boomerang, Lantern, Talisman }

    [SerializeField] private Item item = Item.Bombs;
    [SerializeField] private Image icon;
    [Tooltip("Bombs only: the counter text.")]
    [SerializeField] private TMP_Text countText;
    [SerializeField] private string countPrefix = "x";
    [Tooltip("On = show a greyed-out icon while locked. Off = hide the icon (and counter) while locked.")]
    [SerializeField] private bool greyOutWhenLocked = false;
    [SerializeField] private Color lockedColour = new Color(1f, 1f, 1f, 0.3f);

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
        bool unlocked = item switch
        {
            Item.Bombs => PlayerItems.HasBombs,
            Item.Boomerang => PlayerItems.HasBoomerang,
            Item.Lantern => PlayerItems.HasLantern,
            _ => PlayerItems.HasTalisman,
        };

        if (icon != null)
        {
            icon.gameObject.SetActive(unlocked || greyOutWhenLocked);
            icon.color = unlocked ? Color.white : lockedColour;
        }

        if (countText != null)
        {
            countText.gameObject.SetActive(unlocked);
            if (item == Item.Bombs) countText.text = countPrefix + PlayerItems.Bombs;
        }
    }
}
