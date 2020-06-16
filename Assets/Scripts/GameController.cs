using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public bool debugMode;
    public Scene currentScene;

    public GameObject test;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        debugMode = true;
#endif

        test = Instantiate(Settings.main.damageTextSettings.prefab,
            new Vector3(100,100), Quaternion.identity);

        if (!debugMode) SceneManager.LoadSceneAsync(
            SceneIndex.Puzzle, LoadSceneMode.Additive);
    
    
    }

    void Update()
    {
        if(test != null && Time.time > 2)
        {
            Destroy(test);
            test = null;
        }


    }
    
}

public class SceneIndex
{
    public const int
        Global = 0,
        Puzzle = 1,
        Dialogue = 2
        //Map,
        ;
}