using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Enemy battler;


    public GameObject characterModelObject;

    public float flashSpeed;
    public float flash;
    public SpriteRenderer[] renderers;

    [Header("Hp Bar")]
    public SpriteRenderer hpBarBracket;
    public SpriteRenderer hpBarEmpty;
    public SpriteRenderer hpBarCurrent;
    public SpriteRenderer hpBarFill;
    private float displayedHealth;

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

        UpdateModel();
    }

    void Start()
    {
        // Collider
        BoxCollider2D collider = GetComponentInChildren<BoxCollider2D>();
        collider.size = new Vector2(
            Settings.main.enemies.colliderWidths[(int)battler.colliderSize],
            collider.size.y);

        // Sprites
        foreach (SpriteRenderer rend in renderers)
            rend.sortingLayerName = "Enemies";
        flash = 1;

        // Battler
        health = battler.maxHealth;
        displayedHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var rend in renderers)
            rend.color = new Color(1, flash, flash);

        if (flash < 1)
            flash += Time.deltaTime * flashSpeed;

        if (displayedHealth > health)
            displayedHealth--;
        else if (displayedHealth < health)
            displayedHealth++;

        UpdateHealthBar();

        if (displayedHealth <= 0)
            Destroy(gameObject);
    }

    private void UpdateHealthBar()
    {
        if (health >= battler.maxHealth)
        {
            hpBarBracket.enabled = hpBarEmpty.enabled = hpBarCurrent.enabled = hpBarFill.enabled = false;
        }
        else 
        {
            float realPercent = Mathf.Max(0, 1.0f * health / battler.maxHealth);
            float displayPercent = Mathf.Max(0, 1.0f * displayedHealth / battler.maxHealth);

            hpBarBracket.enabled = hpBarEmpty.enabled = hpBarCurrent.enabled = hpBarFill.enabled = true;

            hpBarCurrent.transform.localScale = new Vector3(
                displayPercent,
                hpBarCurrent.transform.localScale.y,
                hpBarCurrent.transform.localScale.z);

            hpBarFill.transform.localScale = new Vector3(
                realPercent,
                hpBarFill.transform.localScale.y,
                hpBarFill.transform.localScale.z);

            hpBarCurrent.color = GetHealthBarColor(displayPercent, hpBarCurrent.color.a);
            hpBarFill.color = GetHealthBarColor(realPercent, hpBarFill.color.a);
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

    Color GetHealthBarColor(float percent, float colorAlpha = 1f)
    {
        float range = highHealth - lowHealth;
        Color result = ((percent - lowHealth) / range * highHealthColor +
            (highHealth - percent) / range * lowHealthColor) * 1.8f;
        result.a = colorAlpha;
        return result;
    }


    public void UpdateModel()
    {
        renderers = null;
        SpriteRenderer[] children = characterModelObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var rend in children)
            DestroyImmediate(rend.gameObject);

        GameObject model = Instantiate(battler.characterModel, characterModelObject.transform.position,
            Quaternion.identity, characterModelObject.transform);
        model.name = battler.name + " (Model)";

        renderers = characterModelObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer rend in renderers)
            rend.sortingLayerName = "Enemies";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Troop"))
        {
            flash = 0;
            TroopMover cont = collision.GetComponent<TroopMover>();
            int damage = CalculateDamage(cont.troop);
            health -= damage;
            cont.Destruct();
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(EnemyController))]
    public class EnemyControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Update Model"))
            {
                EnemyController cont = target as EnemyController;
                cont.UpdateModel();
            }
 
        }
    }
#endif
}
