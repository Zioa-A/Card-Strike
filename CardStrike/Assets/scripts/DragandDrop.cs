using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragandDrop : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    //drag varibales 
    private Vector2 originalPosition;
    private Vector3 originalScale;
    private bool isDragging = false;

    private Vector2 targetposition;
    private Vector3 targetScale;
    public float transitionSpeed = 10f;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    //rotation variables
    private Quaternion originalRotation;
    private Quaternion targetRotation;
    public float dragRotationZ = 0f;

    //drop varibales 
    private bool droppeOnPlayer;
    public GameObject playerTarget;

    [Header("Hover Settings")]
    public float hoverScale= 1.1f; 
    public float dragedScale = 1.15f; 
    public float hoverMoveAmount = 20f; 


    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;

        targetposition = originalPosition;
        targetScale=originalScale;

        originalRotation = rectTransform.localRotation;
        targetRotation = originalRotation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDragging)
        {
            targetScale = originalScale * hoverScale;
            targetposition = originalPosition + new Vector2(0, hoverMoveAmount);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging)
        {
            targetScale = originalScale;
            targetposition = originalPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if (droppeOnPlayer)
        {
            // Handle the drop on player logic here
            Debug.Log("Dropped on player!");
        }
        else
        {
            // Return to original position if not dropped on player
            rectTransform.anchoredPosition = originalPosition;
            rectTransform.localScale = originalScale;
            targetRotation = originalRotation;
        }
    }




    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        rectTransform.localScale = originalScale * dragedScale;
        targetRotation = Quaternion.Euler(0, 0, dragRotationZ);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDragging)
        {
            rectTransform.localScale = Vector3.Lerp(
                rectTransform.localScale,
                targetScale,
                transitionSpeed * Time.deltaTime
            );
        }

        rectTransform.localRotation = Quaternion.Lerp(
            rectTransform.localRotation,
            targetRotation,
            transitionSpeed * Time.deltaTime
        );
    }
}
