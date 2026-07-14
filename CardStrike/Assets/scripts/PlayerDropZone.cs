using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerDropZone : MonoBehaviour, IDropHandler
{

     public void OnDrop(PointerEventData eventData)
    {
        //get the object that was dropped onto the drop zone
        GameObject droppedObject = eventData.pointerDrag;
        
        
        if(droppedObject == null)
        {
            return;
        }

        //get card data script form object if it has it its the card
        CardData cardData = droppedObject.GetComponent<CardData>();

        if(droppedObject != null)
        {
            Debug.Log("player used " + cardData.cardName);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
