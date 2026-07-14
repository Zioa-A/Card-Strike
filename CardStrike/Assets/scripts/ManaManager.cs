using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ManaManager : MonoBehaviour
{

    [Header("Mana Settings")]
    public int maxMana = 10;
    public int currentMana = 10;

    [UnitHeaderInspectable("UI")]
    public TextMeshProUGUI manaText;

    // Start is called before the first frame update
    void Start()
    {
        currentMana = maxMana;
        updateManaUI();
    }

    public bool CanUseCard(int Manacost)
    {
        return currentMana >= Manacost;
    }

    public void SpendMana(int Manacost)
    {
        if(currentMana < Manacost)
        {
            
            Debug.Log("Not enough mana to use the card.");
        }

        currentMana -= Manacost;


        if (currentMana < 0)
        {
            currentMana = 0;
        }
        updateManaUI();
    }

    public void updateManaUI()
    {
        manaText.text = currentMana.ToString() + "/" + maxMana.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
