using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyParticleController : MonoBehaviour
{
    private new ParticleSystem particleSystem;
    
    [Header("Settings")]
    public int elementId;
    
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
        ParticleSystem.ColorOverLifetimeModule colorModule = particleSystem.colorOverLifetime;
        colorModule.color = Settings.main.elements[elementId].particlesColor;

    }

    public void InitiateParticle(int elementId)
    {
        this.elementId = elementId;
        startPosition = transform.position;
        targetPosition = BattleController.main.GetHeroCardPositions(elementId);
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
                Destroy(gameObject);
            }
        }
    }




}
