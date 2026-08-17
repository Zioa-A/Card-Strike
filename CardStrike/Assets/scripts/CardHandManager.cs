using System.Collections;
using UnityEngine;

public class CardHandManager : MonoBehaviour
{
    [Header("Replacement Settings")]
    public float fadeOutTime = 0.3f;
    public float fadeInTime = 0.3f;

    // Called when a card has been successfully used.
    public void ReplaceCard(GameObject usedCard)
    {
        if (usedCard == null)
        {
            return;
        }

        StartCoroutine(ReplaceCardRoutine(usedCard));
    }

    private IEnumerator ReplaceCardRoutine(GameObject cardObject)
    {
        CardData cardData = cardObject.GetComponent<CardData>();

        if (cardData == null)
        {
            Debug.LogWarning("Card does not have CardData.");
            yield break;
        }

        // Gets or creates a CanvasGroup so we can fade the card.
        CanvasGroup canvasGroup = cardObject.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = cardObject.AddComponent<CanvasGroup>();
        }

        // Stops the card from being dragged while it is changing.
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;


        // -----------------------------
        // FADE OLD CARD OUT
        // -----------------------------
        float timer = 0f;

        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;

            canvasGroup.alpha =
                Mathf.Lerp(1f, 0f, timer / fadeOutTime);

            yield return null;
        }

        canvasGroup.alpha = 0f;


        // -----------------------------
        // CHOOSE NEW CARD
        // -----------------------------
        CardEffectType oldEffect = cardData.effectType;
        CardEffectType newEffect = oldEffect;

        // Keeps choosing until it gets a different card.
        // This stops a card from replacing itself immediately.
        while (newEffect == oldEffect)
        {
            int randomIndex =
                Random.Range(
                    0,
                    System.Enum.GetValues(typeof(CardEffectType)).Length
                );

            newEffect = (CardEffectType)randomIndex;
        }

        cardData.effectType = newEffect;


        // Updates the card's preset values
        // such as mana cost, damage and card name.
        cardData.ApplyCardPreset();

        // Refreshes the card visuals/text.
        cardData.UpdateCardUI();


        // -----------------------------
        // FADE NEW CARD IN
        // -----------------------------
        timer = 0f;

        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;

            canvasGroup.alpha =
                Mathf.Lerp(0f, 1f, timer / fadeInTime);

            yield return null;
        }

        canvasGroup.alpha = 1f;


        // Makes the new card usable again.
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        Debug.Log(
            "Card replaced with: " +
            cardData.cardName
        );
    }
}