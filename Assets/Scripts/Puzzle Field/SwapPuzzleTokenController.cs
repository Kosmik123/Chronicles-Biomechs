using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TokenType
{
    NONE, NORMAL, BOMB, COLOR
}
[RequireComponent(typeof(Token))]
public class SwapPuzzleTokenController : MonoBehaviour
{
    public SwapController puzzleController;

    [Header("To Link")]
    public SpriteRenderer normalSprite;
    public SpriteRenderer bombSprite;
    public SpriteRenderer colorSprite;
    public Token token;

    [Header("Types")]
    public TokenType type;
    public bool hasJustChangedType;
    public bool wasActivated;

    [Header("Matches")]
    public bool isChecking;
    public bool isMatched;

    [Header("Swipe")]
    private Vector3 pressPosition, releasePosition;

    private TroopMover troop;


    public void Awake()
    {
        puzzleController = SwapController.main;
        token = GetComponent<Token>();    
    }

    void Start()
    {
        type = TokenType.NORMAL;

    }
    void Update()
    {
        if (token.isDisappearing)
            ManageDisappearing();

        normalSprite.transform.localScale = new Vector3(
            1 + (type == TokenType.BOMB ? 0.5f : 0),
            1 + (type == TokenType.COLOR ? 0.5f : 0),
            1);
        if(!hasJustChangedType)
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
                Settings.main.elements[token.elementId].color;
    }

    private void ManageDisappearing()
    {
        if (token.animator.GetCurrentAnimatorStateInfo(0).IsName("Disappeared"))
        {
            token.isDisappearing = false;
            troop.isMoving = true;

            if (!hasJustChangedType)
            {
                PuzzleGrid.main.tokens[token.gridPosition.y, token.gridPosition.x] = null;
                Destroy(gameObject);
            }
            hasJustChangedType = false;
        }
    }

    public void UpdateSprite()
    {
        normalSprite.sprite = Settings.main.elements[token.elementId].maskSprite;
    }

    public void UpdateSpriteImmediate()
    {
        normalSprite.sprite = Settings.GetImmediate().elements[token.elementId].maskSprite;
    }

    public void Activate()
    {
        if (!hasJustChangedType)
        {
            if (type == TokenType.BOMB)
            {
                SwapController.main.DestroyNeighbours(token.gridPosition);
                wasActivated = true;
            }
            else if (type == TokenType.COLOR)
            {
                SwapController.main.DestroyTokensOfElement(token.elementId);
                wasActivated = true;
            }
        }
    }



    public void BeginChangeToTroops()
    {
        if (!token.isDisappearing)
        {
            token.isDisappearing = true;
            if (type != TokenType.NORMAL && !wasActivated)
                Activate();

            isMatched = false;
            var troopObj = Instantiate(Settings.main.troops.prefab, transform.position, Quaternion.identity);
            troop = troopObj.GetComponent<TroopMover>();
            troop.troop = Player.main.troopsByElement[token.elementId];
            troop.UpdateSprite();

            token.animator.SetTrigger("Reveal");
        }
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






    private void OnMouseDown()
    {
        if (!PuzzleGrid.main.IsAnyTokenMoving())
        {
            pressPosition = Input.mousePosition;
        }
    }

    private void OnMouseUp()
    {
        if (token.isMoving)
            return;

        if (token.isClicked)
        {
            Debug.Log("Released after click");
            releasePosition = Input.mousePosition;

            Vector2Int direction = GetSwipeDirection();
            SwapController.main.SwapTokensBySwipe(this, direction);

            token.isClicked = false;
            token.holdTime = 0;
        }
    }

    private void OnMouseUpAsButton()
    {
        if (!token.isMoving)
        {
            if (type != TokenType.NORMAL && token.holdTime < 0.5f) 
                Activate();
            else
                puzzleController.Select(this);
        }
        token.animator.SetBool("Shake", false);
    }

    private void OnDrawGizmos()
    {
        UpdateSpriteImmediate();
    }
}
