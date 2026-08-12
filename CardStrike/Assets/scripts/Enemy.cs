using System.Collections;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    [Header("Managers")]
    public EnemyManager enemyManager;

    [Header("Health Settings")]
    public int maxHealth = 50;
    public int currentHealth;

    [Header("Status Effects")]
    public int poisonAmount = 0;
    public int vulnerableAmount = 0;

    [Header("Attack Settings")]
    public int attackDamage = 5;

    [Header("Attack Animation")]
    public RectTransform enemyRect;
    public float moveDistance = -80f;
    public float attackSpeed = 8f;
    public float attackRotationZ = 10f;

    private Vector2 enemyStartPosition;
    private Quaternion enemyStartRotation;

    [Header("UI")]
    public TextMeshProUGUI healthText;

    [Header("Damage Popup")]
    public GameObject damagePopupPrefab;
    public Transform damagePopupSpawnPoint;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Save starting position and rotation for attack animation
        enemyStartPosition = enemyRect.anchoredPosition;
        enemyStartRotation = enemyRect.localRotation;
    }

    public void TakeDamage(int damageAmount)
    {
        int finalDamage = damageAmount;

        // Vulnerable makes the next attack against this enemy deal 50% more damage
        if (vulnerableAmount > 0)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * 1.5f);
            vulnerableAmount--;

            Debug.Log(gameObject.name + " was Vulnerable. Damage increased to " + finalDamage);
        }

        currentHealth -= finalDamage;

        ShowDamagePopup(finalDamage);

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Debug.Log(gameObject.name + " defeated!");

            if (enemyManager != null)
            {
                enemyManager.CheckStageClear();
            }
        }
    }

    public void ApplyPoison(int amount)
    {
        poisonAmount += amount;
        Debug.Log(gameObject.name + " received " + amount + " Poison. Total Poison: " + poisonAmount);
    }

    public void ApplyVulnerable(int amount)
    {
        vulnerableAmount += amount;
        Debug.Log(gameObject.name + " received " + amount + " Vulnerable. Total Vulnerable: " + vulnerableAmount);
    }

    public void ApplyPoisonDamage()
    {
        if (poisonAmount <= 0)
        {
            return;
        }

        Debug.Log(gameObject.name + " takes " + poisonAmount + " poison damage.");

        int poisonDamage = poisonAmount;
        poisonAmount--;

        TakeDamage(poisonDamage);
    }

    public void AttackPlayer(Player player, TurnManager turnManager)
    {
        StartCoroutine(EnemyAttackAnimation(player, turnManager));
    }

    // Enemy moves forward, damages the player, then moves back
    private IEnumerator EnemyAttackAnimation(Player player, TurnManager turnManager)
    {
        Vector2 attackPosition = enemyStartPosition + new Vector2(moveDistance, 0);
        Quaternion attackRotation = Quaternion.Euler(0, 0, attackRotationZ);

        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * attackSpeed;

            enemyRect.anchoredPosition = Vector2.Lerp(enemyStartPosition, attackPosition, timer);
            enemyRect.localRotation = Quaternion.Lerp(enemyStartRotation, attackRotation, timer);

            yield return null;
        }

        player.TakeDamage(attackDamage);

        timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * attackSpeed;

            enemyRect.anchoredPosition = Vector2.Lerp(attackPosition, enemyStartPosition, timer);
            enemyRect.localRotation = Quaternion.Lerp(attackRotation, enemyStartRotation, timer);

            yield return null;
        }

        enemyRect.anchoredPosition = enemyStartPosition;
        enemyRect.localRotation = enemyStartRotation;
    }

    public void UpdateHealthUI()
    {
        healthText.text = "Enemy HP: " + currentHealth.ToString() + " / " + maxHealth.ToString();
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