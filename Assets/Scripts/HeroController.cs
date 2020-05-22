using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public Hero hero;

    [Header("To Link")]
    public Transform healthBar;
    public Transform energyBar;

    [Header("Sprites")]
    public SpriteRenderer characterRenderer ;
    public SpriteRenderer backgroundRenderer;
    public SpriteMask spriteMask;

    [Header("States")]
    public int health;
    public int energy;

    public int battleIndex;

    // Start is called before the first frame update
    void Start()
    {
        health = hero.GetMaxHealth();

        SetSortingOrders();
    }

    private void SetSortingOrders()
    {
        backgroundRenderer.sortingOrder = battleIndex * 10 + 1;
        characterRenderer.sortingOrder = battleIndex * 10 + 2;

        spriteMask.isCustomRangeActive = true;
        spriteMask.backSortingLayerID = spriteMask.frontSortingLayerID = characterRenderer.sortingLayerID;
        spriteMask.frontSortingOrder = battleIndex * 10 + 9;
        spriteMask.backSortingOrder = battleIndex * 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
