using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardTooltip : MonoBehaviour
{
    [Header("Tooltip UI")]
    public GameObject tooltipPanel;

    [Header("Tooltip Text")]
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI manaCostText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI descriptionText;

    [Header("Tooltip Artwork")]
    public Image cardArtImage;

    void Start()
    {
        // Tooltip starts hidden.
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public void ShowTooltip(CardData cardData)
    {
        if (cardData == null)
        {
            return;
        }

        // Shows the tooltip panel.
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(true);
        }

        // Updates the card name.
        if (cardNameText != null)
        {
            cardNameText.text = cardData.cardName;
        }

        // Updates the mana cost.
        if (manaCostText != null)
        {
            manaCostText.text = "Mana: " + cardData.ManaCost;
        }

        // Updates the damage.
        if (damageText != null)
        {
            damageText.text = "Damage: " + cardData.Damage;
        }

        // Updates the description.
        if (descriptionText != null)
        {
            descriptionText.text = cardData.cardDescription;
        }

        // Uses the same artwork as the hovered card.
        if (cardArtImage != null && cardData.cardBG != null)
        {
            cardArtImage.sprite = cardData.cardBG.sprite;
        }
    }

    public void HideTooltip()
    {
        // Hides the tooltip when the mouse leaves the card.
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
}