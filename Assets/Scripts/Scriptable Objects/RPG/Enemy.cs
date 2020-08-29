using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Battlers/Enemy")]
public class Enemy : Battler
{
    public enum ColliderSize
    {
        _1or2,
        _2or3,
        _3or4,
        _4or5,
        _5or6
    }

    [Header("Enemy")]
    public ColliderSize colliderSize;

    public Loot loot;
}


[System.Serializable]
public class Loot
{
    [System.Serializable]
    public class ItemLoot
    {
        public Item item;
        public float probability;
    }

    public Stat experience;
    public Stat gold;
    public ItemLoot[] items;
}

