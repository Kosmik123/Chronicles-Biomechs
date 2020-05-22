using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class TokenChain
{
    public List<PuzzleToken> tokens;
    
    public TokenChain()
    {
        tokens = new List<PuzzleToken>();
    }

    public void Add(PuzzleToken token)
    {
        tokens.Add(token);
    }

    public void Add(TokenChain other)
    {
        foreach(PuzzleToken token in other.tokens)
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
            PuzzleToken changedToken = tokens[1];
            foreach(PuzzleToken token in tokens)
            {
                if (token.wasMoved)
                {
                    changedToken = token;
                    break;
                }  
            }
            changedToken.type = PuzzleToken.TokenType.BOMB;
            changedToken.hasJustChangedType = true;
        }
        else if(tokens.Count >= 5)
        {
            PuzzleToken changedToken = tokens[0];
            foreach (PuzzleToken token in tokens)
            {
                if (token.wasMoved)
                {
                    changedToken = token;
                    break;
                }
            }
            changedToken.type = PuzzleToken.TokenType.COLOR;
            changedToken.hasJustChangedType = true;
        }
    }
}


public class SwapController : MonoBehaviour
{ 
    public static SwapController main;

    private PuzzleGrid grid;

    [Header("Swapped Tokens")]
    public PuzzleToken selectedToken;
    public List<TokenChain> tokenChains = new List<TokenChain>();

    [Header("Settings")]
    public bool debugMode;

    [Header("States")]
    public bool hasJustSwapped;
    public bool matchFound;
    public bool hasJustDestroyed;

    private void Awake()
    {
        main = this;
        grid = GetComponent<PuzzleGrid>();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!debugMode)
        {
            if (!grid.IsAnyTokenMoving() && !grid.IsAnyTokenAnimating())
            {
                if (hasJustSwapped || hasJustDestroyed || matchFound)
                {
                    FindMatches();
                    foreach (TokenChain chain in tokenChains)
                    {
                        chain.ChangeTokenTypes();
                    }

                    MoveUnmatchedBack();
                    hasJustSwapped = false;
                    hasJustDestroyed = false;
                }

                if (matchFound)
                {
                    grid.ChangeMatchedToTroops();
                    tokenChains.Clear();
                }
            }
        }
    }

    public void AddToSwap(PuzzleToken token)
    {
        if (selectedToken != null && token.IsNextTo(selectedToken))
        {
            SwapTokens(selectedToken, token);
        }
        else
            selectedToken = token;
    }

    public void FindMatches()
    {
        matchFound = false;

        for (int j = 0; j < grid.gridSize.y; j++)
        {
            for (int i = 0; i < grid.gridSize.x; i++)
            {
                PuzzleToken token = grid.tokens[j, i];
                if (!token.isMatched)
                {
                    token.isChecking = true;
                    TokenChain chain = grid.CheckMatches(token, true);
                    if(chain.IsMatchFound())
                    {
                        matchFound = true;
                        tokenChains.Add(chain);
                    }
                    matchFound = chain.IsMatchFound() || matchFound;
                }    
            }
        }

        foreach (PuzzleToken tok in grid.tokens)
            tok.isChecking = false;
    }

    protected void MoveUnmatchedBack()
    {
        foreach (PuzzleToken token in grid.tokens)
        {
            if (token != null && token.wasMoved && !token.isMatched)
            {
                PuzzleToken other = grid.tokens[token.previousGridPosition.y, token.previousGridPosition.x];
                if (other != null)
                {
                    if (!other.isMatched)
                    {
                        grid.MoveBack(token);
                        grid.MoveBack(other);
                    }
                    other.wasMoved = false;
                }
                token.wasMoved = false;
            }
        }
    }

    public void SwapTokens(PuzzleToken token1, PuzzleToken token2)
    {
        token1.previousGridPosition = token1.gridPosition;
        token2.previousGridPosition = token2.gridPosition;

        grid.SetTokenInGrid(token1, token2.previousGridPosition);
        grid.SetTokenInGrid(token2, token1.previousGridPosition);

        grid.UpdateTokenPosition(token1, Settings.main.tokens.swapTime);
        grid.UpdateTokenPosition(token2, Settings.main.tokens.swapTime);

        hasJustSwapped = true;
    }

    public void SwapTokensBySwipe(PuzzleToken token, Vector2Int direction)
    {
        if (direction.sqrMagnitude != 1)
        {
            Debug.Log("Wektor krótki");
            return;
        }
        Vector2Int otherPos = token.gridPosition + direction;
        if (otherPos.x < 0 || otherPos.x >= grid.gridSize.x ||
            otherPos.y < 0 || otherPos.y >= grid.gridSize.y)
            return;

        PuzzleToken otherToken = grid.tokens[otherPos.y, otherPos.x];
        SwapTokens(token, otherToken);
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

        }
    }
#endif
}
