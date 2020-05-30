using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Enemy battler;


    public GameObject characterModelObject;

    public float flashSpeed;
    private float flash;
    public SpriteRenderer[] renderers;

    [Header("Hp Bar")]
    public SpriteRenderer hpBarBracket;
    public SpriteRenderer hpBarEmpty;
    public SpriteRenderer hpBarFill;

    [Range(0, 1)]
    public float lowHealth;
    public Color lowHealthColor;

    [Range(0, 1)]
    public float highHealth;
    public Color highHealthColor;



    [Header("Battler State")]
    public int health;

    void Awake()
    {
        renderers = characterModelObject.GetComponentsInChildren<SpriteRenderer>();
    }

    void Start()
    {
        // Sprites
        foreach (SpriteRenderer rend in renderers)
            rend.sortingLayerName = "Enemies";
        flash = 1;

        // Collider
        BoxCollider2D collider = GetComponentInChildren<BoxCollider2D>();
        collider.size = new Vector2(
            Settings.main.enemies.colliderWidths[(int) battler.colliderSize],
            collider.size.y);

        // Battler
        health = battler.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

        if (flash < 1)
            flash += Time.deltaTime * flashSpeed;

        if (health >= battler.maxHealth)
        {
            hpBarBracket.enabled = hpBarEmpty.enabled = hpBarFill.enabled = false;
        } 
        else
        {
            float healthPercent = 1.0f * health / battler.maxHealth;

            hpBarBracket.enabled = hpBarEmpty.enabled = hpBarFill.enabled = true;
            hpBarFill.transform.localScale = new Vector3(
                healthPercent,
                hpBarFill.transform.localScale.y,
                hpBarFill.transform.localScale.z);

            hpBarFill.color = GetHealthBarColor(healthPercent);
        }



        foreach (var rend in renderers)
            rend.color = new Color(1, flash, flash);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Troop"))
        {
            flash = 0;
            Destroy(collision.gameObject);

            Troop troop = collision.GetComponent<TroopMover>().troop;

            int damage = CalculateDamage(troop);
            health -= damage;

            if (health < 0)
                Destroy(gameObject);
        }
    }

    private int CalculateDamage(Troop troop)
    {
        // do ogarnięcia atak bohatera
        int heroAttack = 3;
    
        int damage = (int) (heroAttack * (1 + troop.attackBonus))*4;
        damage -= 2*battler.defence;

        if (troop.elementId - 1 == battler.elementId)
            damage *= 2;
        else if (troop.elementId + 1 == battler.elementId)
            damage /= 2;

        return damage;
    }

    Color GetHealthBarColor(float percent)
    {
        float range = highHealth - lowHealth;
        return ( (percent - lowHealth) / range  * highHealthColor +
            (highHealth - percent) / range * lowHealthColor) * 1.8f;
    }

}
