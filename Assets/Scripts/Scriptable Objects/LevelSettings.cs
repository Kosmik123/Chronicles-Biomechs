using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Level", menuName = "Puzzles/Level")]
public class LevelSettings : ScriptableObject
{
    [System.Serializable]
    public class EnemySetting
    {
        public Enemy enemy;
        public int level;
    }

    [Header("General")]
    public Sprite background;

    [Header("Enemies")]
    public EnemySetting[] enemies;    


}
