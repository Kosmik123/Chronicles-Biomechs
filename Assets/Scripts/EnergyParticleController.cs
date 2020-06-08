using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyParticleController : MonoBehaviour
{
    private new ParticleSystem particleSystem;
    
    [Header("Settings")]
    public int elementId;
    public HeroController targetHero;
    
    [Header("Movement")]
    public bool isMoving;
    public float moveSpeed;
    public Vector3 targetPosition, startPosition;
    private float moveProgress;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        ParticleSystem.ColorOverLifetimeModule colorModule = particleSystem.colorOverLifetime;
        colorModule.color = Settings.main.elements[elementId].particlesColor;
    }

    public void InitiateParticle(HeroController target)
    {
        targetHero = target;
        elementId = target.hero.elementId;
        UpdateColor();

        startPosition = transform.position;
        targetPosition = BattleData.main.GetHeroCardPositionByIndex(target.battleIndex);
        isMoving = true;
    }




    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            if(moveProgress < 1)
            {
                moveProgress += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPosition, targetPosition, moveProgress);
            }
            else
            {
                transform.position = targetPosition;
                isMoving = false;
                Destruct();
            }
        }
    }

    void Destruct()
    {
        targetHero.energy++;
        Destroy(gameObject);
    }


}
