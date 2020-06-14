using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable, 
    CreateAssetMenu(fileName = "New Kanohi", menuName = "Puzzles/Kanohi")]
public class Kanohi : ScriptableObject
{
    public new string name;
    public Sprite sprite;
}