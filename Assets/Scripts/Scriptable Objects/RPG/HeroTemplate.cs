using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero", menuName = "Battlers/Hero")]
public class HeroTemplate : Battler
{
    [Header("Hero")]
    public int rarity;

    public int energySpeed;
}






