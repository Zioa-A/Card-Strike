using System.Collections;
using UnityEngine;
using TMPro;

// Different enemy types use different attack sounds.
public enum EnemyType
{
    Goblin,
    Skeleton,
    Orc
}

public class Enemy : MonoBehaviour
{
    [Header("Managers")]
    public EnemyManager enemyManager;

    [Header("Enemy Type")]
    public EnemyType enemyType;

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

    // Status effect icons.
    public GameObject poisonIcon;
    public GameObject vulnerableIcon;

    [Header("Damage Popup")]
    public GameObject damagePopupPrefab;
    public Transform damagePopupSpawnPoint;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip goblinAttackSound;
    public AudioClip skeletonAttackSound;
    public AudioClip orcAttackSound;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Save starting position and rotation for attack animation.
        enemyStartPosition = enemyRect.anchoredPosition;
        enemyStartRotation = enemyRect.localRotation;
    }

    public void TakeDamage(int damageAmount)
    {
        int finalDamage = damageAmount;
        bool vulnerableTriggered = false;

        // If the enemy is Vulnerable, the next normal attack
        // deals 50% extra damage and removes 1 Vulnerable.
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

            Debug.Log(
                gameObject.name +
                " was Vulnerable. Damage increased to " +
                finalDamage
            );
        }

        currentHealth -= finalDamage;

        // Shows normal damage or red Vulnerable damage.
        ShowDamagePopup(
            finalDamage,
            false,
            vulnerableTriggered
        );

        // Stops health from going below 0.
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        // Checks whether this damage defeated the enemy.
        CheckIfDefeated();
    }

    public void ApplyPoison(int amount)
    {
        // Adds poison instead of replacing the existing amount.
        poisonAmount += amount;

        // Shows the poison icon while the enemy is poisoned.
        if (poisonIcon != null)
        {
            poisonIcon.SetActive(true);
        }

        Debug.Log(
            gameObject.name +
            " received " +
            amount +
            " Poison. Total Poison: " +
            poisonAmount
        );
    }

    public void ApplyVulnerable(int amount)
    {
        // Adds Vulnerable stacks to the enemy.
        vulnerableAmount += amount;

        // Shows the Vulnerable icon while the effect is active.
        if (vulnerableIcon != null)
        {
            vulnerableIcon.SetActive(true);
        }

        Debug.Log(
            gameObject.name +
            " received " +
            amount +
            " Vulnerable. Total Vulnerable: " +
            vulnerableAmount
        );
    }

    public void ApplyPoisonDamage()
    {
        // If the enemy has no poison, nothing happens.
        if (poisonAmount <= 0)
        {
            return;
        }

        // The current poison amount becomes the damage for this tick.
        int poisonDamage = poisonAmount;

        Debug.Log(
            gameObject.name +
            " takes " +
            poisonDamage +
            " poison damage."
        );

        // Poison becomes weaker by 1 after every tick.
        // Example: 3 damage -> 2 damage -> 1 damage -> finished.
        poisonAmount--;

        // Hides the poison icon once the poison effect reaches 0.
        if (poisonAmount <= 0 && poisonIcon != null)
        {
            poisonIcon.SetActive(false);
        }

        // Poison damages health directly instead of using TakeDamage().
        // This prevents poison from consuming Vulnerable.
        currentHealth -= poisonDamage;

        // Shows purple poison damage.
        ShowDamagePopup(
            poisonDamage,
            true,
            false
        );

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        // Poison can still defeat an enemy,
        // so we check for death after the poison tick.
        CheckIfDefeated();
    }

    public void AttackPlayer(Player player, TurnManager turnManager)
    {
        StartCoroutine(
            EnemyAttackAnimation(
                player,
                turnManager
            )
        );
    }

    // Enemy moves forward, damages the player,
    // then moves back to its starting position.
    private IEnumerator EnemyAttackAnimation(
        Player player,
        TurnManager turnManager
    )
    {
        Vector2 attackPosition =
            enemyStartPosition +
            new Vector2(moveDistance, 0);

        Quaternion attackRotation =
            Quaternion.Euler(
                0,
                0,
                attackRotationZ
            );

        float timer = 0f;

        // Move towards the player.
        while (timer < 1f)
        {
            timer +=
                Time.deltaTime *
                attackSpeed;

            enemyRect.anchoredPosition =
                Vector2.Lerp(
                    enemyStartPosition,
                    attackPosition,
                    timer
                );

            enemyRect.localRotation =
                Quaternion.Lerp(
                    enemyStartRotation,
                    attackRotation,
                    timer
                );

            yield return null;
        }

        // Plays the attack sound based on
        // whether this enemy is a Goblin, Skeleton or Orc.
        PlayAttackSound();

        // Damage happens when the enemy reaches the player.
        player.TakeDamage(attackDamage);

        timer = 0f;

        // Move back to the original position.
        while (timer < 1f)
        {
            timer +=
                Time.deltaTime *
                attackSpeed;

            enemyRect.anchoredPosition =
                Vector2.Lerp(
                    attackPosition,
                    enemyStartPosition,
                    timer
                );

            enemyRect.localRotation =
                Quaternion.Lerp(
                    attackRotation,
                    enemyStartRotation,
                    timer
                );

            yield return null;
        }

        // Makes sure the enemy ends exactly where it started.
        enemyRect.anchoredPosition =
            enemyStartPosition;

        enemyRect.localRotation =
            enemyStartRotation;
    }

    // Plays the correct sound based on the Enemy Type dropdown.
    private void PlayAttackSound()
    {
        if (audioSource == null)
        {
            return;
        }

        switch (enemyType)
        {
            case EnemyType.Goblin:

                if (goblinAttackSound != null)
                {
                    audioSource.PlayOneShot(
                        goblinAttackSound
                    );
                }

                break;

            case EnemyType.Skeleton:

                if (skeletonAttackSound != null)
                {
                    audioSource.PlayOneShot(
                        skeletonAttackSound
                    );
                }

                break;

            case EnemyType.Orc:

                if (orcAttackSound != null)
                {
                    audioSource.PlayOneShot(
                        orcAttackSound
                    );
                }

                break;
        }
    }

    public void UpdateHealthUI()
    {
        healthText.text =
            "Enemy HP: " +
            currentHealth.ToString() +
            " / " +
            maxHealth.ToString();
    }

    // Handles enemy death and tells EnemyManager
    // to check whether every enemy has been defeated.
    private void CheckIfDefeated()
    {
        if (currentHealth <= 0)
        {
            Debug.Log(
                gameObject.name +
                " defeated!"
            );

            if (enemyManager != null)
            {
                enemyManager.CheckStageClear();
            }
        }
    }

    // Creates normal, poison or Vulnerable damage popups.
    void ShowDamagePopup(
        int damageAmount,
        bool isPoisonDamage,
        bool isVulnerableDamage
    )
    {
        if (
            damagePopupPrefab != null &&
            damagePopupSpawnPoint != null
        )
        {
            GameObject popup =
                Instantiate(
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
                    damageAmount,
                    isPoisonDamage,
                    isVulnerableDamage,
                    false
                );
            }
        }
    }
}