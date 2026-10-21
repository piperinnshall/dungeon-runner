using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One item in the shop window: its name, its price (or "Sold out") and a Buy button.
/// Put this on each row inside the shop window, choose the Item and drag in the two texts and the button.
/// The Merchant finds the rows itself and wires up the buttons, so no OnClick setup is needed.
/// </summary>
public class ShopRow : MonoBehaviour
{
    [SerializeField] private Merchant.ShopItem item;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;

    private Merchant merchant;

    public void Bind(Merchant owner)
    {
        merchant = owner;
        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(OnBuyClicked);
            buyButton.onClick.AddListener(OnBuyClicked);
        }
    }

    private void OnBuyClicked()
    {
        if (merchant != null) merchant.Buy(item);
    }

    public void Refresh()
    {
        if (merchant == null) return;

        Merchant.Offer offer = merchant.GetOffer(item);
        if (nameText != null) nameText.text = offer.title;
        if (priceText != null) priceText.text = offer.available ? offer.price + " coins" : offer.status;
        if (buyButton != null) buyButton.interactable = offer.available;
    }
}
