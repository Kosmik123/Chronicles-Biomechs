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
    public PuzzleToken[,] tokens;

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
        tokens = new PuzzleToken[gridSize.y, gridSize.x];
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                Vector3 newPos;
                if (withMove)
                {
                    bool isDown = (collapseDirection == MoveDirection.Down);
                    newPos = GridToWorldPosition(
                            i,
                            j + (isDown ? -1 : 1) * (gridSize.y)) +
                                Vector3.up * (-Mathf.Abs(i - gridSize.x / 2) * tokenSize.y * 2 - 1);
                }
                else
                {
                    newPos = GridToWorldPosition(i, j);
                }

                GameObject tokenObj = Instantiate(Settings.main.tokens.prefab, 
                    newPos, Quaternion.identity, transform);
                PuzzleToken token = tokenObj.GetComponent<PuzzleToken>();
                token.previousGridPosition = new Vector2Int(-1, -1);
                SetTokenInGrid(token, i, j);
                UpdateTokenPosition(token, Settings.main.tokens.collapseTime);
                token.wasMoved = false;

                if (avoidMatches)
                {
                    List<int> unwantedElements = new List<int>();
                    if (j > 1)
                    {
                        if (tokens[j - 1, i].elementId == tokens[j - 2, i].elementId)
                            unwantedElements.Add(tokens[j - 1, i].elementId);
                    }

                    if (i > 1)
                    {
                        if (tokens[j, i - 1].elementId == tokens[j, i - 2].elementId)
                            unwantedElements.Add(tokens[j, i - 1].elementId);
                    }
                    token.SetRandomType(unwantedElements.ToArray());
                }
                else
                {
                    token.SetRandomType();
                }

                token.UpdateSprite();

                tokenObj.name = token.elementId + "(" + i + ", " + j + ")";
            }
        }
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

    public void SetTokenInGrid(PuzzleToken token, int xGrid, int yGrid)
    {
        try
        {
            tokens[yGrid, xGrid] = token;
        }catch(Exception e)
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

    public void SetTokenInGrid(PuzzleToken token, Vector2Int gridPos)
    {
        SetTokenInGrid(token, gridPos.x, gridPos.y);
    }

    public void UpdateTokenPosition(PuzzleToken token, float time=0)
    {
        token.BeginMovingToPosition(GridToWorldPosition(token.gridPosition.x, token.gridPosition.y), time);
    }

    public void Clear()
    {
        PuzzleToken[] allTokens = GetComponentsInChildren<PuzzleToken>();
        if (allTokens != null)
        {
            for (int i = 0; i < allTokens.Length; i++)
            {
                DestroyImmediate(allTokens[i].gameObject);
            }
        }
    }

    public TokenChain CheckMatches(PuzzleToken token, bool mustAddToChain = false)
    {
        TokenChain chain = new TokenChain();
        bool isAdded = !mustAddToChain;

        Vector2Int[] directions = new Vector2Int[] {
            Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int nearPos = token.gridPosition + dir;
            if(IsInsideGrid(nearPos))
            {
                PuzzleToken nearToken = tokens[nearPos.y, nearPos.x];
                // check nearest token 
                if (!nearToken.isChecking && nearToken.elementId == token.elementId)
                {
                    Vector2Int furtherPos = nearPos + dir;
                    Vector2Int backPos = token.gridPosition - dir;

                    if (IsInsideGrid(furtherPos))
                    {
                        PuzzleToken furtherToken = tokens[furtherPos.y, furtherPos.x];
                        if (furtherToken.elementId == token.elementId)
                        {
                            if (!isAdded)
                            {
                                isAdded = true;
                                chain.Add(token);
                            }

                            token.isMatched = true;
                            nearToken.isMatched = true;
                            furtherToken.isMatched = true;

                            bool checkNear = false, checkFurther = false;
                            if (!nearToken.isChecking)
                            {
                                chain.Add(nearToken);
                                Debug.Log("Add near 1");
                                nearToken.isChecking = true;
                                checkNear = true;
                            }

                            if (!furtherToken.isChecking)
                            {
                                chain.Add(furtherToken);
                                Debug.Log("Add further");

                                furtherToken.isChecking = true;
                                checkFurther = true;
                            }

                            if (checkNear)
                                chain.Add(CheckMatches(nearToken));

                            if (checkFurther)
                                chain.Add(CheckMatches(furtherToken));
                        }
                    }

                    if (IsInsideGrid(backPos))
                    {
                        PuzzleToken backToken = tokens[backPos.y, backPos.x];
                        if (backToken.elementId == token.elementId)
                        {
                            if (!isAdded)
                            {
                                isAdded = true;
                                chain.Add(token);
                            }

                            token.isMatched = true;
                            nearToken.isMatched = true;
                            backToken.isMatched = true;

                            bool checkNear = false, checkBack = false;
                            if (!nearToken.isChecking)
                            {
                                chain.Add(nearToken);
                                Debug.Log("Add near 2");

                                nearToken.isChecking = true;
                                checkNear = true;
                            }

                            if (!backToken.isChecking)
                            {
                                chain.Add(backToken);
                                Debug.Log("Add back");

                                backToken.isChecking = true;
                                checkBack = true;
                            }

                            if (checkNear)
                                chain.Add(CheckMatches(nearToken));

                            if (checkBack)
                                chain.Add(CheckMatches(backToken));
                        }
                    }
                }
            }
        }
        return chain;
    }

    //public bool CheckVerticalMatches(int x, int y, int offset = 0)
    //{
    //    int maxY = y + 1 + offset;
    //    int minY = y - 1 + offset;
    //    if (maxY >= gridSize.y || minY < 0 || x >= gridSize.x)
    //        return false;

    //    PuzzleToken token0, token1, token2;
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

    //    PuzzleToken token0, token1, token2;
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

    public void MoveBack(PuzzleToken token)
    {
        Debug.Log("Moving back");
        SetTokenInGrid(token, token.previousGridPosition);
        token.BeginMovingToPosition(GridToWorldPosition(token.gridPosition), Settings.main.tokens.swapTime);
    }


    public bool IsAnyTokenMoving()
    {
        foreach (PuzzleToken token in tokens)
        {
            if (token != null && token.isMoving)
                return true;
        }
        return false;
    }

    public bool IsAnyTokenAnimating()
    {
        foreach (PuzzleToken token in tokens)
        {
            if (token != null && token.isDisappearing)
                return true;
        }
        return false;
    }

    public void DestroyMatchedAndCollapse()
    {
        ChangeMatchedToTroops();
        CollapseColumns(collapseDirection);
    }

    public void ChangeMatchedToTroops()
    {
        foreach (PuzzleToken token in tokens)
        {
            if (token != null && token.isMatched)
            {
                token.BeginChangeToTroops();
            }
        }
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

            PuzzleToken token = tokenObj.GetComponent<PuzzleToken>();
            token.previousGridPosition = new Vector2Int(-1, -1);
            token.SetRandomType();

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
            PuzzleToken currentToken = tokens[currentRow, col];

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

    public void DestroyTokensOfElement(int id)
    {
        foreach(PuzzleToken token in tokens)
        {
            if(token.elementId == id)
            {
                token.BeginChangeToTroops();
            }
        }
    }

    public void DestroyNeighbours(Vector2Int pos)
    {
        tokens[pos.y, pos.x].BeginChangeToTroops();
        if (pos.y + 1 < gridSize.y)
            tokens[pos.y + 1, pos.x].BeginChangeToTroops();
        if(pos.y > 0)
            tokens[pos.y - 1, pos.x].BeginChangeToTroops();
        if(pos.x + 1 < gridSize.x)
            tokens[pos.y, pos.x + 1].BeginChangeToTroops();
        if(pos.x > 0)
            tokens[pos.y, pos.x - 1].BeginChangeToTroops();
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
                    if (tokens[j, i].isMatched) result += "[";
                    result += letters[(int)(tokens[j, i].elementId)];
                    if (tokens[j, i].wasMoved) result += "!";
                    if (tokens[j, i].isChecking) result += "c";
                    if (tokens[j, i].isMatched) result += "]";
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

            if (GUILayout.Button("Change To Troops (4)"))
            {
                PuzzleGrid grid = target as PuzzleGrid;
                PuzzleGrid.main = grid;
                grid.ChangeMatchedToTroops();
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