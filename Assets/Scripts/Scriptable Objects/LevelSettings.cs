using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
