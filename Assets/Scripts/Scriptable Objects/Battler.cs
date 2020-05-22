using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Battler : ScriptableObject
{
    public GameObject characterModel; //prefab
    public int elementId;
    public int professionId;

    [SerializeField]
    protected int baseAttack;
    [SerializeField]
    protected int baseDefence;
    [SerializeField]
    protected int baseMaxHealth;


    
    //public Skill specialSkills;

}

[System.Serializable]
public class Loot
{
    [System.Serializable]
    public class ItemLoot
    {
        public Object item; // TODO zmiana object na klasę Item 
        public float probability;
    }

    public int experience;
    public int gold;
    public ItemLoot[] items;
}



[CreateAssetMenu(fileName = "New Enemy", menuName = "Puzzles/Battlers/Enemy")]
public class Enemy : Battler
{
    public enum EnemyColliderSize
    {
        _2or3,
        _3or4,
        _4or5,
        _5or6
    }

    public EnemyColliderSize colliderSize;

    public int attack { get { return baseAttack; } }
    public int defence { get { return baseDefence; } }
    public int maxHealth { get { return baseMaxHealth; } }

    public Loot loot;



}


[CreateAssetMenu(fileName = "New Hero", menuName = "Puzzles/Battlers/Hero")]
public class HeroTemplate : Battler
{
    public int rarity;


    [SerializeField]
    private int[] attackByLevel;
    [SerializeField]
    private int[] defenceByLevel;
    [SerializeField]
    private int[] maxHealthByLevel;


    public int GetMaxHealth(int lv)
    {
        if (maxHealthByLevel == null || maxHealthByLevel.Length == 0)
            return baseMaxHealth;

        lv = Mathf.Min(lv, maxHealthByLevel.Length - 1);
        return baseMaxHealth + maxHealthByLevel[lv];
    }

}





