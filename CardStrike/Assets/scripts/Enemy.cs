using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    public int currentHealth;

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

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Saves the enemy's starting position and rotation so it can return after attacking
        enemyStartPosition = enemyRect.anchoredPosition;
        enemyStartRotation = enemyRect.localRotation;
    }

    public void TakeDamage(int damageAmount)
    {
        // Remove health based on card damage
        currentHealth -= damageAmount;

        // Stop health from going below 0
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // Update health text after taking damage
        UpdateHealthUI();

        // Check if enemy is dead
        if (currentHealth <= 0)
        {
            Debug.Log("Enemy defeated!");
        }
    }

    public void AttackPlayer(Player player, TurnManager turnManager)
    {
        StartCoroutine(EnemyAttackAnimation(player, turnManager));
    }

    // This coroutine creates a simple enemy attack animation:
    // the enemy lunges forward, rotates slightly, damages the player, then returns back to normal.
    private IEnumerator EnemyAttackAnimation(Player player, TurnManager turnManager)
    {
        Vector2 attackPosition = enemyStartPosition + new Vector2(moveDistance, 0);
        Quaternion attackRotation = Quaternion.Euler(0, 0, attackRotationZ);

        float timer = 0f;

        // Move toward player
        while (timer < 1f)
        {
            timer += Time.deltaTime * attackSpeed;

            enemyRect.anchoredPosition = Vector2.Lerp(enemyStartPosition, attackPosition, timer);
            enemyRect.localRotation = Quaternion.Lerp(enemyStartRotation, attackRotation, timer);

            yield return null;
        }

        // Damage player at the hit moment
        player.TakeDamage(attackDamage);

        timer = 0f;

        // Move back to original position
        while (timer < 1f)
        {
            timer += Time.deltaTime * attackSpeed;

            enemyRect.anchoredPosition = Vector2.Lerp(attackPosition, enemyStartPosition, timer);
            enemyRect.localRotation = Quaternion.Lerp(attackRotation, enemyStartRotation, timer);

            yield return null;
        }

        // Make sure enemy ends exactly where it started
        enemyRect.anchoredPosition = enemyStartPosition;
        enemyRect.localRotation = enemyStartRotation;

        turnManager.StartPlayerTurn();
    }

    public void UpdateHealthUI()
    {
        healthText.text = "Enemy HP: " + currentHealth.ToString() + " / " + maxHealth.ToString();
    }
}