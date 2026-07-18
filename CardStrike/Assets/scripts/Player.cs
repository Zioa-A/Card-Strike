using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
   public GameManager gameManager;

    [Header("Health Settings")]
    public int maxHealth = 50;
    public int currentHealth;

    [Header("UI")]
    public TextMeshProUGUI healthText;


    [Header("Damage Popup")]
    public GameObject damagePopupPrefab;
    public Transform damagePopupSpawnPoint;

    void Start()
    {
        currentHealth = maxHealth;

       
        UpdateHealthUI();
    }

    public void TakeDamage(int damageAmount)
    {
       
        currentHealth -= damageAmount;

        // Stop health from going below 0
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // Update the health text after taking damage
        UpdateHealthUI();

        // Check if the player is dead
        if (currentHealth <= 0)
        {
            gameManager.PlayerLoses();
            Debug.Log("Player defeated!");
        }

        ShowDamagePopup(damageAmount);
    }

    void UpdateHealthUI()
    {
       
        healthText.text = "Player HP: " + currentHealth + " / " + maxHealth;
    }

    void ShowDamagePopup(int damageAmount)
    {
        if (damagePopupPrefab != null && damagePopupSpawnPoint != null)
        {
            GameObject popup = Instantiate(damagePopupPrefab, damagePopupSpawnPoint.position, Quaternion.identity, damagePopupSpawnPoint.parent);

            DamagePopup damagePopup = popup.GetComponent<DamagePopup>();

            if (damagePopup != null)
            {
                damagePopup.Setup(damageAmount);
            }
        }
    }
}