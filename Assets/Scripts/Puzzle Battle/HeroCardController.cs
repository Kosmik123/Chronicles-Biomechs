using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCardController : MonoBehaviour
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

    public Transform energyRotator;
    private SpriteRenderer[] energyRenderers;
    public SpriteMask energyMask;

    [Header("States")]
    public int maxHealth;
    public int health;
    public int maxEnergy = 100;
    public int energy;

    //public List<State> states;

    // Start is called before the first frame update
    void Start()
    {
        energyRenderers = energyRotator.GetComponentsInChildren<SpriteRenderer>();
        foreach(var rend in energyRenderers)
            rend.enabled = false;
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
        energyRenderers = energyRotator.GetComponentsInChildren<SpriteRenderer>();
        foreach (var rend in energyRenderers)
            rend.color = Settings.main.elements[hero.elementId].cardEnergyColor;

        SetSortingOrders();
    }

    private void SetSortingOrders()
    {
        int layerCount = 100;
        cardRenderer.sortingOrder = battleIndex * layerCount;
        backgroundRenderer.sortingOrder = battleIndex * layerCount + 11;

        SpriteRenderer[] modelRenderers = model.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in modelRenderers)
        {
            renderer.sortingLayerName = Settings.main.heroCards.modelLayerName;
            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            renderer.sortingOrder = battleIndex * layerCount + 20 + renderer.sortingOrder;
        }
        spriteMask.isCustomRangeActive = true;
        spriteMask.backSortingLayerID = spriteMask.frontSortingLayerID = modelRenderers[0].sortingLayerID;
        spriteMask.frontSortingOrder = battleIndex * layerCount + 50;
        spriteMask.backSortingOrder = battleIndex * layerCount + 10;

        foreach (var rend in energyRenderers)
            rend.sortingOrder = battleIndex * layerCount + 4;
        energyMask.backSortingLayerID = energyMask.frontSortingLayerID = modelRenderers[0].sortingLayerID;
        energyMask.frontSortingOrder = battleIndex * layerCount + 5;
        energyMask.backSortingOrder = battleIndex * layerCount + 3;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStatusBars();

        if(energy >= maxEnergy)
        {
            foreach (var rend in energyRenderers)
                rend.enabled = true;
            energyRotator.rotation = Quaternion.AngleAxis(
                Settings.main.heroCards.energyRotationSpeed * Time.time,
                Vector3.forward);
        }
        else
        {
            foreach (var rend in energyRenderers)
                rend.enabled = false;
        }

    }

    private void UpdateStatusBars()
    {
        energyBar.instantChange = (energy >= maxEnergy);
        healthBar.SetValue(1f * health / maxHealth);
        energyBar.SetValue(1f * energy / maxEnergy);
    }

    public void AddEnergy()
    {
        energy += hero.GetTemplate().energySpeed;
    }

    void OnMouseUpAsButton()
    {
        if (energy >= maxEnergy)
        {
            energy = 0;
            Activate();
        }
    }

    private void Activate()
    { 
    }

}
