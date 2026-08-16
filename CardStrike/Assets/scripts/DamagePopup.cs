using System.Collections;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI damageText;

    public Color normalDamageColor = Color.white;
    public Color poisonDamageColor = Color.magenta;
    public Color vulnerableDamageColor = Color.red;
    public Color healColor = Color.green;

    [Header("Animation")]
    public float moveUpDistance = 80f;
    public float lifeTime = 1.5f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    // Sets the popup number and decides which colour/type it should use.
    public void Setup(
        int amount,
        bool isPoisonDamage,
        bool isVulnerableDamage,
        bool isHealing
    )
    {
        if (damageText == null)
        {
            damageText = GetComponent<TextMeshProUGUI>();
        }

        // Healing shows a + symbol instead of -
        if (isHealing)
        {
            damageText.text = "+" + amount.ToString();
            damageText.color = healColor;
        }

        // Poison damage appears purple.
        else if (isPoisonDamage)
        {
            damageText.text = "-" + amount.ToString();
            damageText.color = poisonDamageColor;
        }

        // Damage increased by Vulnerable appears red.
        else if (isVulnerableDamage)
        {
            damageText.text = "-" + amount.ToString();
            damageText.color = vulnerableDamageColor;
        }

        // Normal damage uses the normal damage colour.
        else
        {
            damageText.text = "-" + amount.ToString();
            damageText.color = normalDamageColor;
        }

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        StartCoroutine(AnimatePopup());
    }

    private IEnumerator AnimatePopup()
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 endPosition =
            startPosition + new Vector2(0, moveUpDistance);

        float timer = 0f;

        while (timer < lifeTime)
        {
            timer += Time.deltaTime;

            float progress = timer / lifeTime;

            rectTransform.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    endPosition,
                    progress
                );

            canvasGroup.alpha =
                Mathf.Lerp(1f, 0f, progress);

            yield return null;
        }

        Destroy(gameObject);
    }
}