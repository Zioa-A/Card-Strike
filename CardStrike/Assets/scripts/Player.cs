using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    public int currentHealth;

    [Header("UI")]
    public TextMeshProUGUI healthText;

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
            Debug.Log("Player defeated!");
        }
    }

    void UpdateHealthUI()
    {
       
        healthText.text = "Player HP: " + currentHealth + " / " + maxHealth;
    }
}