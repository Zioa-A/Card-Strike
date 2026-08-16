using System.Collections;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI damageText;
    public Color normalDamageColor = Color.white;
    public Color poisonDamageColor = Color.magenta;

    [Header("Animation")]
    public float moveUpDistance = 80f;
    public float lifeTime = 1.5f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public void Setup(int damageAmount, bool isPoisonDamage)
    {
        if (damageText == null)
        {
            damageText = GetComponent<TextMeshProUGUI>();
        }

        damageText.text = "-" + damageAmount.ToString();
        if (isPoisonDamage)
        {
            damageText.color = poisonDamageColor;
        }
        else
        {
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
        Vector2 endPosition = startPosition + new Vector2(0, moveUpDistance);

        float timer = 0f;

        while (timer < lifeTime)
        {
            timer += Time.deltaTime;

            float progress = timer / lifeTime;

            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, progress);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);

            yield return null;
        }

        Destroy(gameObject);
    }

   
}