using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public class PuzzleGrid : MonoBehaviour
{
    static public PuzzleGrid main;

    [Header("To Link")]
    public SpriteRenderer brightSquares; 
    public SpriteRenderer darkSquares; 

    [Header("Settings")]
    public Vector2Int gridSize;
    private Bounds fieldBounds;
    private Vector2 tokenSize;

    private float generationTokensRelativeDistance; 
    public bool generateNonMatchedGrid;
    public MoveDirection collapseDirection;

    [Header("States")]
    public MaskToken[,] tokens;
    public bool isFresh;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        tokenSize = Settings.main.tokens.size;
        CreateGrid(generateNonMatchedGrid);
    }

    void Update()
    {
        Collapse();

        if(Input.GetMouseButtonDown(0))
        {
            WorldToGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

    }

    public void CreateGrid(bool avoidMatches = false, bool withMove = true)
    {
        ResizeSquares();

        generationTokensRelativeDistance = Settings.main.tokens.relativeDistanceAtGeneration;
        tokens = new MaskToken[gridSize.y, gridSize.x];
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                CreateToken(i, j, avoidMatches, withMove);
            }
        }
        isFresh = true;
    }

    void ResizeSquares()
    {
        brightSquares.size = new Vector3(gridSize.x * tokenSize.x, gridSize.y * tokenSize.y);
        darkSquares.transform.localScale = tokenSize;
        darkSquares.size = gridSize;
    }

    private void CreateToken(int gridX, int gridY, bool avoidMatches, bool withMove)
    {
        Vector3 newPos;
        if (withMove)
        {
            bool isDown = (collapseDirection == MoveDirection.Down);

            float columnsVerticalOffset = -Mathf.Abs(gridX - gridSize.x / 2) * 
            tokenSize.y * generationTokensRelativeDistance;

            newPos = GridToWorldPosition(
                    gridX,
                    gridY + (isDown ? -1 : 1) * (gridSize.y )) 
                + Vector3.up * (isDown ? -1 : 1) * columnsVerticalOffset;
        }
        else
        {
            newPos = GridToWorldPosition(gridX, gridY);
        }

        GameObject tokenObj = Instantiate(Settings.main.tokens.prefab,
            newPos, Quaternion.identity, transform);
        MaskToken token = tokenObj.GetComponent<MaskToken>();
        token.previousGridPosition = new Vector2Int(-1, -1);
        SetTokenInGrid(token, gridX, gridY);
        UpdateTokenPosition(token, Settings.main.tokens.collapseTime);
        token.wasMoved = false;

        if (avoidMatches)
        {
            List<int> unwantedElements = new List<int>();
            if (gridY > 1)
            {
                if (tokens[gridY - 1, gridX].elementId == tokens[gridY - 2, gridX].elementId)
                    unwantedElements.Add(tokens[gridY - 1, gridX].elementId);
            }

            if (gridX > 1)
            {
                if (tokens[gridY, gridX - 1].elementId == tokens[gridY, gridX - 2].elementId)
                    unwantedElements.Add(tokens[gridY, gridX - 1].elementId);
            }
            token.SetRandomElement(unwantedElements.ToArray());
        }
        else
        {
            token.SetRandomElement();
        }
        token.UpdateSpriteImmediate();
        tokenObj.name = Settings.main.elements[token.elementId].name + "(" + gridX + ", " + gridY + ")";
    }

    public Vector3 GridToWorldPosition(Vector2Int position)
    {
        return GridToWorldPosition(position.x, position.y);
    }

    public Vector3 GridToWorldPosition(int xGrid, int yGrid)
    {
        float firstTokenX = transform.position.x - tokenSize.x * (gridSize.x - 1) / 2;
        float firstTokenY = transform.position.y + tokenSize.y * (gridSize.y - 1) / 2;

        float x = firstTokenX + xGrid * tokenSize.x;
        float y = firstTokenY - yGrid * tokenSize.y;

        return new Vector3(x, y);
    }

    public Vector2 WorldToGridPosition(Vector3 worldPos)
    {
        float relativeX = worldPos.x - transform.position.x;
        float relativeY = -worldPos.y + transform.position.y;

        relativeX /= tokenSize.x;
        relativeY /= tokenSize.y;

        relativeX += 0.5f * gridSize.x;
        relativeY += 0.5f * gridSize.y;

        int gridX = Mathf.FloorToInt(relativeX);
        int gridY = Mathf.FloorToInt(relativeY);

        return new Vector2(gridX, gridY);
    }


    public void SetTokenInGrid(MaskToken token, int xGrid, int yGrid)
    {
        try
        {
            tokens[yGrid, xGrid] = token;
        }
        catch(Exception)
        {
            Debug.LogError("Problematyczny index:" + xGrid + ", " + yGrid);
        }
        
        token.gridPosition = new Vector2Int(xGrid, yGrid);
    }

    public bool IsInsideGrid(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.x >= gridSize.x)
            return false;
        if (gridPos.y < 0 || gridPos.y >= gridSize.y)
            return false;
        return true;
    }

    public void SetTokenInGrid(MaskToken token, Vector2Int gridPos)
    {
        SetTokenInGrid(token, gridPos.x, gridPos.y);
    }

    public void UpdateTokenPosition(MaskToken token, float time=0)
    {
        token.BeginMovingToPosition(GridToWorldPosition(token.gridPosition.x, token.gridPosition.y), time);
    }

    public void Clear()
    {
        MaskToken[] allTokens = GetComponentsInChildren<MaskToken>();
        if (allTokens != null)
        {
            for (int i = 0; i < allTokens.Length; i++)
            {
                DestroyImmediate(allTokens[i].gameObject);
            }
        }
    }

    public bool IsAnyTokenMoving()
    {
        foreach (MaskToken token in tokens)
        {
            if (token != null && token.isMoving)
                return true;
        }
        return false;
    }

    public bool IsAnyTokenAnimating()
    {
        foreach (MaskToken token in tokens)
        {
            if (token != null && token.isDisappearing)
                return true;
        }
        return false;
    }





    public void Collapse()
    {
        CollapseColumns(collapseDirection);
    }

    private void CollapseColumns(MoveDirection direction)
    {
        if (direction != MoveDirection.Up && direction != MoveDirection.Down)
            return;

        bool isDown = (direction == MoveDirection.Down);
        for (int i = 0; i < gridSize.x; i++)
        {
            int emptyTokensCount = CollapseTokensInColumn(i, isDown);

            RefillColumn(i, emptyTokensCount, isDown);
        }
    }

    private void RefillColumn(int col, int nullCount, bool isDown)
    {
        // create new tokens and collapse them
        for (int p = 0; p < nullCount; p++)
        {
            int initialRow = isDown ? (-p - 1) : (gridSize.y + p);
            int finalRow = initialRow + (isDown ? 1 : -1) * nullCount;

            GameObject tokenObj = Instantiate(Settings.main.tokens.prefab,
                GridToWorldPosition(col, initialRow), Quaternion.identity, transform);

            MaskToken token = tokenObj.GetComponent<MaskToken>();
            token.previousGridPosition = new Vector2Int(-1, -1);
            token.SetRandomElement();

            tokenObj.name = Settings.main.elements[token.elementId].name + "(" + col + ", " + finalRow + ")";

            SetTokenInGrid(token, col, finalRow);
            token.BeginMovingToPosition(GridToWorldPosition(token.gridPosition), Settings.main.tokens.collapseTime);
            token.wasMoved = false;
        }
    }

    private int CollapseTokensInColumn(int col, bool isDown)
    {
        int nullCount = 0;
        for (int j = 0; j < gridSize.y; j++)
        {
            int currentRow = (isDown) ? (gridSize.y - j - 1) : j;
            MaskToken currentToken = tokens[currentRow, col];

            if (currentToken == null)
            {
                nullCount += 1;
            }
            else if (nullCount > 0)
            {
                SetTokenInGrid(currentToken, col, currentRow + ((isDown) ? 1 : -1) * nullCount);
                tokens[currentRow, col] = null;
                currentToken.BeginMovingToPosition(GridToWorldPosition(currentToken.gridPosition), Settings.main.tokens.collapseTime);
                currentToken.wasMoved = false;
            }
        }
        return nullCount;
    }

#if UNITY_EDITOR
    public void Print()
    {
        string[] letters = { "F", "W", "E", "A", "S", "I" };
        string result = "";
        for (int j = 0; j < gridSize.y; j++)
        {
            for (int i = 0; i < gridSize.x; i++)
            {
                if (tokens[j, i] == null)
                    result += "\t ";
                else
                {
                    result += "\t";
                    result += letters[(int)(tokens[j, i].elementId)];
                }
            }
            result += "\n";
        }
        Debug.Log(result);
    }


    private void OnDrawGizmosSelected()
    {
        tokenSize = Settings.GetImmediate().tokens.size;
        ResizeSquares();   
    }

    [CustomEditor(typeof(PuzzleGrid))]
    public class PuzzleGridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Print"))
            {
                PuzzleGrid grid = target as PuzzleGrid;
                PuzzleGrid.main = grid;
                grid.Print();
            }
            if (GUILayout.Button("Clear"))
            {
                PuzzleGrid grid = target as PuzzleGrid;
                PuzzleGrid.main = grid;
                grid.Clear();
                grid.isFresh = true;
            }

            if (GUILayout.Button("Generate"))
            {
                PuzzleGrid grid = target as PuzzleGrid;
                PuzzleGrid.main = grid; 
                grid.tokenSize = Settings.GetImmediate().tokens.size;
                grid.CreateGrid(grid.generateNonMatchedGrid, false);
            }

            if (GUILayout.Button("Collapse Columns (in loop)"))
            {
                PuzzleGrid grid = target as PuzzleGrid;
                PuzzleGrid.main = grid;
                grid.CollapseColumns(grid.collapseDirection);
            }
        }
    }
#endif
}