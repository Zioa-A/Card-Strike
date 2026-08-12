using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerDropZone : MonoBehaviour, IDropHandler
{
    [Header("Managers")]
    public ManaManager manaManager;
    public TurnManager turnManager;
    public EnemyManager enemyManager;

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

        Enemy enemy = enemyManager.GetRandomAliveEnemy();

        if (enemy == null)
        {
            Debug.Log("No alive enemies left.");
            return;
        }

        manaManager.SpendMana(cardData.ManaCost);

        Debug.Log("Player used " + cardData.cardName);
        Debug.Log("Random enemy selected: " + enemy.gameObject.name);

        StartCoroutine(PlayerAttackAnimation(enemy, cardData));
    }

    // This handles the attack movement first, then applies the selected card effect at the hit moment
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
                    Debug.Log("Coin Toss: Tails. Normal damage for now. Player Vulnerable will be added after Player.cs update.");
                    enemy.TakeDamage(cardData.Damage);
                }

                break;

            case CardEffectType.BreakthroughStrike:
                enemy.TakeDamage(cardData.Damage);
                enemy.ApplyVulnerable(1);
                break;

            case CardEffectType.SilentPoison:
                enemy.TakeDamage(cardData.Damage);
                enemy.ApplyPoison(3);
                break;

            case CardEffectType.LifeSteal:
                Debug.Log("Life Steal heal will be added after Player.cs update. For now it deals damage only.");
                enemy.TakeDamage(cardData.Damage);
                break;

            case CardEffectType.ImNotStayingDown:
                Debug.Log("I'm Not Staying Down will be added after Player.cs update.");
                break;

            default:
                enemy.TakeDamage(cardData.Damage);
                break;
        }
    }
}