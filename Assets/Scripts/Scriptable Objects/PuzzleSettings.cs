using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Settings", menuName = "Puzzles/Settings")]
public class PuzzleSettings : ScriptableObject
{ 
    [Header("General")]
    public PuzzleElement[] elements;

    [System.Serializable]
    public class TokensSettings
    {
        public GameObject prefab;
        public float swapTime;
        public float collapseTime;
        public int minimalSwipeDistance;
    }
    public TokensSettings tokens;

    [System.Serializable]
    public class TroopsSettings
    {
        public GameObject prefab;
        public float moveSpeed;
    }
    public TroopsSettings troops;

    [System.Serializable]
    public class EnemiesSettings
    {
        public float[] colliderWidths;
    }
    public EnemiesSettings enemies;

}

[System.Serializable]
public class PuzzleElement
{
    public string name;
    public Sprite sprite;
    public Color color;
}