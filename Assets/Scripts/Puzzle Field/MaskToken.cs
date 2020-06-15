using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MaskToken : MonoBehaviour
{
    public new SpriteRenderer renderer;

    [Header("Settings")]
    public Mask mask;

    [Header("States")]
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

    public int elementId => mask.elementId;

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
        renderer.sprite = mask.kanohi.sprite;
        renderer.color = Settings.main.elements[mask.elementId].maskColor;
    }

    public void UpdateSpriteImmediate()
    {
        renderer.sprite = mask.kanohi.sprite;
        renderer.color = Settings.GetImmediate().elements[mask.elementId].maskColor;
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
            int elemId = Random.Range(0, allElementsCount);
            mask = BattleData.main.battlingTeam.maskSets[elemId].mask;
            hasBadElement = false;
            foreach (int id in exceptionIds)
            {
                if (id == elemId)
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
        int elemId = Random.Range(0, Settings.main.elements.Length);
        mask = BattleData.main.battlingTeam.maskSets[elemId].mask;
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

    public bool IsNextTo(MaskToken other)
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
    [CustomEditor(typeof(MaskToken))]
    public class TokenEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Update Sprite"))
            {
                MaskToken token = target as MaskToken;
                token.UpdateSpriteImmediate();
            }
        }
    }


#endif
}

