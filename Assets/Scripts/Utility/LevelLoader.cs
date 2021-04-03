using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader
{
    public static Scene LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
        return SceneManager.GetSceneByName(levelName);
    }

    public static Scene LoadLevel(int seriesId, int worldId, int levelId, int phaseId)
    {
        return LoadLevel(seriesId + "." + worldId + "." + levelId + "." + phaseId);
    }

    public static void UnloadLevel(string levelName)
    {
        SceneManager.UnloadSceneAsync(levelName);
    }

    public static void UnloadLevel(int seriesId, int worldId, int levelId, int phaseId)
    {
        UnloadLevel(seriesId + "." + worldId + "." + levelId + "." + phaseId);
    }



}
