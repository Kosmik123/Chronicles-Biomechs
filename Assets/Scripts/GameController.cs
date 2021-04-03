using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController main;

    public bool debugMode;
    public Scene currentScene;

    public SceneIndex currentSceneIndex;

    public GameObject textToLoad;

    [Header("Level Loading")]
    public LevelSettings levelToLoad;

    private void Awake()
    {
        main = this;
    }


    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        debugMode = true;
#endif
        textToLoad = Instantiate(Settings.main.damageTextSettings.prefab,
            new Vector3(100,100), Quaternion.identity);

        if (!debugMode) SceneManager.LoadSceneAsync( (int)
            SceneIndex.Puzzle, LoadSceneMode.Additive);
    }

    void Update()
    {
        if(textToLoad != null && Time.time > 2f)
        {
            Destroy(textToLoad);
            textToLoad = null;
        }
    }
    
    public static void LoadLevel(LevelSettings lvToLoad)
    {
        main.levelToLoad = lvToLoad;
        ChangeScene(SceneIndex.Puzzle);
    }

    private static void ChangeScene(SceneIndex newSceneIndex)
    {
        SceneManager.LoadSceneAsync((int)newSceneIndex, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync((int)main.currentSceneIndex);
        main.currentSceneIndex = newSceneIndex;
    }

}

public enum SceneIndex
{
    Global = 0,
    Map = 1,
    Puzzle = 2,
    Dialogue = 3
}