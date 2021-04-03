using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSwipeManager : MonoBehaviour
{
    const int MODE_STAGES = 0;
    const int MODE_TEAMS = 1;
    


    public enum SwipeDirection
    {
        NOT_DECIDED, VERTICAL, HORIZONTAL
    }


    [Header("To link")]
    public Camera camera;

    [Header("Settings")]
    public Sprite panelSprite;
    public float panelSpriteScale;
    public Vector2 swipeDistances;
    public float minimumMoveTime;
    public float minimumSquareMouseShift;
    public int stagesAmount;

    [Header("States")]
    public int currentStage;
    public int currentMode;
    //private Vector3 moveDirection;
    private bool isPressed;

    [Header("Swipe states")]
    private Vector3 mousePressWorldPosition;
    private Vector3 lastPosition;
    private SwipeDirection swipeDirection;
    private float timer;
    private Vector3 mouseShift;

    [System.Serializable]
    public class SnapToGridSettings
    {
        public float speed;
        [SerializeField]
        private Vector3 targetPosition, lastPosition;
        [SerializeField]
        private float progress;
        public bool isActive;

        public void SetNewTarget(Vector3 lastPos, Vector3 target)
        {
            lastPosition = lastPos;
            targetPosition = target;
            isActive = true;
            progress = 0;
        }

        public Vector3 GetPosition()
        {
            progress += speed * Time.deltaTime;
            if (progress > 1)
                isActive = false;
            return Vector3.Lerp(lastPosition, targetPosition, progress);
        }
    }
    public SnapToGridSettings snapping;

    void Start()
    {
        swipeDistances = panelSpriteScale * panelSprite.rect.size / panelSprite.pixelsPerUnit; 
    }

    void Update()
    {   
        if (Input.GetMouseButtonDown(0))
        {
            OnMousePressed();
            snapping.isActive = false;
            swipeDirection = SwipeDirection.NOT_DECIDED;
            timer = 0;
            mouseShift = Vector3.zero;
        }
        else if(isPressed && Input.GetMouseButtonUp(0))
        {
            isPressed = false;
            currentStage = Mathf.Clamp(
                -Mathf.RoundToInt(transform.position.x / swipeDistances.x),
                0, stagesAmount - 1);
            currentMode = -Mathf.RoundToInt(transform.position.y / swipeDistances.y);
            SnapToGrid();
        }

        if (isPressed)
        {
            Vector3 currentMouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);

            if (swipeDirection == SwipeDirection.NOT_DECIDED)
                timer += Time.deltaTime;
            if (timer > minimumMoveTime)
            {
                Vector3 mousePosDiff = currentMouseWorldPosition - mousePressWorldPosition;
                if (mousePosDiff.sqrMagnitude > minimumSquareMouseShift)
                {
                    if (swipeDirection == SwipeDirection.NOT_DECIDED)
                    {
                        if (Mathf.Abs(mousePosDiff.x) > Mathf.Abs(mousePosDiff.y))
                        {
                            swipeDirection = SwipeDirection.HORIZONTAL;
                        }
                        else
                        {
                            swipeDirection = SwipeDirection.VERTICAL;
                        }
                    }
                    if (swipeDirection == SwipeDirection.HORIZONTAL)
                    {
                        float mousePosDiffX = mousePosDiff.x;
                        if ((mousePosDiffX > 0 && currentStage <= 0) ||
                           (mousePosDiffX < 0 && currentStage >= stagesAmount - 1) ||
                           currentMode != MODE_STAGES)
                            mousePosDiffX = 0;
                        mouseShift = Vector3.right * mousePosDiffX;
                    }
                    else if (swipeDirection == SwipeDirection.VERTICAL)
                    {
                        float mousePosDiffY = mousePosDiff.y;
                        if ((mousePosDiffY > 0 && currentMode <= 0) ||
                           (mousePosDiffY < 0 && currentMode >= 1))
                            mousePosDiffY = 0;
                        mouseShift = Vector3.up * mousePosDiffY;
                    }
                }
            }
            transform.position = lastPosition + mouseShift;
        }
        else if (snapping.isActive)
            transform.position = snapping.GetPosition();
    }

    void SnapToGrid()
    {
        Vector2 gridPosition = new Vector2(transform.position.x / swipeDistances.x, transform.position.y / swipeDistances.y);
        int xGridPos = (int) Mathf.Round(gridPosition.x);
        int yGridPos = (int) Mathf.Round(gridPosition.y);

        snapping.SetNewTarget(transform.position, new Vector3(
            xGridPos * swipeDistances.x,
            yGridPos * swipeDistances.y,
            transform.position.z));
    }

    private void OnMousePressed()
    {
        isPressed = true;
        mousePressWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        lastPosition =  transform.position;
    }

    private void OnMouseDown()
    {

    }

    private void OnMouseUp()
    {

    }
}
