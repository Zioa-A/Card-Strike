using UnityEngine;
using TMPro;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    [Header("Turn Settings")]
    public bool isPlayerTurn = true;
    public bool playerUsedCard = false;

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

        //Debug.Log("Enemy turn started.");

        UpdateTurnText();

        // Each alive enemy attacks one by one
        foreach (Enemy enemy in enemyManager.enemies)
        {
            if (enemy != null && enemy.currentHealth > 0)
            {
                enemy.AttackPlayer(player, this);

                // Wait so enemies do not all attack at the exact same time
                yield return new WaitForSeconds(1.2f);
            }
        }

        enemyTurnSequenceRunning = false;

        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        // Stops individual enemies from restarting the player turn early
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