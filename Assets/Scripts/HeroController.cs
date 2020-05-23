using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public Hero hero;
    public int battleIndex;

    [Header("To Link")]
    public Transform healthBar;
    public Transform energyBar;

    public SpriteRenderer characterRenderer;
    public SpriteRenderer backgroundRenderer;
    public SpriteMask spriteMask;
    
    [Header("States")]
    public int health;
    public int maxHealth;
    public int energy;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth = hero.GetMaxHealth();
        energy = 0;

        SetSortingOrders();

        backgroundRenderer.sprite = hero.GetBackground();
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
        ResizeBars();
    }

    private void ResizeBars()
    {
        healthBar.localScale =
            new Vector3(1.0f * health / hero.GetMaxHealth(),
            healthBar.localScale.y, healthBar.localScale.z);
        energyBar.localScale =
                new Vector3(1.0f * energy / 100,
                energyBar.localScale.y, energyBar.localScale.z);
    }
}
