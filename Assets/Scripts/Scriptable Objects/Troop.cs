using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Troop", menuName = "Puzzles/Troop")]
public class Troop : ScriptableObject
{
    public Sprite sprite;
    public int rarity;
    public int elementId;

    public float attackBonus;
    public float defenceBonus;
    public float healthBonus;
}
