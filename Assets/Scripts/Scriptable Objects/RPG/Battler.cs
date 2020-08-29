using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Battler : ScriptableObject
{
    [Header("Battler")]
    public GameObject characterModel; //prefab
    public int elementId;
    public int professionId;
    public int maxLevel = 100;
    [SerializeField]
    protected int baseAttack;
    [SerializeField]
    protected int baseDefence;
    [SerializeField]
    protected int baseMaxHealth;

    [SerializeField]
    private Stat attackByLevel;
    [SerializeField]
    private Stat defenceByLevel;
    [SerializeField]
    private Stat maxHealthByLevel;

    public string description;

    public Skill specialSkills;

    private void OnValidate()
    {
        if (elementId < Settings.GetImmediate().elements.Length && elementId > -1)
            description = Settings.GetImmediate().elements[elementId].name;
        else
            description = "Unknown";
        description += " ";
        if (professionId < Settings.GetImmediate().professions.Length && professionId > -1)
            description += Settings.GetImmediate().professions[professionId].name;
        else
            description += "Nobody";
    }

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

public class Skill
{

}

