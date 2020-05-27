using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Token : MonoBehaviour
{
    public new SpriteRenderer renderer;

    [Header("Settings")]
    public int elementId;
    public Vector2Int gridPosition;
    public Vector2Int previousGridPosition;

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

    [Header("Animation")]
    public bool isDisappearing;
    public Animator animator;
    public AnimationClip revealAnimation, shakeAnimation;


    void Start()
    {
        animator = GetComponent<Animator>();
        previousGridPosition = Vector2Int.one * -1;
    }

    void Update()
    {
        if (isMoving)
            MoveToTarget();

        if (isClicked)
        {
            holdTime += Time.deltaTime;
        }

        animator.SetFloat("Shake", holdTime);
    }

    public void UpdateSprite()
    {
        renderer.sprite = Settings.main.elements[elementId].maskSprite;
    }

    public void UpdateSpriteImmediate()
    {
        renderer.sprite = Settings.GetImmediate().elements[elementId].maskSprite;
    }
    private void MoveToTarget()
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

    public void SetRandomElement(int[] exceptionIds)
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

    public void SetRandomElement()
    {
        elementId = Random.Range(0, Settings.main.elements.Length);
        UpdateSprite();
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

    public bool IsNextTo(Token other)
    {
        int xDiff = Mathf.Abs(gridPosition.x - other.gridPosition.x);
        int yDiff = Mathf.Abs(gridPosition.y - other.gridPosition.y);

        if (xDiff + yDiff == 1)
            return true;
        return false;
    }

    private void OnMouseDown()
    {
        if (!PuzzleGrid.main.IsAnyTokenMoving())
        {
            isClicked = true;
        }
    }

    private void OnMouseUp()
    {
        isClicked = false;
        holdTime = 0;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(Token))]
    public class TokenEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Update Sprite"))
            {
                Token token = target as Token;
                token.UpdateSpriteImmediate();
            }
        }
    }


#endif
}

