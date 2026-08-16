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

    // Temporary icon for the Vulnerable status effect.
    // Later this can be replaced with the final artist icon.
    public GameObject vulnerableIcon;

    [Header("Damage Popup")]
    public GameObject damagePopupPrefab;
    public Transform damagePopupSpawnPoint;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Makes sure the Vulnerable icon starts hidden.
        if (vulnerableIcon != null)
        {
            vulnerableIcon.SetActive(false);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        int finalDamage = damageAmount;

        bool vulnerableTriggered = false;

        // Vulnerable makes the next enemy attack deal 50% more damage to the player
        if (vulnerableAmount > 0)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * 1.5f);
            vulnerableAmount--;

            vulnerableTriggered = true;

            // Hides the Vulnerable icon once the effect has been used up.
            if (vulnerableAmount <= 0 && vulnerableIcon != null)
            {
                vulnerableIcon.SetActive(false);
            }

            Debug.Log("Player was Vulnerable. Damage increased to " + finalDamage);
        }

        // If this hit would defeat the player, survival protection keeps them at 1 HP once
        if (surviveNextFatalHit && currentHealth - finalDamage <= 0)
        {
            currentHealth = 1;
            surviveNextFatalHit = false;

            UpdateHealthUI();

            ShowPopup(
                finalDamage,
                false,
                vulnerableTriggered,
                false
            );

            Debug.Log("I'm Not Staying Down activated. Player survived with 1 HP.");

            return;
        }

        currentHealth -= finalDamage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        // Shows normal or Vulnerable damage depending on the hit.
        ShowPopup(
            finalDamage,
            false,
            vulnerableTriggered,
            false
        );

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
        // Stores health before healing so we can calculate
        // the amount that was actually restored.
        int healthBeforeHealing = currentHealth;

        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Calculates the real amount healed.
        // Example: 49 HP + 3 healing only restores 1 HP.
        int actualHealAmount =
            currentHealth - healthBeforeHealing;

        UpdateHealthUI();

        // Only shows a popup if health was actually restored.
        if (actualHealAmount > 0)
        {
            ShowPopup(
                actualHealAmount,
                false,
                false,
                true
            );
        }

        Debug.Log("Player healed for " + actualHealAmount);
    }

    public void ApplyVulnerable(int amount)
    {
        vulnerableAmount += amount;

        // Shows the Vulnerable icon while the effect is active.
        if (vulnerableIcon != null)
        {
            vulnerableIcon.SetActive(true);
        }

        Debug.Log(
            "Player received " +
            amount +
            " Vulnerable. Total Vulnerable: " +
            vulnerableAmount
        );
    }

    public void ActivateSurvivalProtection()
    {
        surviveNextFatalHit = true;

        Debug.Log(
            "I'm Not Staying Down activated. Player will survive the next fatal enemy attack."
        );
    }

    void UpdateHealthUI()
    {
        healthText.text =
            "Player HP: " +
            currentHealth +
            " / " +
            maxHealth;
    }

    // Creates normal damage, Vulnerable damage,
    // poison damage or healing popups.
    void ShowPopup(
        int amount,
        bool isPoisonDamage,
        bool isVulnerableDamage,
        bool isHealing
    )
    {
        if (
            damagePopupPrefab != null &&
            damagePopupSpawnPoint != null
        )
        {
            GameObject popup = Instantiate(
                damagePopupPrefab,
                damagePopupSpawnPoint.position,
                Quaternion.identity,
                damagePopupSpawnPoint.parent
            );

            DamagePopup damagePopup =
                popup.GetComponent<DamagePopup>();

            if (damagePopup != null)
            {
                damagePopup.Setup(
                    amount,
                    isPoisonDamage,
                    isVulnerableDamage,
                    isHealing
                );
            }
        }
    }
}