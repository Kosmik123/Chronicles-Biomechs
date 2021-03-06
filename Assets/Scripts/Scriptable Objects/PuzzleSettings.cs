﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Settings", menuName = "Puzzles/Settings")]
public class PuzzleSettings : ScriptableObject
{ 
    [Header("General")]
    public PuzzleElement[] elements;
    public Profession[] professions;

    [System.Serializable]
    public class TokensSettings
    {
        public GameObject prefab;

        public float swapTime;
        public float collapseTime;
        
        public float minimalSwipeSquareDistance;
        public float minimalSwipeSquareDistanceWithRelease;

        public float relativeDistanceAtGeneration;
        public Vector2 size;
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
        public string tagName;
    }
    public ParticleSettings particles;

    [System.Serializable]
    public class HeroCardSettings
    {
        public GameObject prefab;
        public float horizontalDistance;
        public float verticalPosition;
        public string modelLayerName;
        public float energyRotationSpeed;
    }
    public HeroCardSettings heroCards;

    [System.Serializable]
    public class EnemiesSettings
    {
        public float appearSpeed;
        public float[] colliderWidths;
    }
    public EnemiesSettings enemies;

    [System.Serializable]
    public class DamageTextSettings
    {
        public GameObject prefab;

        [Header("Normal Damage")]
        public Color normalFillColor;
        public Color normalOutlineColor;
        public int normalFontSize;

        [Header("Weak Damage")]
        public Color weakFillColor;
        public Color weakOutlineColor;
        public int weakFontSize;

        [Header("Strong Damage")]
        public Color strongFillColor;
        public Color strongOutlineColor;
        public int strongFontSize;
    }
    public DamageTextSettings damageTextSettings;

    [System.Serializable]
    public class StagesSettings
    {
        public GameObject[] stagePanels;
        public float panelXDistance;

        public Gradient starCenterGlowRange;
        public float starCenterGlowSpeed;

        public float lockedLevelScale;
        public float unlockedLevelScale;
        public float completedLevelScale;

        public float unlockedLvColiderRadius;
        public float completedLvColiderRadius;

    }
    public StagesSettings stagesSettings;


    [System.Serializable]
    public class LevelsLists
    {
        [System.Serializable]
        public class StageSettings
        {
            public string name;
            public GameObject panelPrefab;
            public LevelSettings[] levels;
        }
        public StageSettings[] stages;
    }
    public LevelsLists levels;

}


[System.Serializable]
public class Profession
{
    public string name;
    public Sprite icon;
}