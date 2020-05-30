using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public bool debugMode;
    public Scene currentScene;


    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        debugMode = true;
#endif

        if (!debugMode) SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        currentScene = SceneManager.GetActiveScene();
    }

    
}
