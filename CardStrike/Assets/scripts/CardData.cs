using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum CardEffectType
{
    BasicAttack,
    CoinToss,
    LoadedDice,
    SilentPoison,
    BreakthroughStrike,
    ImNotStayingDown,
    LifeSteal
}

public class CardData : MonoBehaviour
{
    [Header("Card Info")]
    public string cardName;
    public int ManaCost;
    public int Damage;

    [Header("Card Effect")]
    public CardEffectType effectType;
    public string cardDescription;

    [Header("Card UI Text")]
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI descriptionText;

    public void Start()
    {
        ApplyCardPreset();
        UpdateCardUI();
    }

    // This updates the card automatically in the Inspector when you change the effect type
    private void OnValidate()
    {
        ApplyCardPreset();
        UpdateCardUI();
    }

    public void ApplyCardPreset()
    {
        switch (effectType)
        {
            case CardEffectType.BasicAttack:
                cardName = "Basic Attack";
                ManaCost = 1;
                Damage = 6;
                cardDescription = "Deal 6 damage to a random enemy.";
                break;

            case CardEffectType.CoinToss:
                cardName = "Coin Toss";
                ManaCost = 2;
                Damage = 7;
                cardDescription = "Deal 7 damage. Heads deals 7 extra damage. Tails makes the player Vulnerable.";
                break;

            case CardEffectType.LoadedDice:
                cardName = "Loaded Dice";
                ManaCost = 3;
                Damage = 3;
                cardDescription = "Roll a dice. Damage equals 3 times the dice roll.";
                break;

            case CardEffectType.SilentPoison:
                cardName = "Silent Poison";
                ManaCost = 2;
                Damage = 4;
                cardDescription = "Deal 4 damage and apply 3 Poison.";
                break;

            case CardEffectType.BreakthroughStrike:
                cardName = "Breakthrough Strike";
                ManaCost = 2;
                Damage = 6;
                cardDescription = "Deal 6 damage and make the enemy Vulnerable.";
                break;

            case CardEffectType.ImNotStayingDown:
                cardName = "I'm Not Staying Down";
                ManaCost = 4;
                Damage = 0;
                cardDescription = "Survive the next fatal enemy attack with 1 HP.";
                break;

            case CardEffectType.LifeSteal:
                cardName = "Life Steal";
                ManaCost = 3;
                Damage = 10;
                cardDescription = "Deal 10 damage and heal for 30% of the damage dealt.";
                break;
        }
    }

    public void UpdateCardUI()
    {
        if (cardNameText != null)
        {
            cardNameText.text = cardName;
        }

        if (costText != null)
        {
            costText.text = "Cost: " + ManaCost.ToString();
        }

        if (damageText != null)
        {
            damageText.text = "Damage: " + Damage.ToString();
        }

        if (descriptionText != null)
        {
            descriptionText.text = cardDescription;
        }
    }

    public void PrintCardInfo()
    {
        Debug.Log("Card Name: " + cardName);
        Debug.Log("Mana Cost: " + ManaCost);
        Debug.Log("Damage: " + Damage);
        Debug.Log("Effect Type: " + effectType);
    }
}