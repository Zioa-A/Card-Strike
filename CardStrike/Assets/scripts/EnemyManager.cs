using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemies in Battle")]
    public List<Enemy> enemies = new List<Enemy>();

    [Header("References")]
    public GameManager gameManager;

    public Enemy GetRandomAliveEnemy()
    {
        List<Enemy> aliveEnemies = new List<Enemy>();

        // Make a temporary list of enemies that are still alive
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.currentHealth > 0)
            {
                aliveEnemies.Add(enemy);
            }
        }

        // If no enemies are alive, return nothing
        if (aliveEnemies.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, aliveEnemies.Count);
        return aliveEnemies[randomIndex];
    }

    public bool AreAllEnemiesDead()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.currentHealth > 0)
            {
                return false;
            }
        }

        return true;
    }

    public void CheckStageClear()
    {
        // Only clear the stage when every enemy is defeated
        if (AreAllEnemiesDead())
        {
            if (gameManager != null)
            {
                gameManager.StageCleared();
            }
        }
    }
}