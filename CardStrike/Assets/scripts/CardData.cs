using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    [Header("Card Info")]
    public string cardName;
    public int ManaCost;
    public int Damage;

   public void PrintCardInfo()
    {
        Debug.Log("Card Name: " + cardName);
        Debug.Log("Mana Cost: " + ManaCost);
        Debug.Log("Damage: " + Damage);
    }
}
