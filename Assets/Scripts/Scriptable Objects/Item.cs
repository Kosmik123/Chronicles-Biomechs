using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{    
    [Header("General")]
    public string name;
    public Sprite sprite;

    [Header("Effects")]
    public int healthRecovery;
    public int energyRecovery;

    public int attackBonus;
    public int defenceBonus;
    public int healthBonus;

    public Effect effect; 



}


public class Effect : ScriptableObject
{

}