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
        Debug.Log("Tworzenie particli");

        ParticleSystem.MainModule main = particleSystem.main;
        //main.startColor = Settings.main.elements[elementId].color;
        
        ParticleSystem.ColorOverLifetimeModule colorModule = particleSystem.colorOverLifetime;
        colorModule.color = Settings.main.elements[elementId].particlesColor;

        startPosition = transform.position;
        targetPosition = new Vector3(0, -13);
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
