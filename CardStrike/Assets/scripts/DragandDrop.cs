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
    // Drag variables
    private Vector2 originalPosition;
    private Vector3 originalScale;
    private bool isDragging = false;

    private Vector2 targetposition;
    private Vector3 targetScale;
    public float transitionSpeed = 10f;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    // Rotation variables
    private Quaternion originalRotation;
    private Quaternion targetRotation;
    public float dragRotationZ = 0f;

    // Drop variables
    private bool droppeOnPlayer;
    public GameObject playerTarget;

    [Header("Hover Settings")]
    public float hoverScale = 1.1f;
    public float dragedScale = 1.15f;
    public float hoverMoveAmount = 20f;

    [Header("Player Turn Position")]
    public TurnManager turnManager;

    // How far all cards move up during the player's turn.
    public float playerTurnMoveAmount = 40f;

    private bool isHovering = false;

    [Header("Tooltip")]
    public CardTooltip cardTooltip;

    private CardData cardData;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        // Gets this card's data for the tooltip.
        cardData = GetComponent<CardData>();

        // Finds TurnManager automatically if it has not
        // been assigned manually in the Inspector.
        if (turnManager == null)
        {
            turnManager = FindFirstObjectByType<TurnManager>();
        }

        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;

        targetposition = originalPosition;
        targetScale = originalScale;

        originalRotation = rectTransform.localRotation;
        targetRotation = originalRotation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        if (!isDragging)
        {
            targetScale = originalScale * hoverScale;

            // Shows this card's information in the tooltip.
            if (cardTooltip != null && cardData != null)
            {
                cardTooltip.ShowTooltip(cardData);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        if (!isDragging)
        {
            targetScale = originalScale;

            // Hides the tooltip when the mouse leaves the card.
            if (cardTooltip != null)
            {
                cardTooltip.HideTooltip();
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (droppeOnPlayer)
        {
            // Handle the drop on player logic here.
            Debug.Log("Dropped on player!");
        }
        else
        {
            // Returns the card back into the hand.
            rectTransform.localScale = originalScale;
            targetRotation = originalRotation;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        // Hides the tooltip when dragging starts.
        if (cardTooltip != null)
        {
            cardTooltip.HideTooltip();
        }

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        rectTransform.localScale =
            originalScale * dragedScale;

        targetRotation =
            Quaternion.Euler(0, 0, dragRotationZ);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDragging)
        {
            Vector2 desiredPosition = originalPosition;

            // Raises every card while it is the player's turn.
            if (turnManager != null && turnManager.isPlayerTurn)
            {
                desiredPosition +=
                    new Vector2(0, playerTurnMoveAmount);
            }

            // Raises the hovered card even higher.
            if (isHovering)
            {
                desiredPosition +=
                    new Vector2(0, hoverMoveAmount);
            }

            targetposition = desiredPosition;

            // Smooth card scaling.
            rectTransform.localScale = Vector3.Lerp(
                rectTransform.localScale,
                targetScale,
                transitionSpeed * Time.deltaTime
            );

            // Smooth card movement.
            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                targetposition,
                transitionSpeed * Time.deltaTime
            );
        }

        // Smooth card rotation.
        rectTransform.localRotation = Quaternion.Lerp(
            rectTransform.localRotation,
            targetRotation,
            transitionSpeed * Time.deltaTime
        );
    }
}