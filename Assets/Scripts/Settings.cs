using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static PuzzleSettings main;

    [SerializeField]
    private PuzzleSettings settingsToLink;

    void Awake()
    {
        main = settingsToLink;
    }

    public static PuzzleSettings GetImmediate()
    {
        main = FindObjectOfType<Settings>().settingsToLink;
        return main;
    }
}
