using UnityEngine;
using TMPro;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    [Header("Turn Settings")]
    public bool isPlayerTurn = true;
    public bool playerUsedCard = false;

    [Header("Enemy Turn Timing")]
    public float firstEnemyAttackDelay = 1f;
    public float delayBetweenEnemyAttacks = 0.5f;

    [Header("References")]
    public Player player;
    public EnemyManager enemyManager;
    public ManaManager manaManager;

    [Header("UI")]
    public TextMeshProUGUI TurnText;

    private bool enemyTurnSequenceRunning = false;

    void Start()
    {
        StartPlayerTurn();
    }

    public bool CanPlayerUseCard()
    {
        return isPlayerTurn && !playerUsedCard;
    }

    public void EndPlayerTurn()
    {
        playerUsedCard = true;
        isPlayerTurn = false;

        Debug.Log("Player turn ended.");

        StartCoroutine(EnemyTurnSequence());
    }

    private IEnumerator EnemyTurnSequence()
    {
        enemyTurnSequenceRunning = true;

        UpdateTurnText();

        yield return new WaitForSeconds(0.5f); // Small delay before poison ticks

        // -----------------------------
        // POISON TICKS FIRST
        // -----------------------------
        // Every alive enemy takes its poison damage
        // at the beginning of the enemy turn.
        foreach (Enemy enemy in enemyManager.enemies)
        {
            if (enemy != null && enemy.currentHealth > 0)
            {
                enemy.ApplyPoisonDamage();
            }
        }


        // -----------------------------
        // FIRST ENEMY DELAY
        // -----------------------------
      
        yield return new WaitForSeconds(firstEnemyAttackDelay);


        // -----------------------------
        // ENEMIES ATTACK ONE BY ONE
        // -----------------------------
        foreach (Enemy enemy in enemyManager.enemies)
        {
            if (enemy != null && enemy.currentHealth > 0)
            {
                enemy.AttackPlayer(player, this);

                // Wait before the next enemy attacks.
                yield return new WaitForSeconds(delayBetweenEnemyAttacks);
            }
        }


        enemyTurnSequenceRunning = false;

        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        // Stops individual enemies from restarting
        // the player turn early.
        if (enemyTurnSequenceRunning)
        {
            return;
        }

        isPlayerTurn = true;
        playerUsedCard = false;

        manaManager.RestoreMana();

        UpdateTurnText();

        //Debug.Log("Player turn started.");
    }

    void UpdateTurnText()
    {
        if (isPlayerTurn)
        {
            TurnText.text = "Your Turn";
        }
        else
        {
            TurnText.text = "Enemy Turn";
        }
    }
}