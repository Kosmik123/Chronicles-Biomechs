using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(
    fileName = "New Kanohi", menuName = "RPG/Kanohi")]
public class Kanohi : ScriptableObject
{
    public new string name;
    public Sprite sprite;

    public Effect effect;
}