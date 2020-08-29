using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class TokenChain
{
    public List<SwapPuzzleTokenController> tokens;
    
    public TokenChain()
    {
        tokens = new List<SwapPuzzleTokenController>();
    }

    public void Add(SwapPuzzleTokenController token)
    {
        tokens.Add(token);
    }

    public void Add(TokenChain other)
    {
        foreach(SwapPuzzleTokenController token in other.tokens)
        {
            tokens.Add(token);
        }
    }

    public bool IsMatchFound()
    {
        return (tokens.Count >= 3);
    }

    public void ChangeTokenTypes()
    {
        if(tokens.Count == 4)
        {
            SwapPuzzleTokenController changedToken = tokens[1];
            foreach(SwapPuzzleTokenController con in tokens)
            {
                if (con.token.wasMoved)
                {
                    changedToken = con;
                    break;
                }  
            }
            changedToken.type = TokenType.BOMB;
            changedToken.hasJustChangedType = true;
            changedToken.token.wasMoved = false;
        }
        else if(tokens.Count >= 5)
        {
            SwapPuzzleTokenController changedToken = tokens[0];
            foreach (SwapPuzzleTokenController con in tokens)
            {
                if (con.token.wasMoved)
                {
                    changedToken = con;
                    break;
                }
            }
            changedToken.type = TokenType.COLOR;
            changedToken.hasJustChangedType = true;
            changedToken.token.wasMoved = false;
        }
    }
}


public abstract class  PuzzleController : MonoBehaviour
{
    public PuzzleGrid grid;

    [Header("States")]
    public bool hasJustSwapped;
    public bool matchFound;
    public bool hasJustDestroyed;
    public bool bombActivated;

    [Header("Swapped Tokens")]
    public SwapPuzzleTokenController selectedToken;

    public abstract void DoUpdate();
}

public class SwapController : PuzzleController
{ 
    public static SwapController main;

    public List<TokenChain> tokenChains = new List<TokenChain>();

    [Header("Settings")]
    public bool debugMode;


    private void Awake()
    {
        main = this;
        grid = GetComponent<PuzzleGrid>();
    }


    // Start is called before the first frame update
    void Start()
    {

    }


    public override void DoUpdate()
    {
        if (debugMode)
            return;


        if (!grid.IsAnyTokenMoving() && !grid.IsAnyTokenAnimating())
        {
            if (hasJustSwapped || hasJustDestroyed || matchFound || grid.isFresh)
            {
                FindMatches();
                foreach (TokenChain chain in tokenChains)
                {
                    chain.ChangeTokenTypes();
                }

                MoveUnmatchedBack();
                hasJustDestroyed = false;
                grid.isFresh = false;
            }

            if (matchFound || hasJustSwapped)
            {
                do
                {
                    bombActivated = false;
                    ChangeMatchedToTroops();
                    tokenChains.Clear();

                } while (bombActivated);
            }
            hasJustSwapped = false;
        }

    }

    public void Select(SwapPuzzleTokenController swapped)
    {
        if (selectedToken != null && swapped.token.IsNextTo(selectedToken.token))
        {
            SwapTokens(selectedToken, swapped);
        }
        else
            selectedToken = swapped;
    }

    public void FindMatches()
    {
        matchFound = false;

        for (int j = 0; j < grid.gridSize.y; j++)
        {
            for (int i = 0; i < grid.gridSize.x; i++)
            {
                var cont = grid.tokens[j, i].GetComponent< SwapPuzzleTokenController>();
                if (!cont.isMatched)
                {
                    cont.isChecking = true;
                    TokenChain chain = FindMatches(cont, true);
                    if(chain.IsMatchFound())
                    {
                        matchFound = true;
                        tokenChains.Add(chain);
                    }
                    matchFound = chain.IsMatchFound() || matchFound;
                }    
            }
        }

        foreach (MaskToken tok in grid.tokens)
            tok.GetComponent<SwapPuzzleTokenController>().isChecking = false;
    }

    protected void MoveUnmatchedBack()
    {
        foreach (MaskToken token in grid.tokens)
        {
            if (token != null && token.wasMoved)
            {
                if (token.previousGridPosition.x != -1 && token.previousGridPosition.y != -1)
                {
                    var tokenCont = token.GetComponent<SwapPuzzleTokenController>();
                    if (!tokenCont.isMatched)
                    {
                        var other = grid.tokens[
                            token.previousGridPosition.y,
                            token.previousGridPosition.x].GetComponent<
                                SwapPuzzleTokenController>();
                        if (other != null)
                        {
                            if (!other.isMatched)
                            {
                                MoveBack(token);
                                MoveBack(other.token);
                            }
                            other.token.wasMoved = false;
                        }
                        token.wasMoved = false;
                    }
                }
            }
        }
    }

    public void SwapTokens(SwapPuzzleTokenController cont1, SwapPuzzleTokenController cont2)
    {
        cont1.token.previousGridPosition = cont1.token.gridPosition;
        cont2.token.previousGridPosition = cont2.token.gridPosition;

        grid.SetTokenInGrid(cont1.token, cont2.token.previousGridPosition);
        grid.SetTokenInGrid(cont2.token, cont1.token.previousGridPosition);

        grid.UpdateTokenPosition(cont1.token, Settings.main.tokens.swapTime);
        grid.UpdateTokenPosition(cont2.token, Settings.main.tokens.swapTime);

        hasJustSwapped = true;
    }

    public void SwapTokensBySwipe(SwapPuzzleTokenController token, Vector2Int direction)
    {
        if (direction.sqrMagnitude != 1)
        {
            return;
        }
        Vector2Int otherPos = token.token.gridPosition + direction;
        if (otherPos.x < 0 || otherPos.x >= grid.gridSize.x ||
            otherPos.y < 0 || otherPos.y >= grid.gridSize.y)
            return;

        SwapPuzzleTokenController otherToken = grid.tokens[otherPos.y, otherPos.x].GetComponent<SwapPuzzleTokenController>();
        SwapTokens(token, otherToken);
    }


    public void DestroyTokensOfElement(int id)
    {
        foreach (MaskToken token in grid.tokens)
        {
            if (token.elementId == id)
            {
                token.GetComponent<SwapPuzzleTokenController>().isMatched = true;
            }
        }
        bombActivated = true;
        hasJustSwapped = true;
    }


    public void DestroyNeighbours(Vector2Int pos)
    {
        grid.tokens[pos.y, pos.x].GetComponent<
            SwapPuzzleTokenController>().isMatched = true;

        if (pos.y + 1 < grid.gridSize.y)
            grid.tokens[pos.y + 1, pos.x].GetComponent<
                SwapPuzzleTokenController>().isMatched = true;
        if (pos.y > 0)
            grid.tokens[pos.y - 1, pos.x].GetComponent<
                SwapPuzzleTokenController>().isMatched = true;
        if (pos.x + 1 < grid.gridSize.x)
            grid.tokens[pos.y, pos.x + 1].GetComponent<
                SwapPuzzleTokenController>().isMatched = true;
        if (pos.x > 0)
            grid.tokens[pos.y, pos.x - 1].GetComponent<
                SwapPuzzleTokenController>().isMatched = true;

        bombActivated = true;
        hasJustSwapped = true;
    }


    public TokenChain FindMatches(SwapPuzzleTokenController cont, bool mustAddToChain = false)
    {
        TokenChain chain = new TokenChain();
        bool isAdded = !mustAddToChain;

        Vector2Int[] directions = new Vector2Int[] {
            Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int nearPos = cont.token.gridPosition + dir;
            if (grid.IsInsideGrid(nearPos))
            {
                var near = grid.tokens[nearPos.y, nearPos.x].GetComponent<SwapPuzzleTokenController>();
                // check nearest token 
                if (!near.isChecking && near.token.elementId == cont.token.elementId)
                {
                    Vector2Int furtherPos = nearPos + dir;
                    Vector2Int backPos = cont.token.gridPosition - dir;

                    if (grid.IsInsideGrid(furtherPos))
                    {
                        var further = grid.tokens[furtherPos.y, furtherPos.x].GetComponent<SwapPuzzleTokenController>();
                        if (further.token.elementId == cont.token.elementId)
                        {
                            if (!isAdded)
                            {
                                isAdded = true;
                                chain.Add(cont);
                            }

                            cont.isMatched = true;
                            near.isMatched = true;
                            further.isMatched = true;

                            bool checkNear = false, checkFurther = false;
                            if (!near.isChecking)
                            {
                                chain.Add(near);
                                near.isChecking = true;
                                checkNear = true;
                            }

                            if (!further.isChecking)
                            {
                                chain.Add(further);
                                further.isChecking = true;
                                checkFurther = true;
                            }

                            if (checkNear)
                                chain.Add(FindMatches(near));

                            if (checkFurther)
                                chain.Add(FindMatches(further));
                        }
                    }

                    if (grid.IsInsideGrid(backPos))
                    {
                        var back = grid.tokens[backPos.y, backPos.x].GetComponent<SwapPuzzleTokenController>();
                        if (back.token.elementId == cont.token.elementId)
                        {
                            if (!isAdded)
                            {
                                isAdded = true;
                                chain.Add(cont);
                            }

                            cont.isMatched = true;
                            near.isMatched = true;
                            back.isMatched = true;

                            bool checkNear = false, checkBack = false;
                            if (!near.isChecking)
                            {
                                chain.Add(near);
                                near.isChecking = true;
                                checkNear = true;
                            }

                            if (!back.isChecking)
                            {
                                chain.Add(back);
                                back.isChecking = true;
                                checkBack = true;
                            }

                            if (checkNear)
                                chain.Add(FindMatches(near));

                            if (checkBack)
                                chain.Add(FindMatches(back));
                        }
                    }
                }
            }
        }
        return chain;
    }

    public void DestroyMatchedAndCollapse()
    {
        ChangeMatchedToTroops();
        grid.Collapse();
    }

    public void ChangeMatchedToTroops()
    {
        foreach (MaskToken token in grid.tokens)
        {
            if (token != null)
            {
                var cont = token.GetComponent<SwapPuzzleTokenController>();
                if (cont.isMatched)
                {
                    cont.BeginChangeToTroops();
                }
            }
        }
        hasJustDestroyed = true;
    }

    public void MoveBack(MaskToken token)
    {
        Debug.Log("Moving back");
        grid.SetTokenInGrid(token, token.previousGridPosition);
        token.BeginMovingToPosition(grid.GridToWorldPosition(token.gridPosition), 
            Settings.main.tokens.swapTime);
    }





#if UNITY_EDITOR
    [CustomEditor(typeof(SwapController))]
    public class SwapControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Find Matches (1)"))
            {
                SwapController controller = target as SwapController;
                SwapController.main = controller;
                controller.FindMatches();
            }

            if (GUILayout.Button("Change Token Types (2)"))
            {
                SwapController controller = target as SwapController;
                SwapController.main = controller;
                foreach (TokenChain chain in controller.tokenChains)
                {
                    chain.ChangeTokenTypes();
                }
            }

            if (GUILayout.Button("Move Unmatched Back (3)"))
            {
                SwapController controller = target as SwapController;
                SwapController.main = controller;
                controller.MoveUnmatchedBack();
            }

            if (GUILayout.Button("Change To Troops (4)"))
            {
                SwapController controller = target as SwapController;
                SwapController.main = controller;
                controller.ChangeMatchedToTroops();
            }
        }
    }
#endif
}
