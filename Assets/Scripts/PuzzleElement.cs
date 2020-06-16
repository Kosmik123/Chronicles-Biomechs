using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzleElement
{
    public string name;
    public Sprite maskSprite;

    [Header("Colors")]
    public Color color;
    public Color secondaryColor;
    public Color maskColor;
    public Color cardColor;
    public Color cardEnergyColor;
    public Color particlesColor;

    public Sprite defaultBackground;

}