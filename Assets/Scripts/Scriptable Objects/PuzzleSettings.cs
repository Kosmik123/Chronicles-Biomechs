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
        
        public float minimalSwipeSquareDistance;
        public float minimalSwipeSquareDistanceWithRelease;

        public float relativeDistanceAtGeneration;
        public Vector2 radius;
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
    public class ParticleSettings
    {
        public GameObject prefab;
    }
    public ParticleSettings particles;







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
    public Sprite maskSprite;

    [Header("Colors")]
    public Color color;
    public Color secondaryColor;
    public Color cardColor;
    public Color particlesColor;

    public Sprite defaultBackground;

}