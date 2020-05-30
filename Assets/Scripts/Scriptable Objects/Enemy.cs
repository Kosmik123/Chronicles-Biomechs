using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Puzzles/Battlers/Enemy")]
public class Enemy : Battler
{
    public enum EnemyColliderSize
    {
        _2or3,
        _3or4,
        _4or5,
        _5or6,
        _1or2
    }

    public EnemyColliderSize colliderSize;

    public int attack { get { return baseAttack; } }
    public int defence { get { return baseDefence; } }
    public int maxHealth { get { return baseMaxHealth; } }

    public Loot loot;
}





