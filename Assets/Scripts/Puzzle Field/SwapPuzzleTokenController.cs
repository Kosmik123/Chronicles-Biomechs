using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TokenType
{
    NONE, NORMAL, BOMB, COLOR
}
[RequireComponent(typeof(MaskToken))]
public class SwapPuzzleTokenController : MonoBehaviour
{
    public SwapController puzzleController;

    [Header("To Link")]
    public SpriteRenderer normalSprite;
    public SpriteRenderer bombSprite;
    public SpriteRenderer colorSprite;
    public MaskToken token;

    [Header("Types")]
    public TokenType type;
    public bool hasJustChangedType;
    public bool wasActivated;

    [Header("Matches")]
    public bool isChecking;
    public bool isMatched;

    [Header("Swipe")]
    public Vector3 pressWorldPosition;
    public Vector3 releaseWorldPosition;

    private TroopMover troop;


    public void Awake()
    {
        puzzleController = SwapController.main;
        token = GetComponent<MaskToken>();    
    }

    void Start()
    {
        type = TokenType.NORMAL;
        GetComponent<BoxCollider2D>().size = Settings.main.tokens.size;
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

        if (token.isClicked)
        {
            releaseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float dist = Settings.main.tokens.minimalSwipeSquareDistance;

            if ((releaseWorldPosition - pressWorldPosition).sqrMagnitude > dist)
            {
                Vector2Int direction = GetSwipeDirection();
                SwapController.main.SwapTokensBySwipe(this, direction);
                token.isClicked = false;
            }
        }

    }

    private void ManageSprites()
    {
        normalSprite.enabled = (type == TokenType.NORMAL);
        bombSprite.enabled = (type == TokenType.BOMB);
        colorSprite.enabled = (type == TokenType.COLOR);

        normalSprite.color = colorSprite.color = bombSprite.color =
             Settings.main.elements[token.elementId].maskColor;
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
            {
                Activate();
                isMatched = false;
            }

            var troopObj = Instantiate(Settings.main.troops.prefab, 
                transform.position, Quaternion.identity, transform.parent);
            troop = troopObj.GetComponent<TroopMover>();
            troop.troop = BattleData.main.heroTeam.maskSets[token.elementId].troop;
            troop.UpdateSprite();

            token.animator.SetTrigger("Reveal");
        }
    }




    Vector2Int GetSwipeDirection()
    {
        float angle = Mathf.Atan2(releaseWorldPosition.y - pressWorldPosition.y,
            releaseWorldPosition.x - pressWorldPosition.x) * Mathf.Rad2Deg;
        if (angle > -135 && angle <= -45)
        {
            return Vector2Int.up;
        }
        if (angle > -45 && angle <= 45)
        {
            return Vector2Int.right;
        }
        if (angle > 45 && angle <= 135)
        {
            return Vector2Int.down;
        }
        return Vector2Int.left;
    }

    private void OnMouseDown()
    {
        if (!PuzzleGrid.main.IsAnyTokenMoving())
        {
            pressWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    
    private void OnMouseUp()
    {
        if (token.isMoving)
            return;

        if (token.isClicked)
        {
            releaseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float dist = Settings.main.tokens.minimalSwipeSquareDistanceWithRelease;
            if ((releaseWorldPosition - pressWorldPosition).sqrMagnitude > dist)
            {
                Vector2Int direction = GetSwipeDirection();
                SwapController.main.SwapTokensBySwipe(this, direction);
            }
            token.isClicked = false;
        }
    }


    private void OnMouseUpAsButton()
    {
        if (!token.isMoving)
        {
            if (type != TokenType.NORMAL && token.holdTime < 0.5f && !wasActivated)
            {
                Debug.Log("Aktywuje bombe");
                Activate();
            }
            else
                puzzleController.Select(this);
        }
    }

    private void OnDrawGizmos()
    {
        UpdateSpriteImmediate();
    }
}
