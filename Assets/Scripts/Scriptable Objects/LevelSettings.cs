using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable, CreateAssetMenu(fileName = "New Level", menuName = "Puzzles/Level Settings")]
public class LevelSettings : ScriptableObject
{
    [System.Serializable]
    public class EnemySetting
    {
        public string name;
        public Enemy enemy;
        public int level;
    }

    [System.Serializable]
    public class BossSetting
    {
        [SerializeField]
        public EnemySetting bossEnemy;
        public EnemySetting helper;

        public EnemyContainer configuration;

        public int[] bossIndexes;
        public int[] helperIndexes;
    }

    [Header("General")]
    public Sprite background;
    public int numberOfBasicWaves; // Waves before final wave with boss

    [Header("Enemies")]
    public EnemySetting[] enemies;
    public EnemyContainer[] possibleConfigurations;
    public BossSetting boss;



}
