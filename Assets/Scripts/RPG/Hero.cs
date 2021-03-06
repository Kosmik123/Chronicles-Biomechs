﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Hero
{
    [SerializeField]
    private HeroTemplate template;

    public string name => template.name;
    public int level;
    public int experience;

    public bool isDead;
    public int elementId => template.elementId;

    public int GetMaxHealth()
    {
        return template.GetMaxHealth(level);
    }

    public int GetAttack()
    {
        return template.GetAttack(level);
    }

    public int GetDefence()
    {
        return template.GetDefence(level);
    }


    public Sprite GetBackground()
    {
        return Settings.main.elements[(int)template.elementId].defaultBackground;
    }

    public HeroTemplate GetTemplate()
    {
        return template;
    }

    public int CalculatePower()
    {
        return GetMaxHealth() + GetAttack() * 2 + GetDefence() + level;
    }

}

