using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Battler : ScriptableObject
{
    [Header("Battler")]
    public GameObject characterModel; //prefab
    public int elementId;
    public int professionId;

    [SerializeField]
    protected int baseAttack;
    [SerializeField]
    protected int baseDefence;
    [SerializeField]
    protected int baseMaxHealth;

    public string description;

    //public Skill specialSkills;

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

    public int experience;
    public int gold;
    public ItemLoot[] items;
}
