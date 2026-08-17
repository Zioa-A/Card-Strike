using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class PlayerDropZone : MonoBehaviour, IDropHandler
{
    [Header("Managers")]
    public ManaManager manaManager;
    public TurnManager turnManager;
    public EnemyManager enemyManager;
    public Player player;
    public CardHandManager cardHandManager;

    [Header("Attack Animation")]
    public RectTransform playerRect;
    public float moveDistance = 80f;
    public float attackSpeed = 8f;
    public float attackRotationZ = -10f;


    // -----------------------------
    // LOADED DICE
    // -----------------------------

    [Header("Loaded Dice UI")]
    public Image diceImage;

    public float diceRollDuration = 1f;
    public float diceNumberSpeed = 0.1f;

    [Header("Dice Sprites")]
    public Sprite dice1Sprite;
    public Sprite dice2Sprite;
    public Sprite dice3Sprite;
    public Sprite dice4Sprite;
    public Sprite dice5Sprite;
    public Sprite dice6Sprite;


    // -----------------------------
    // COIN TOSS
    // -----------------------------

    [Header("Coin Toss UI")]
    public TextMeshProUGUI coinResultText;
    public Image coinImage;

    public Sprite headsSprite;
    public Sprite tailsSprite;

    public float coinTossDuration = 1f;
    public float coinFlipSpeed = 0.1f;

    [Header("Coin Toss Animation")]
    public RectTransform coinRect;
    public float coinMoveUpDistance = 120f;
    public float coinRotationSpeed = 720f;


    private Vector2 playerStartPosition;
    private Quaternion playerStartRotation;

    private Vector2 coinStartPosition;
    private Quaternion coinStartRotation;


    public void Start()
    {
        // Saves the player's starting position and rotation
        // so the player can return after attacking.
        playerStartPosition = playerRect.anchoredPosition;
        playerStartRotation = playerRect.rotation;


        // Saves the coin's starting position and rotation.
        if (coinRect != null)
        {
            coinStartPosition = coinRect.anchoredPosition;
            coinStartRotation = coinRect.localRotation;
        }


        // Makes sure the dice image starts hidden.
        if (diceImage != null)
        {
            diceImage.gameObject.SetActive(false);
        }


        // Makes sure the coin text starts hidden.
        if (coinResultText != null)
        {
            coinResultText.gameObject.SetActive(false);
        }

        // Makes sure the coin image starts hidden.
        if (coinImage != null)
        {
            coinImage.gameObject.SetActive(false);
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


        // This card affects the player instead of an enemy.
        if (cardData.effectType == CardEffectType.ImNotStayingDown)
        {
            ApplyCardEffect(null, cardData);

            if (cardHandManager != null)
            {
                cardHandManager.ReplaceCard(cardData.gameObject);
            }

            turnManager.EndPlayerTurn();

            return;
        }


        // All attack cards choose a random alive enemy.
        Enemy enemy = enemyManager.GetRandomAliveEnemy();

        if (enemy == null)
        {
            Debug.Log("No alive enemies left.");
            return;
        }


        Debug.Log(
            "Random enemy selected: " +
            enemy.gameObject.name
        );


        StartCoroutine(
            PlayerAttackAnimation(
                enemy,
                cardData
            )
        );
    }


    // Moves the player towards the enemy,
    // applies the effect, then returns the player.
    private IEnumerator PlayerAttackAnimation(
        Enemy enemy,
        CardData cardData
    )
    {
        Vector2 attackPosition =
            playerStartPosition +
            new Vector2(moveDistance, 0);

        Quaternion attackRotation =
            Quaternion.Euler(
                0,
                0,
                attackRotationZ
            );


        float timer = 0f;


        // Move towards the enemy.
        while (timer < 1f)
        {
            timer +=
                Time.deltaTime *
                attackSpeed;


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


        // Loaded Dice waits for the dice animation.
        if (
            cardData.effectType ==
            CardEffectType.LoadedDice
        )
        {
            yield return StartCoroutine(
                LoadedDiceAnimation(
                    enemy,
                    cardData
                )
            );
        }


        // Coin Toss waits for the coin animation.
        else if (
            cardData.effectType ==
            CardEffectType.CoinToss
        )
        {
            yield return StartCoroutine(
                CoinTossAnimation(
                    enemy,
                    cardData
                )
            );
        }


        else
        {
            ApplyCardEffect(
                enemy,
                cardData
            );
        }


        timer = 0f;


        // Move back to the starting position.
        while (timer < 1f)
        {
            timer +=
                Time.deltaTime *
                attackSpeed;


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


        playerRect.anchoredPosition =
            playerStartPosition;

        playerRect.localRotation =
            playerStartRotation;


        // Replaces the used card.
        if (cardHandManager != null)
        {
            cardHandManager.ReplaceCard(
                cardData.gameObject
            );
        }


        // Starts the enemy turn after everything finishes.
        turnManager.EndPlayerTurn();
    }


    void ApplyCardEffect(
        Enemy enemy,
        CardData cardData
    )
    {
        switch (cardData.effectType)
        {
            case CardEffectType.BasicAttack:

                enemy.TakeDamage(
                    cardData.Damage
                );

                break;


            case CardEffectType.LoadedDice:

                // Handled inside PlayerAttackAnimation.

                break;


            case CardEffectType.CoinToss:

                // Handled inside PlayerAttackAnimation.

                break;


            case CardEffectType.BreakthroughStrike:

                enemy.TakeDamage(
                    cardData.Damage
                );

                // Makes the enemy Vulnerable.
                enemy.ApplyVulnerable(1);

                break;


            case CardEffectType.SilentPoison:

                enemy.TakeDamage(
                    cardData.Damage
                );

                // Adds 3 Poison.
                enemy.ApplyPoison(3);

                break;


            case CardEffectType.LifeSteal:

                enemy.TakeDamage(
                    cardData.Damage
                );


                // Heals the player for 30%
                // of the card damage.
                if (player != null)
                {
                    int healAmount =
                        Mathf.RoundToInt(
                            cardData.Damage *
                            0.3f
                        );

                    player.Heal(
                        healAmount
                    );
                }

                break;


            case CardEffectType.ImNotStayingDown:

                if (player != null)
                {
                    player.ActivateSurvivalProtection();
                }

                break;


            default:

                enemy.TakeDamage(
                    cardData.Damage
                );

                break;
        }
    }


    // -----------------------------
    // LOADED DICE ANIMATION
    // -----------------------------

    private IEnumerator LoadedDiceAnimation(
        Enemy enemy,
        CardData cardData
    )
    {
        // Shows the dice image.
        if (diceImage != null)
        {
            diceImage.gameObject.SetActive(true);
        }


        float timer = 0f;


        // Rapidly cycles through dice faces.
        while (timer < diceRollDuration)
        {
            int rollingNumber =
                Random.Range(1, 7);


            // Changes the dice image
            // to match the rolling number.
            SetDiceSprite(
                rollingNumber
            );


            yield return new WaitForSeconds(
                diceNumberSpeed
            );


            timer +=
                diceNumberSpeed;
        }


        // Chooses the final dice result.
        int finalRoll =
            Random.Range(1, 7);


        // Makes sure the final dice image
        // matches the final result.
        SetDiceSprite(
            finalRoll
        );


        Debug.Log(
            "Loaded Dice rolled: " +
            finalRoll
        );


        // Leaves the final dice visible briefly.
        yield return new WaitForSeconds(
            0.5f
        );


        // Final damage =
        // card damage multiplied by dice result.
        int diceDamage =
            cardData.Damage *
            finalRoll;


        enemy.TakeDamage(
            diceDamage
        );


        yield return new WaitForSeconds(
            0.5f
        );


        // Hides the dice image.
        if (diceImage != null)
        {
            diceImage.gameObject.SetActive(false);
        }
    }


    // Changes the dice image based
    // on the number that was rolled.
    private void SetDiceSprite(
        int diceNumber
    )
    {
        if (diceImage == null)
        {
            return;
        }


        switch (diceNumber)
        {
            case 1:

                diceImage.sprite =
                    dice1Sprite;

                break;


            case 2:

                diceImage.sprite =
                    dice2Sprite;

                break;


            case 3:

                diceImage.sprite =
                    dice3Sprite;

                break;


            case 4:

                diceImage.sprite =
                    dice4Sprite;

                break;


            case 5:

                diceImage.sprite =
                    dice5Sprite;

                break;


            case 6:

                diceImage.sprite =
                    dice6Sprite;

                break;
        }
    }


    // -----------------------------
    // COIN TOSS ANIMATION
    // -----------------------------

    private IEnumerator CoinTossAnimation(
        Enemy enemy,
        CardData cardData
    )
    {
        // Shows the coin text.
        if (coinResultText != null)
        {
            coinResultText.gameObject.SetActive(true);
        }


        // Shows the coin image.
        if (coinImage != null)
        {
            coinImage.gameObject.SetActive(true);
        }


        // Resets the coin before each toss.
        if (coinRect != null)
        {
            coinRect.anchoredPosition =
                coinStartPosition;

            coinRect.localRotation =
                coinStartRotation;
        }


        float timer = 0f;
        float flipTimer = 0f;


        // Coin moves upward, spins,
        // and switches between Heads and Tails.
        while (timer < coinTossDuration)
        {
            timer += Time.deltaTime;
            flipTimer += Time.deltaTime;


            float progress =
                timer /
                coinTossDuration;


            // Creates an up-and-down arc.
            float height =
                Mathf.Sin(
                    progress *
                    Mathf.PI
                ) *
                coinMoveUpDistance;


            if (coinRect != null)
            {
                coinRect.anchoredPosition =
                    coinStartPosition +
                    new Vector2(
                        0,
                        height
                    );


                coinRect.Rotate(
                    0f,
                    0f,
                    coinRotationSpeed *
                    Time.deltaTime
                );
            }


            // Changes Heads/Tails
            // at the selected flip speed.
            if (flipTimer >= coinFlipSpeed)
            {
                flipTimer = 0f;


                int temporaryFlip =
                    Random.Range(
                        0,
                        2
                    );


                if (temporaryFlip == 0)
                {
                    if (coinResultText != null)
                    {
                        coinResultText.text =
                            "Heads";
                    }


                    if (coinImage != null)
                    {
                        coinImage.sprite =
                            headsSprite;
                    }
                }


                else
                {
                    if (coinResultText != null)
                    {
                        coinResultText.text =
                            "Tails";
                    }


                    if (coinImage != null)
                    {
                        coinImage.sprite =
                            tailsSprite;
                    }
                }
            }


            yield return null;
        }


        // Returns the coin to its starting position.
        if (coinRect != null)
        {
            coinRect.anchoredPosition =
                coinStartPosition;

            coinRect.localRotation =
                coinStartRotation;
        }


        // Chooses the final result.
        int finalFlip =
            Random.Range(
                0,
                2
            );


        // HEADS
        if (finalFlip == 0)
        {
            if (coinResultText != null)
            {
                coinResultText.text =
                    "Heads!";
            }


            if (coinImage != null)
            {
                coinImage.sprite =
                    headsSprite;
            }


            Debug.Log(
                "Coin Toss: Heads. Double damage."
            );


            yield return new WaitForSeconds(
                0.5f
            );


            enemy.TakeDamage(
                cardData.Damage *
                2
            );
        }


        // TAILS
        else
        {
            if (coinResultText != null)
            {
                coinResultText.text =
                    "Tails!";
            }


            if (coinImage != null)
            {
                coinImage.sprite =
                    tailsSprite;
            }


            Debug.Log(
                "Coin Toss: Tails. Player takes 1 Vulnerable."
            );


            yield return new WaitForSeconds(
                0.5f
            );


            enemy.TakeDamage(
                cardData.Damage
            );


            if (player != null)
            {
                player.ApplyVulnerable(1);
            }
        }


        // Keeps the final coin visible briefly.
        yield return new WaitForSeconds(
            0.5f
        );


        // Hides the coin text.
        if (coinResultText != null)
        {
            coinResultText.gameObject.SetActive(false);
        }


        // Hides the coin image.
        if (coinImage != null)
        {
            coinImage.gameObject.SetActive(false);
        }
    }
}