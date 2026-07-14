using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerDropZone : MonoBehaviour, IDropHandler
{
    public ManaManager manaManager;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject == null)
        {
            return;
        }

        CardData cardData = droppedObject.GetComponent<CardData>();

        if (cardData != null)
        {
            if (manaManager.CanUseCard(cardData.ManaCost))
            {
                manaManager.SpendMana(cardData.ManaCost);
                Debug.Log("Player used " + cardData.cardName);
            }
            else
            {
                Debug.Log("Not enough mana to use " + cardData.cardName);
            }
        }
    }
}