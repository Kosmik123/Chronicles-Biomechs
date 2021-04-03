using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Settings")]
    public Enemy battler;
    public int level;
    public float flashSpeed;
    public float flash;
    public SpriteRenderer[] modelRenderers;

    [Header("To Link")]
    public GameObject characterModelObject;
    public StatusBarController hpBar;
    private DamageTextController damageTextController;

    [Header("States")]
    public bool isActive;
    public bool isAppearing;
    [SerializeField]
    private float[] originalAlphas;
    [SerializeField]
    private float appearTimer;

    [Header("Battler State")]
    public int health;
    public bool isDead;

    void Awake()
    {
        Debug.Log("Enemy Awake");
    }

    void Start()
    {
        Debug.Log("Enemy Start");

        // Collider
        BoxCollider2D collider = GetComponentInChildren<BoxCollider2D>();
        collider.size = new Vector2(
            Settings.main.enemies.colliderWidths[(int)battler.colliderSize],
            collider.size.y);

        // Damage Text Controller
        damageTextController = GetComponentInChildren<DamageTextController>();
        damageTextController.textGenerationBounds.extents = new Vector2(
            collider.size.x / 2,
            damageTextController.textGenerationBounds.extents.y);

        // Sprites
        foreach (SpriteRenderer rend in modelRenderers)
            rend.sortingLayerName = "Enemies";

        flash = 1;

        // Battler
        health = battler.GetMaxHealth(level);
        hpBar.SetValue(1.0f, true);
    
        UpdateModel();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive && isAppearing)
            UpdateAppearing();

        if (isActive)
            BattleUpdate();


    }

    public void SetBattler(LevelSettings.EnemySetting enemyStn)
    {
        battler = enemyStn.enemy;
        level = enemyStn.level;
        UpdateModel();
    }


    private void BattleUpdate()
    {
        foreach (var rend in modelRenderers)
            rend.color = new Color(1, flash, flash);

        if (flash < 1)
            flash += Time.deltaTime * flashSpeed;

        UpdateHealthBar();

        if (!isDead && hpBar.GetDisplayedValue() <= 0)
        {
            isDead = true;

            foreach (var rend in modelRenderers)
                rend.enabled = false;
            GetComponentInChildren<BoxCollider2D>().enabled = false;
            hpBar.isActive = false;
        }
    }

    private void UpdateAppearing()
    {
        appearTimer += Time.deltaTime * Settings.main.enemies.appearSpeed;
        if (appearTimer >= 1)
        {
            isActive = true;
            isAppearing = false;
            for (int i = 0; i < modelRenderers.Length; i++)
                modelRenderers[i].color = new Color(
                    modelRenderers[i].color.r,
                    modelRenderers[i].color.g,
                    modelRenderers[i].color.b,
                    originalAlphas[i]);
        }
        else
        {
            for (int i = 0; i < modelRenderers.Length; i++)
                modelRenderers[i].color = new Color(
                    modelRenderers[i].color.r,
                    modelRenderers[i].color.g,
                    modelRenderers[i].color.b,
                    Mathf.Lerp(0, originalAlphas[i], appearTimer));
        }
    }


    private void UpdateHealthBar()
    {
        hpBar.SetValue(1.0f * health / battler.GetMaxHealth(level));
    }

    private int CalculateDamage(Troop troop)
    {
        int heroAttack = BattleData.main.GetHeroesAttack(troop.elementId);    
    
        int damage = (int) (heroAttack * (1 + troop.attackBonus)) * 4;
        damage -= 2 * battler.GetDefence(level);
        damage = Mathf.Max(0, damage);

        DamageStrength type = DamageStrength.NORMAL;
        int elementCount = Settings.main.elements.Length;
        int totalDamage = damage;
        if ((troop.elementId - 1) % elementCount == battler.elementId)
        {
            type = DamageStrength.STRONG;
            totalDamage = 2 * damage;
        }
        else if ((troop.elementId + 1) % elementCount == battler.elementId)
        {
            type = DamageStrength.WEAK;
            totalDamage = damage / 2;
        }
        totalDamage += Random.Range(-damage / 3, damage / 3 + 1);

        damageTextController.ShowDamage(totalDamage, type);
        return totalDamage;
    }


    public void UpdateModel()
    {
        modelRenderers = null;
        SpriteRenderer[] children = characterModelObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var rend in children)
            DestroyImmediate(rend.gameObject);
        Debug.Log("Battler: " + battler + ", a level: " + level);
        GameObject model = Instantiate(battler.characterModel, characterModelObject.transform.position,
            Quaternion.identity, characterModelObject.transform);
        model.name = battler.name + " (Model)";

        modelRenderers = characterModelObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer rend in modelRenderers)
            rend.sortingLayerName = "Enemies";

        originalAlphas = new float[modelRenderers.Length];
        for (int i = 0; i < modelRenderers.Length; i++)
        {
            originalAlphas[i] = modelRenderers[i].color.a;
            modelRenderers[i].color = new Color(
                modelRenderers[i].color.r,
                modelRenderers[i].color.g,
                modelRenderers[i].color.b,
                0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && collision.CompareTag("Troop"))
        {
            flash = 0;
            TroopMover cont = collision.GetComponent<TroopMover>();
            int damage = CalculateDamage(cont.troop);
            health -= damage;
            cont.Destruct();
        }
    }

#if FALSE
    //UNITY_EDITOR

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
