using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    [Header("Card Artwork")]
    public Image cardBG;

    public Sprite lifeSteal;
    public Sprite silentpoison;
    public Sprite loadedDice;
    public Sprite iAmNotStayingDown;
    public Sprite coinToss;
    public Sprite breakthroughStrike;
    public Sprite basicAttack;

    public void Start()
    {
        ApplyCardPreset();
        UpdateCardUI();
    }

    // Updates the card automatically in the Inspector
    // when the effect type is changed.
    private void OnValidate()
    {
        ApplyCardPreset();
        UpdateCardUI();
    }

    // Sets all stats and artwork based on the card effect.
    public void ApplyCardPreset()   
    {
        switch (effectType)
        {
            case CardEffectType.BasicAttack:
                cardName = "Basic Attack";
                ManaCost = 1;
                Damage = 3;
                cardDescription = "Deal 3 damage to a random enemy.";

                if (cardBG != null)
                {
                    cardBG.sprite = basicAttack;
                }

                break;

            case CardEffectType.CoinToss:
                cardName = "Coin Toss";
                ManaCost = 2;
                Damage = 7;
                cardDescription =
                    "Deal 7 damage. Heads deals 7 extra damage. Tails makes the player Vulnerable.";

                if (cardBG != null)
                {
                    cardBG.sprite = coinToss;
                }

                break;

            case CardEffectType.LoadedDice:
                cardName = "Loaded Dice";
                ManaCost = 3;
                Damage = 3;
                cardDescription =
                    "Roll a dice. Damage equals 3 times the dice roll.";

                if (cardBG != null)
                {
                    cardBG.sprite = loadedDice;
                }

                break;

            case CardEffectType.SilentPoison:
                cardName = "Silent Poison";
                ManaCost = 2;
                Damage = 4;
                cardDescription =
                    "Deal 4 damage and apply 3 Poison.";

                if (cardBG != null)
                {
                    cardBG.sprite = silentpoison;
                }

                break;

            case CardEffectType.BreakthroughStrike:
                cardName = "Breakthrough Strike";
                ManaCost = 3;
                Damage = 6;
                cardDescription =
                    "Deal 6 damage and make the enemy Vulnerable.";

                if (cardBG != null)
                {
                    cardBG.sprite = breakthroughStrike;
                }

                break;

            case CardEffectType.ImNotStayingDown:
                cardName = "I'm Not Staying Down";
                ManaCost = 4;
                Damage = 0;
                cardDescription =
                    "Survive the next fatal enemy attack with 1 HP.";

                if (cardBG != null)
                {
                    cardBG.sprite = iAmNotStayingDown;
                }

                break;

            case CardEffectType.LifeSteal:
                cardName = "Life Steal";
                ManaCost = 3;
                Damage = 10;
                cardDescription =
                    "Deal 10 damage and heal for 30% of the damage dealt.";

                if (cardBG != null)
                {
                    cardBG.sprite = lifeSteal;
                }

                break;
        }
    }

    // Updates any text currently being shown on the card.
    // Later these can be left empty if the artwork contains everything.
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