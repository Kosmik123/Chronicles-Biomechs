using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero", menuName = "Battlers/Hero")]
public class HeroTemplate : Battler
{
    [Header("Hero")]
    public int rarity;
    public int maxLevel = 100;

    [SerializeField]
    private Stat attackByLevel;
    [SerializeField]
    private Stat defenceByLevel;
    [SerializeField]
    private Stat maxHealthByLevel;

    public int energySpeed;



    public int GetMaxHealth(int lv)
    {
        return baseMaxHealth + maxHealthByLevel.Get(1.0f * lv / maxLevel);
    }

    public int GetAttack(int lv)
    {
        return baseAttack + attackByLevel.Get(1.0f * lv / maxLevel);
    }

    public int GetDefence(int lv)
    {
        return baseDefence + defenceByLevel.Get(1.0f * lv / maxLevel);
    }
}

[System.Serializable]
public class Stat
{
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public int modifier;

    public int Get(float percent)
    {
        return (int)(modifier * curve.Evaluate(percent));
    }

}





