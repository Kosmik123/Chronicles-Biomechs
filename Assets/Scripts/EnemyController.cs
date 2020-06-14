using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Enemy battler;

    public GameObject characterModelObject;

    [Header("Settings")]
    public float flashSpeed;
    public float flash;
    public SpriteRenderer[] renderers;

    [Header("To Link")]
    public StatusBarController hpBar;
    private DamageTextController damageTextController;

    [Header("Battler State")]
    public int health;

    void Awake()
    {
        UpdateModel();

    }

    void Start()
    {
        damageTextController = GetComponentInChildren<DamageTextController>();

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
        hpBar.SetValue(health, true);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var rend in renderers)
            rend.color = new Color(1, flash, flash);

        if (flash < 1)
            flash += Time.deltaTime * flashSpeed;

        UpdateHealthBar();

        if (hpBar.GetDisplayedValue() <= 0)
            Destroy(gameObject);
    }

    private void UpdateHealthBar()
    {
        hpBar.SetValue(1.0f * health / battler.maxHealth);
    }

    private int CalculateDamage(Troop troop)
    {
        int heroAttack = BattleData.main.GetHeroesAttack(troop.elementId);    
    
        int damage = (int) (heroAttack * (1 + troop.attackBonus)) * 4;
        damage -= 2 * battler.defence;

        DamageStrength type = DamageStrength.NORMAL;
        if (troop.elementId - 1 == battler.elementId)
        {
            type = DamageStrength.STRONG;
            damage *= 2;
        }
        else if (troop.elementId + 1 == battler.elementId)
        {
            type = DamageStrength.WEAK;
            damage /= 2;
        }
        damage += Random.Range(-damage / 3, damage / 3 + 1);

        damageTextController.ShowDamage(damage, type);
        return damage;
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
