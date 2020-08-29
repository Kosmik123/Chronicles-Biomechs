using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController main;

    public bool debugMode;
    public Scene currentScene;

    public int currentSceneIndex;

    public GameObject test;

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
        test = Instantiate(Settings.main.damageTextSettings.prefab,
            new Vector3(100,100), Quaternion.identity);

        if (!debugMode) SceneManager.LoadSceneAsync(
            SceneLoader.Puzzle, LoadSceneMode.Additive);
    
    }

    void Update()
    {
        if(test != null && Time.time > 2f)
        {
            Destroy(test);
            test = null;
        }


    }
    
    public static void LoadLevel(LevelSettings lvToLoad)
    {
        main.levelToLoad = lvToLoad;
        ChangeScene(SceneLoader.Puzzle);
    }

    private static void ChangeScene(int newSceneIndex)
    {
        SceneManager.LoadSceneAsync(newSceneIndex, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(main.currentSceneIndex);
        main.currentSceneIndex = newSceneIndex;
    }

}

public class SceneLoader
{
    public const int
        Global = 0,
        Map = 1,
        Puzzle = 2,
        Dialogue = 3
        ;


}