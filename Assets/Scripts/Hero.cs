using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Hero
{
    [SerializeField]
    private HeroTemplate template;
    
    public int level;
    public int experience;

    public bool isDead;

    public int GetMaxHealth()
    {
        return template.GetMaxHealth(level);
    }
}
