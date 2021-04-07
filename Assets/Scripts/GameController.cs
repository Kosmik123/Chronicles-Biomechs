using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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


    [Header("Files stats")]
    public string initialPath = "C:\\Users\\gosia\\Documents\\Unity\\Chronicles & Biomechs\\Assets\\Scripts\\";
    public List<string> filenames = new List<string>();
    public int numberOfLines;

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
            SceneIndex.Map, LoadSceneMode.Additive);
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

#if UNITY_EDITOR
    [CustomEditor(typeof(GameController))]
    public class GameControllerEditor : Editor
    {
        GameController controller;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            controller = target as GameController;
            if (GUILayout.Button("Licz"))
            {
                controller.filenames.Clear();
                controller.numberOfLines = 0;
                CountLinesInDirectory(controller.initialPath);
            }
        }

        public void CountLinesInDirectory(string path)
        {
            string[] filenames = System.IO.Directory.GetFiles(path);
            foreach (var fName in filenames)
            {
                if (fName[fName.Length - 1] == 's')
                {   
                    int number = System.IO.File.ReadAllLines(fName).Length;
                    controller.numberOfLines += number;
                    string shortName = fName.Substring(controller.initialPath.Length);

                    controller.filenames.Add(shortName);
                    Debug.Log(shortName + " ma lini: " + number);
                }
            }

            string[] dirNames = System.IO.Directory.GetDirectories(path);
            foreach (var dName in dirNames)
                CountLinesInDirectory(dName);
        }

    }

#endif
}

public enum SceneIndex
{
    Global = 0,
    Map = 1,
    Puzzle = 2,
    Dialogue = 3
}
