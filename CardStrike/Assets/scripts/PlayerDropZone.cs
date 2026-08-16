using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

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

    [Header("Loaded Dice UI")]
    public TextMeshProUGUI diceResultText;
    public float diceRollDuration = 1f;
    public float diceNumberSpeed = 0.1f;

    [Header("Coin Toss UI")]
    public TextMeshProUGUI coinResultText;
    public float coinTossDuration = 1f;
    public float coinFlipSpeed = 0.1f;

    private Vector2 playerStartPosition;
    private Quaternion playerStartRotation;

    public void Start()
    {
        // Saves the player's starting position and rotation
        // so the player can return after attacking.
        playerStartPosition = playerRect.anchoredPosition;
        playerStartRotation = playerRect.rotation;

        // Makes sure the dice result starts hidden.
        if (diceResultText != null)
        {
            diceResultText.gameObject.SetActive(false);
        }

        // Makes sure the coin result starts hidden.
        if (coinResultText != null)
        {
            coinResultText.gameObject.SetActive(false);
        }
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

        // Stops the player from using another card during the same turn.
        if (!turnManager.CanPlayerUseCard())
        {
            Debug.Log("You already used a card this turn.");
            return;
        }

        // Checks if the player has enough mana for the card.
        if (!manaManager.CanUseCard(cardData.ManaCost))
        {
            Debug.Log("Not enough mana to use " + cardData.cardName);
            return;
        }

        manaManager.SpendMana(cardData.ManaCost);

        Debug.Log("Player used " + cardData.cardName);


        // This card affects the player instead of an enemy,
        // so it does not need to choose an enemy target.
        if (cardData.effectType == CardEffectType.ImNotStayingDown)
        {
            ApplyCardEffect(null, cardData);
            turnManager.EndPlayerTurn();
            return;
        }


        // All attacking cards choose a random alive enemy.
        Enemy enemy = enemyManager.GetRandomAliveEnemy();

        if (enemy == null)
        {
            Debug.Log("No alive enemies left.");
            return;
        }

        Debug.Log("Random enemy selected: " + enemy.gameObject.name);

        StartCoroutine(PlayerAttackAnimation(enemy, cardData));
    }


    // Moves the player towards the enemy,
    // applies the card effect, then moves back.
    private IEnumerator PlayerAttackAnimation(
        Enemy enemy,
        CardData cardData
    )
    {
        Vector2 attackPosition =
            playerStartPosition + new Vector2(moveDistance, 0);

        Quaternion attackRotation =
            Quaternion.Euler(0, 0, attackRotationZ);

        float timer = 0f;


        // Move towards the enemy.
        while (timer < 1f)
        {
            timer += Time.deltaTime * attackSpeed;

            playerRect.anchoredPosition =
                Vector2.Lerp(
                    playerStartPosition,
                    attackPosition,
                    timer
                );

            playerRect.localRotation =
                Quaternion.Lerp(
                    playerStartRotation,
                    attackRotation,
                    timer
                );

            yield return null;
        }


        // Loaded Dice waits for the dice roll animation
        // before applying damage and continuing.
        if (cardData.effectType == CardEffectType.LoadedDice)
        {
            yield return StartCoroutine(
                LoadedDiceAnimation(enemy, cardData)
            );
        }

        // Coin Toss waits for the coin flip animation
        // before applying its result and continuing.
        else if (cardData.effectType == CardEffectType.CoinToss)
        {
            yield return StartCoroutine(
                CoinTossAnimation(enemy, cardData)
            );
        }

        else
        {
            // All other cards apply their effect immediately.
            ApplyCardEffect(enemy, cardData);
        }


        timer = 0f;


        // Move back to the starting position.
        while (timer < 1f)
        {
            timer += Time.deltaTime * attackSpeed;

            playerRect.anchoredPosition =
                Vector2.Lerp(
                    attackPosition,
                    playerStartPosition,
                    timer
                );

            playerRect.localRotation =
                Quaternion.Lerp(
                    attackRotation,
                    playerStartRotation,
                    timer
                );

            yield return null;
        }


        // Makes sure the player finishes exactly
        // at the original position and rotation.
        playerRect.anchoredPosition = playerStartPosition;
        playerRect.localRotation = playerStartRotation;

        // Enemy turn only begins once the whole card animation is finished.
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

                // Loaded Dice is handled inside PlayerAttackAnimation
                // because we need to wait for the dice roll to finish.

                break;


            case CardEffectType.CoinToss:

                // Coin Toss is handled inside PlayerAttackAnimation
                // because we need to wait for the coin flip to finish.

                break;


            case CardEffectType.BreakthroughStrike:

                enemy.TakeDamage(cardData.Damage);

                // Makes the enemy take 50% extra damage
                // from the next normal attack.
                enemy.ApplyVulnerable(1);

                break;


            case CardEffectType.SilentPoison:

                enemy.TakeDamage(cardData.Damage);

                // Adds 3 Poison.
                // Poison ticks at the beginning of enemy turns.
                enemy.ApplyPoison(3);

                break;


            case CardEffectType.LifeSteal:

                enemy.TakeDamage(cardData.Damage);

                // Heals the player for 30% of the card's damage.
                if (player != null)
                {
                    int healAmount =
                        Mathf.RoundToInt(cardData.Damage * 0.3f);

                    player.Heal(healAmount);
                }

                break;


            case CardEffectType.ImNotStayingDown:

                // Player survives the next fatal enemy attack
                // and stays alive with 1 HP.
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


    // Handles the Loaded Dice visual and damage.
    private IEnumerator LoadedDiceAnimation(
        Enemy enemy,
        CardData cardData
    )
    {
        // Shows the dice UI.
        if (diceResultText != null)
        {
            diceResultText.gameObject.SetActive(true);
        }

        float timer = 0f;


        // Rapidly cycles through random numbers
        // to make the dice look like it is rolling.
        while (timer < diceRollDuration)
        {
            int rollingNumber = Random.Range(1, 7);

            if (diceResultText != null)
            {
                diceResultText.text = "🎲 " + rollingNumber;
            }

            yield return new WaitForSeconds(diceNumberSpeed);

            timer += diceNumberSpeed;
        }


        // Chooses the actual final dice result.
        int finalRoll = Random.Range(1, 7);

        if (diceResultText != null)
        {
            diceResultText.text = "🎲 " + finalRoll;
        }

        Debug.Log("Loaded Dice rolled: " + finalRoll);


        // Keeps the final number visible
        // so the player has time to read it.
        yield return new WaitForSeconds(0.5f);


        // Final damage = base card damage multiplied by dice roll.
        int diceDamage = cardData.Damage * finalRoll;

        enemy.TakeDamage(diceDamage);


        // Keeps the final result visible briefly
        // after the damage appears.
        yield return new WaitForSeconds(0.5f);


        // Hides the dice UI again.
        if (diceResultText != null)
        {
            diceResultText.gameObject.SetActive(false);
        }
    }


    // Handles the Coin Toss visual and card result.
    private IEnumerator CoinTossAnimation(
        Enemy enemy,
        CardData cardData
    )
    {
        // Shows the coin UI.
        if (coinResultText != null)
        {
            coinResultText.gameObject.SetActive(true);
        }

        float timer = 0f;


        // Rapidly switches between Heads and Tails
        // to make the coin look like it is flipping.
        while (timer < coinTossDuration)
        {
            int temporaryFlip = Random.Range(0, 2);

            if (coinResultText != null)
            {
                if (temporaryFlip == 0)
                {
                    coinResultText.text = "Heads";
                }
                else
                {
                    coinResultText.text = "Tails";
                }
            }

            yield return new WaitForSeconds(coinFlipSpeed);

            timer += coinFlipSpeed;
        }


        // Chooses the actual final result.
        int finalFlip = Random.Range(0, 2);


        // HEADS = double damage.
        if (finalFlip == 0)
        {
            if (coinResultText != null)
            {
                coinResultText.text = "Heads!";
            }

            Debug.Log("Coin Toss: Heads. Double damage.");

            // Gives the player time to see the result.
            yield return new WaitForSeconds(0.5f);

            enemy.TakeDamage(cardData.Damage * 2);
        }


        // TAILS = normal damage and 1 Vulnerable on the player.
        else
        {
            if (coinResultText != null)
            {
                coinResultText.text = "Tails!";
            }

            Debug.Log(
                "Coin Toss: Tails. Player takes 1 Vulnerable."
            );

            // Gives the player time to see the result.
            yield return new WaitForSeconds(0.5f);

            enemy.TakeDamage(cardData.Damage);

            if (player != null)
            {
                player.ApplyVulnerable(1);
            }
        }


        // Keeps the final coin result visible briefly.
        yield return new WaitForSeconds(0.5f);


        // Hides the coin UI again.
        if (coinResultText != null)
        {
            coinResultText.gameObject.SetActive(false);
        }
    }
}