using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerDropZone : MonoBehaviour, IDropHandler
{
    [Header("Managers")]
    public ManaManager manaManager;
    public TurnManager turnManager;
    public EnemyManager enemyManager;
    public Player player;

    [Header("Attack Animation")]
    public RectTransform playerRect;
    public float moveDistance = 80f;
    public float attackSpeed = 8f;
    public float attackRotationZ = -10f;

    private Vector2 playerStartPosition;
    private Quaternion playerStartRotation;

    public void Start()
    {
        playerStartPosition = playerRect.anchoredPosition;
        playerStartRotation = playerRect.rotation;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject == null)
        {
            return;
        }

        CardData cardData = droppedObject.GetComponent<CardData>();

        if (cardData == null)
        {
            return;
        }

        if (!turnManager.CanPlayerUseCard())
        {
            Debug.Log("You already used a card this turn.");
            return;
        }

        if (!manaManager.CanUseCard(cardData.ManaCost))
        {
            Debug.Log("Not enough mana to use " + cardData.cardName);
            return;
        }

        manaManager.SpendMana(cardData.ManaCost);

        Debug.Log("Player used " + cardData.cardName);

        // Survival card affects the player, so it does not need to choose an enemy
        if (cardData.effectType == CardEffectType.ImNotStayingDown)
        {
            ApplyCardEffect(null, cardData);
            turnManager.EndPlayerTurn();
            return;
        }

        // All attack cards choose a random alive enemy
        Enemy enemy = enemyManager.GetRandomAliveEnemy();

        if (enemy == null)
        {
            Debug.Log("No alive enemies left.");
            return;
        }

        Debug.Log("Random enemy selected: " + enemy.gameObject.name);

        StartCoroutine(PlayerAttackAnimation(enemy, cardData));
    }

    // Moves the player forward, applies the card effect at the hit moment, then moves back
    private IEnumerator PlayerAttackAnimation(Enemy enemy, CardData cardData)
    {
        Vector2 attackPosition = playerStartPosition + new Vector2(moveDistance, 0);
        Quaternion attackRotation = Quaternion.Euler(0, 0, attackRotationZ);

        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * attackSpeed;

            playerRect.anchoredPosition = Vector2.Lerp(playerStartPosition, attackPosition, timer);
            playerRect.localRotation = Quaternion.Lerp(playerStartRotation, attackRotation, timer);

            yield return null;
        }

        // Card effect happens here, when the player reaches the enemy
        ApplyCardEffect(enemy, cardData);

        timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * attackSpeed;

            playerRect.anchoredPosition = Vector2.Lerp(attackPosition, playerStartPosition, timer);
            playerRect.localRotation = Quaternion.Lerp(attackRotation, playerStartRotation, timer);

            yield return null;
        }

        playerRect.anchoredPosition = playerStartPosition;
        playerRect.localRotation = playerStartRotation;

        turnManager.EndPlayerTurn();
    }

    void ApplyCardEffect(Enemy enemy, CardData cardData)
    {
        switch (cardData.effectType)
        {
            case CardEffectType.BasicAttack:
                enemy.TakeDamage(cardData.Damage);
                break;

            case CardEffectType.LoadedDice:
                int diceRoll = Random.Range(1, 7);
                int diceDamage = cardData.Damage * diceRoll;

                Debug.Log("Loaded Dice rolled: " + diceRoll);
                enemy.TakeDamage(diceDamage);
                break;

            case CardEffectType.CoinToss:
                int coinFlip = Random.Range(0, 2);

                if (coinFlip == 0)
                {
                    Debug.Log("Coin Toss: Heads. Double damage.");
                    enemy.TakeDamage(cardData.Damage * 2);
                }
                else
                {
                    Debug.Log("Coin Toss: Tails. Player takes 1 Vulnerable.");
                    enemy.TakeDamage(cardData.Damage);

                    // Tails makes the player take extra damage from the next enemy attack
                    if (player != null)
                    {
                        player.ApplyVulnerable(1);
                    }
                }

                break;

            case CardEffectType.BreakthroughStrike:
                enemy.TakeDamage(cardData.Damage);

                // Enemy takes extra damage from the next attack
                enemy.ApplyVulnerable(1);
                break;

            case CardEffectType.SilentPoison:
                enemy.TakeDamage(cardData.Damage);

                // Poison will damage the enemy at the start of enemy turns
                enemy.ApplyPoison(3);
                break;

            case CardEffectType.LifeSteal:
                enemy.TakeDamage(cardData.Damage);

                // Heal is 30% of the card's damage value
                if (player != null)
                {
                    int healAmount = Mathf.RoundToInt(cardData.Damage * 0.3f);
                    player.Heal(healAmount);
                }

                break;

            case CardEffectType.ImNotStayingDown:
                // Player survives the next fatal enemy attack with 1 HP
                if (player != null)
                {
                    player.ActivateSurvivalProtection();
                }

                break;

            default:
                enemy.TakeDamage(cardData.Damage);
                break;
        }
    }
}