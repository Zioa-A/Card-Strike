using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerDropZone : MonoBehaviour, IDropHandler
{
    public ManaManager manaManager;

    public TurnManager turnManager;

    [Header("Attack Anaimation")]
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

        GameObject enemyObject = GameObject.FindWithTag("Enemy");
        Enemy enemy = enemyObject.GetComponent<Enemy>();

        if (!turnManager.CanPlayerUseCard())
        {
            Debug.Log("You already used a card this turn.");
            return;
        }

        if (cardData != null)
        {
            if (manaManager.CanUseCard(cardData.ManaCost))
            {
                manaManager.SpendMana(cardData.ManaCost);
                Debug.Log("Player used " + cardData.cardName);
                StartCoroutine(PlayerAttackAnimation(enemy, cardData.Damage));
                turnManager.EndPlayerTurn();

            }
            else
            {
                Debug.Log("Not enough mana to use " + cardData.cardName);
            }

        }
    }


    // This coroutine creates a simple attack animation:
    // the player lunges forward, rotates slightly, damages the enemy, then returns back to normal.
    private IEnumerator PlayerAttackAnimation(Enemy enemy, int damage)
    {
        Vector2 attackPosition = playerStartPosition + new Vector2(moveDistance, 0);
        Quaternion attackRotation = Quaternion.Euler(0, 0, attackRotationZ);

        float timer = 0f;

        // Move toward enemy
        while (timer < 1f)
        {
            timer += Time.deltaTime * attackSpeed;

            playerRect.anchoredPosition = Vector2.Lerp(playerStartPosition, attackPosition, timer);
            playerRect.localRotation = Quaternion.Lerp(playerStartRotation, attackRotation, timer);

            yield return null;
        }

        // Enemy takes damage at the hit moment
        enemy.TakeDamage(damage);

        timer = 0f;

        // Move back to original position
        while (timer < 1f)
        {
            timer += Time.deltaTime * attackSpeed;

            playerRect.anchoredPosition = Vector2.Lerp(attackPosition, playerStartPosition, timer);
            playerRect.localRotation = Quaternion.Lerp(attackRotation, playerStartRotation, timer);

            yield return null;
        }

        playerRect.anchoredPosition = playerStartPosition;
        playerRect.localRotation = playerStartRotation;
    }
}