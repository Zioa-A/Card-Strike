using System.Collections;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI damageText;

    [Header("Animation")]
    public float moveUpDistance = 80f;
    public float lifeTime = 1f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public void Setup(int damageAmount)
    {
        if (damageText == null)
        {
            damageText = GetComponent<TextMeshProUGUI>();
        }

        damageText.text = "-" + damageAmount.ToString();

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

    public void Quit()
    {
        Application.Quit();
    }
}