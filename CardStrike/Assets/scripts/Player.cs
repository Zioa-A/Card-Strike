using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Managers")]
    public GameManager gameManager;

    [Header("Health Settings")]
    public int maxHealth = 50;
    public int currentHealth;

    [Header("Status Effects")]
    public int vulnerableAmount = 0;
    public bool surviveNextFatalHit = false;

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
        int finalDamage = damageAmount;

        // Vulnerable makes the next enemy attack deal 50% more damage to the player
        if (vulnerableAmount > 0)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * 1.5f);
            vulnerableAmount--;

            Debug.Log("Player was Vulnerable. Damage increased to " + finalDamage);
        }

        // If this hit would defeat the player, survival protection keeps them at 1 HP once
        if (surviveNextFatalHit && currentHealth - finalDamage <= 0)
        {
            currentHealth = 1;
            surviveNextFatalHit = false;

            UpdateHealthUI();
            ShowDamagePopup(finalDamage);

            Debug.Log("I'm Not Staying Down activated. Player survived with 1 HP.");
            return;
        }

        currentHealth -= finalDamage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();
        ShowDamagePopup(finalDamage);

        if (currentHealth <= 0)
        {
            if (gameManager != null)
            {
                gameManager.PlayerLoses();
            }

            Debug.Log("Player defeated!");
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();

        Debug.Log("Player healed for " + healAmount);
    }

    public void ApplyVulnerable(int amount)
    {
        vulnerableAmount += amount;

        Debug.Log("Player received " + amount + " Vulnerable. Total Vulnerable: " + vulnerableAmount);
    }

    public void ActivateSurvivalProtection()
    {
        surviveNextFatalHit = true;

        Debug.Log("I'm Not Staying Down activated. Player will survive the next fatal enemy attack.");
    }

    void UpdateHealthUI()
    {
        healthText.text = "Player HP: " + currentHealth + " / " + maxHealth;
    }

    void ShowDamagePopup(int damageAmount)
    {
        if (damagePopupPrefab != null && damagePopupSpawnPoint != null)
        {
            GameObject popup = Instantiate(
                damagePopupPrefab,
                damagePopupSpawnPoint.position,
                Quaternion.identity,
                damagePopupSpawnPoint.parent
            );

            DamagePopup damagePopup = popup.GetComponent<DamagePopup>();

            if (damagePopup != null)
            {
                damagePopup.Setup(damageAmount);
            }
        }
    }
}