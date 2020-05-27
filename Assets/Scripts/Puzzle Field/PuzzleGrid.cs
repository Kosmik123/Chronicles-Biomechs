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
    public GameObject toDoSthToLink;

    [Header("Settings")]
    public Vector2Int gridSize;
    [SerializeField]
    private Bounds fieldBounds;
    [SerializeField]
    private Vector2 tokenSize;
    public bool generateNonMatchedGrid;
    public MoveDirection collapseDirection;

    [Header("States")]
    public Token[,] tokens;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        CreateGrid(generateNonMatchedGrid);
    }

    void Update()
    {
        Collapse();
    }

    public void CreateGrid(bool avoidMatches = false, bool withMove = true)
    {
        tokens = new Token[gridSize.y, gridSize.x];
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                CreateToken(i, j, avoidMatches, withMove);
            }
        }
    }

    private void CreateToken(int gridX, int gridY, bool avoidMatches, bool withMove)
    {
        Vector3 newPos;
        if (withMove)
        {
            bool isDown = (collapseDirection == MoveDirection.Down);
            newPos = GridToWorldPosition(
                    gridX,
                    gridY + (isDown ? -1 : 1) * (gridSize.y)) +
                        Vector3.up * (-Mathf.Abs(gridX - gridSize.x / 2) * tokenSize.y * 2 - 1);
        }
        else
        {
            newPos = GridToWorldPosition(gridX, gridY);
        }

        GameObject tokenObj = Instantiate(Settings.main.tokens.prefab,
            newPos, Quaternion.identity, transform);
        Token token = tokenObj.GetComponent<Token>();
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
        float relativeX = (1.0f * xGrid) / (gridSize.x - 1);
        float relativeY = (1.0f * yGrid) / (gridSize.y - 1);

        float x = Mathf.LerpUnclamped(fieldBounds.min.x + tokenSize.x,
            fieldBounds.max.x - tokenSize.x, relativeX);
        float y = Mathf.LerpUnclamped(fieldBounds.max.y - tokenSize.y,
            fieldBounds.min.y + tokenSize.y, relativeY);

        return new Vector3(x, y);
    }

    public void SetTokenInGrid(Token token, int xGrid, int yGrid)
    {
        try
        {
            tokens[yGrid, xGrid] = token;
        }catch(Exception)
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

    public void SetTokenInGrid(Token token, Vector2Int gridPos)
    {
        SetTokenInGrid(token, gridPos.x, gridPos.y);
    }

    public void UpdateTokenPosition(Token token, float time=0)
    {
        token.BeginMovingToPosition(GridToWorldPosition(token.gridPosition.x, token.gridPosition.y), time);
    }

    public void Clear()
    {
        Token[] allTokens = GetComponentsInChildren<Token>();
        if (allTokens != null)
        {
            for (int i = 0; i < allTokens.Length; i++)
            {
                DestroyImmediate(allTokens[i].gameObject);
            }
        }
    }


    //public bool CheckVerticalMatches(int x, int y, int offset = 0)
    //{
    //    int maxY = y + 1 + offset;
    //    int minY = y - 1 + offset;
    //    if (maxY >= gridSize.y || minY < 0 || x >= gridSize.x)
    //        return false;

    //    Token token0, token1, token2;
    //    token0 = tokens[minY, x];
    //    token1 = tokens[y + offset, x];
    //    token2 = tokens[maxY, x];

    //    if (token0 == null || token1 == null || token2 == null)
    //        return false;

    //    if (token1.elementId == token0.elementId &&
    //        token1.elementId == token2.elementId)
    //    {
    //        token0.isMatched = token1.isMatched = token2.isMatched = true;
    //        return true;
    //    }
    //    return false;
    //}

    //public bool CheckHorizontalMatches(int x, int y, int offset = 0)
    //{
    //    int maxX = x + 1 + offset;
    //    int minX = x - 1 + offset;
    //    if (maxX >= gridSize.x || minX < 0 || y >= gridSize.y)
    //        return false;

    //    Token token0, token1, token2;
    //    token0 = tokens[y, minX];
    //    token1 = tokens[y, x + offset];
    //    token2 = tokens[y, maxX];

    //    if (token0 == null || token1 == null || token2 == null)
    //        return false;

    //    if (token1.elementId == token0.elementId &&
    //        token1.elementId == token2.elementId)
    //    {
    //        token0.isMatched = token1.isMatched = token2.isMatched = true;
    //        return true;
    //    }
    //    return false;
    //}

    public void MoveBack(Token token)
    {
        Debug.Log("Moving back");
        SetTokenInGrid(token, token.previousGridPosition);
        token.BeginMovingToPosition(GridToWorldPosition(token.gridPosition), Settings.main.tokens.swapTime);
    }


    public bool IsAnyTokenMoving()
    {
        foreach (Token token in tokens)
        {
            if (token != null && token.isMoving)
                return true;
        }
        return false;
    }

    public bool IsAnyTokenAnimating()
    {
        foreach (Token token in tokens)
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

            Token token = tokenObj.GetComponent<Token>();
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
            Token currentToken = tokens[currentRow, col];

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



#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(fieldBounds.center, fieldBounds.extents * 2);
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
            }

            if (GUILayout.Button("Generate"))
            {
                PuzzleGrid grid = target as PuzzleGrid;
                PuzzleGrid.main = grid;
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