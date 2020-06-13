using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public Hero hero;
    public int battleIndex;

    [Header("To Link")]
    public StatusBarController healthBar;
    public StatusBarController energyBar;

    public SpriteRenderer cardRenderer;
    public GameObject model;
    public SpriteRenderer backgroundRenderer;
    public SpriteMask spriteMask;

    [Header("States")]
    public int health;
    public int maxHealth;
    public int energy;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void Initialize(int index, Hero hero)
    {
        battleIndex = index;
        this.hero = hero;

        health = maxHealth = hero.GetMaxHealth();
        energy = 0;

        healthBar.SetValue(1, true);
        energyBar.SetValue(0, true);
 
        Instantiate(hero.GetTemplate().characterModel, model.transform.position,
            Quaternion.identity, model.transform);
        backgroundRenderer.sprite = hero.GetBackground();
        cardRenderer.color = Settings.main.elements[hero.elementId].cardColor;

        SetSortingOrders();
    }

    private void SetSortingOrders()
    {
        backgroundRenderer.sortingOrder = battleIndex * 10 + 1;

        SpriteRenderer[] modelRenderers = model.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in modelRenderers)
        {
            renderer.sortingLayerName = Settings.main.heroCards.modelLayerName;
            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            renderer.sortingOrder = battleIndex * 10 + 2 + renderer.sortingOrder;
        }
        spriteMask.isCustomRangeActive = true;
        spriteMask.backSortingLayerID = spriteMask.frontSortingLayerID = modelRenderers[0].sortingLayerID;
        spriteMask.frontSortingOrder = battleIndex * 10 + 9;
        spriteMask.backSortingOrder = battleIndex * 10;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStatusBars();
    }

    private void UpdateStatusBars()
    {
        healthBar.SetValue(1.0f * health / maxHealth);
        energyBar.SetValue(energy / 40f);
    }
}
