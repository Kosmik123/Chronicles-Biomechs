using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PuzzleToken : MonoBehaviour
{
    public enum TokenType
    {
        NONE, NORMAL, BOMB, COLOR
    }

    [Header("To Link")]
    public SpriteRenderer normalSprite;
    public SpriteRenderer bombSprite;
    public SpriteRenderer colorSprite;


    [Header("Settings")]
    public int elementId;
    public Vector2Int gridPosition;
    public Vector2Int previousGridPosition;

    [Header("Matches")]
    public bool isChecking;
    public bool isMatched;

    [Header("Types")]
    public TokenType type;
    public bool hasJustChangedType;

    [Header("Click")]
    public bool isClicked;
    public float holdTime;

    [Header("Movement")]
    public bool isMoving;
    public bool wasMoved;
    Vector3 targetPosition, lastPosition;
    public float defaultMoveTime;
    private float moveTime;
    private float moveProgress;

    [Header("Swipe")]
    private Vector3 pressPosition, releasePosition;

    [Header("Animation")]
    public bool isDisappearing;
    private Animator animator;
    public AnimationClip revealAnimation, shakeAnimation;

    private TroopMover troop;

    void Start()
    {
        type = TokenType.NORMAL;
        animator = GetComponent<Animator>();
        previousGridPosition = Vector2Int.one * -1;
    }

    void Update()
    {
        if (isMoving)
            ManageMovement();

        if (isDisappearing)
            ManageDisappearing();

        if(isClicked)
        {
            holdTime += Time.deltaTime;
        }



        normalSprite.transform.localScale = new Vector3(
            1 + (type == TokenType.BOMB ? 0.5f : 0),
            1 + (type == TokenType.COLOR ? 0.5f : 0),
            1);

        // transform.localScale = new Vector3(1 + (isClicked ? 0.1f : 0), 1 + (isClicked ? 0.1f : 0), 1);
        ManageSprites();
    }

    private void ManageSprites()
    {
        normalSprite.enabled = (type == TokenType.NORMAL);
        bombSprite.enabled = (type == TokenType.BOMB);
        colorSprite.enabled = (type == TokenType.COLOR);

        if (type == TokenType.NORMAL)
            normalSprite.color = isMatched ? Color.grey : Color.white;
        else
            colorSprite.color = bombSprite.color =
                Settings.main.elements[elementId].color;
    }

    private void ManageMovement()
    {
        if (moveProgress < moveTime)
        {
            moveProgress += Time.deltaTime;
            transform.position = Vector3.Lerp(lastPosition, targetPosition, moveProgress / moveTime);
        }
        else
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    private void ManageDisappearing()
    {
        Debug.Log("Animacja 0: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Disappeared"));
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Disappeared"))
        {
            isDisappearing = false;
            troop.isMoving = true;

            if (!hasJustChangedType)
            {
                PuzzleGrid.main.tokens[gridPosition.y, gridPosition.x] = null;
                Destroy(gameObject);
            }
            hasJustChangedType = false;
        }
    }

    public void SetRandomType(int[] exceptionIds)
    {
        int allElementsCount = Settings.main.elements.Length;
        bool hasBadElement;
        do
        {
            elementId = Random.Range(0, allElementsCount);
            hasBadElement = false;
            foreach (int id in exceptionIds)
            {
                if (id == elementId)
                {
                    hasBadElement = true;
                    break;
                }
            }
        } while (hasBadElement);
        UpdateSprite();
    }

    public void SetRandomType()
    {
        elementId = Random.Range(0, Settings.main.elements.Length);
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        normalSprite.sprite = Settings.main.elements[elementId].sprite;
    }
    public void UpdateSpriteImmediate()
    {
        normalSprite.sprite = Settings.GetImmediate().elements[elementId].sprite;
    }
    public void BeginMovingToPosition(Vector3 newPos, float timeOfMove = -1)
    {
        if (timeOfMove == 0)
        {
            transform.position = newPos;
        }
        else
        {
            if (timeOfMove < 0)
                moveTime = defaultMoveTime;
            else
                moveTime = timeOfMove;

            lastPosition = transform.position;
            targetPosition = newPos;
            moveProgress = 0;
            isMoving = true;
            wasMoved = true;
        }
    }

    public bool IsNextTo(PuzzleToken other)
    {
        int xDiff = Mathf.Abs(gridPosition.x - other.gridPosition.x);
        int yDiff = Mathf.Abs(gridPosition.y - other.gridPosition.y);

        if (xDiff + yDiff == 1)
            return true;
        return false;
    }

    public void Activate()
    {
        if (!hasJustChangedType)
        {
            SwapController.main.hasJustDestroyed = true;
            if (type == TokenType.BOMB)
                PuzzleGrid.main.DestroyNeighbours(gridPosition);
            else if (type == TokenType.COLOR)
                PuzzleGrid.main.DestroyTokensOfElement(elementId);
        }
    }


    public void BeginChangeToTroops()
    {
        if (!isDisappearing)
        {
            isDisappearing = true;
            if (type != TokenType.NORMAL)
                Activate();

            isMatched = false;
            var troopObj = Instantiate(Settings.main.troops.prefab, transform.position, Quaternion.identity);
            troop = troopObj.GetComponent<TroopMover>();
            troop.troop = Player.main.troopsByElement[elementId];
            troop.UpdateSprite();
            
            if(type == TokenType.NORMAL)
                animator.SetTrigger("Reveal");
        }
    }

    private void OnMouseDown()
    {
        if (!PuzzleGrid.main.IsAnyTokenMoving())
        {
            pressPosition = Input.mousePosition;
            animator.SetBool("Shake", true);
            isClicked = true;

        }
    }

    private void OnMouseUpAsButton()
    {
        if (!isMoving)
        {
            if(type != TokenType.NORMAL && holdTime < 0.5f) Activate();
            SwapController.main.AddToSwap(this);
            
        }
        animator.SetBool("Shake", false);
    }

    private void OnMouseUp()
    {
        if (isMoving)
            return;
        
        if(isClicked)
        {
            Debug.Log("Released after click");
            releasePosition = Input.mousePosition;

            Vector2Int direction = GetSwipeDirection();
            SwapController.main.SwapTokensBySwipe(this, direction);

        }

        isClicked = false;
        holdTime = 0;
        animator.SetBool("Shake", false);
    }

    Vector2Int GetSwipeDirection()
    {
        float dist = Settings.main.tokens.minimalSwipeDistance;
        Debug.Log("przesunięcie to" + (releasePosition - pressPosition).sqrMagnitude);
        if ((releasePosition - pressPosition).sqrMagnitude < dist)
        {
            Debug.Log("No swipe");
            return Vector2Int.zero;
        }

        float angle = Mathf.Atan2(releasePosition.y - pressPosition.y,
            releasePosition.x - pressPosition.x) * Mathf.Rad2Deg;
        if (angle > -135 && angle <= -45)
        {
            Debug.Log("Swiped up");
            return Vector2Int.up;
        }
        if (angle > -45 && angle <= 45)
        {
            Debug.Log("Swiped right");
            return Vector2Int.right;
        }
        if (angle > 45 && angle <= 135)
        {
            Debug.Log("Swiped down");
            return Vector2Int.down;
        }
        Debug.Log("Swiped left");
        return Vector2Int.left;
    }


    private void OnDrawGizmos()
    {
        UpdateSpriteImmediate();
    }
}

