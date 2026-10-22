using TMPro;
using UnityEngine;

/// <summary>
/// Shows the player's coin total. Put it on a TextMeshPro text object in an always-visible Canvas
/// (e.g. the pause menu Canvas prefab, but NOT inside the pause panel). Updates whenever coins change.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class CoinCounter : MonoBehaviour
{
    [Tooltip("Text shown before the number.")]
    [SerializeField] private string prefix = "Coins: ";

    private TMP_Text label;

    private void Awake()
    {
        label = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        TreasureManager.Shared.CoinsChanged += Show;
        Show(TreasureManager.Shared.GetTotalCoins());
    }

    private void OnDisable()
    {
        TreasureManager.Shared.CoinsChanged -= Show;
    }

    private void Show(int coins)
    {
        label.text = prefix + coins;
    }
}
