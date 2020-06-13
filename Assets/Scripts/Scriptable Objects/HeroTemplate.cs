using UnityEngine;

[CreateAssetMenu(fileName = "New Hero", menuName = "Puzzles/Battlers/Hero")]
public class HeroTemplate : Battler
{
    [Header("Hero")]

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

        int lvl = Mathf.Min(lv, maxHealthByLevel.Length - 1);
        return baseMaxHealth + maxHealthByLevel[lvl];
    }

}





