using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Battlers/Enemy")]
public class Enemy : Battler
{
    public enum EnemyColliderSize
    {
        _1or2,
        _2or3,
        _3or4,
        _4or5,
        _5or6
    }

    [Header("Enemy")]
    public EnemyColliderSize colliderSize;

    public int attack { get { return baseAttack; } }
    public int defence { get { return baseDefence; } }
    public int maxHealth { get { return baseMaxHealth; } }

    public Loot loot;
}





