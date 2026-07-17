using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("Turn Settings")]
    public bool isPlayerTurn = true;
    public bool playerUsedCard = false;

    [Header("References")]
    public Player player;
    public Enemy enemy;
    public ManaManager manaManager;

    void Start()
    {
        StartPlayerTurn();
    }

    public bool CanPlayerUseCard()
    {
        // Player can only use a card if it is their turn
        // and they have not already used a card this turn
        return isPlayerTurn && !playerUsedCard;
    }

    public void EndPlayerTurn()
    {
        playerUsedCard = true;
        isPlayerTurn = false;

        Debug.Log("Player turn ended.");

        EnemyTurn();
    }

    void EnemyTurn()
    {
        Debug.Log("Enemy turn started.");

        // Enemy handles its own attack animation and damage
        enemy.AttackPlayer(player,this);
    }

    public void StartPlayerTurn()
    {
        isPlayerTurn = true;
        playerUsedCard = false;

        // Refill mana at the start of the player's turn
        manaManager.RestoreMana();

        Debug.Log("Player turn started.");
    }
}