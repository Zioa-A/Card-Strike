using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    [Header("Card Info")]
    public string cardName;
    public int ManaCost;
    public int Damage;

    [Header("Card UI Text")]
    public TMPro.TextMeshProUGUI cardNameText;
    public TMPro.TextMeshProUGUI costText;
    public TMPro.TextMeshProUGUI damageText;
    public TMPro.TextMeshProUGUI descriptionText;

    public void Start()
    {
     
        UpdateCardUI();

    }

    public void UpdateCardUI()
    {
        // Update the UI text elements with the card's information
        cardNameText.text = cardName;
        costText.text = "cost: " + ManaCost.ToString();
        damageText.text = "damage: " + Damage.ToString();
        descriptionText.text = "This is a card description.";

    }

    public void PrintCardInfo()
    {
        Debug.Log("Card Name: " + cardName);
        Debug.Log("Mana Cost: " + ManaCost);
        Debug.Log("Damage: " + Damage);
    }
}
