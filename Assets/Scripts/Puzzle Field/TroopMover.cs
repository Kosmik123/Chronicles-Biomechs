using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopMover : MonoBehaviour
{
    public Troop troop;

    [Header("Sprite")]
    public SpriteRenderer[] renderers;
    public float appearSpeed;
    public float disappearSpeed;
    public bool isAppearing, isDisappearing;
    private float currentAlpha;

    [Header("Movement")]
    public float maxY;
    public bool isMoving;
    private float moveSpeed;

    void Start()
    {
        currentAlpha = 0;
        isAppearing = true;
        foreach (var rend in renderers)
            rend.color = new Color(1, 1, 1, 0);

        moveSpeed = Settings.main.troops.moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.y) > maxY)
            Disappear();

        if (isAppearing)
        {
            currentAlpha += appearSpeed * Time.deltaTime;
            if (currentAlpha >= 1)
                isAppearing = false;
        }
        else if (isDisappearing)
        {
            currentAlpha -= disappearSpeed * Time.deltaTime;
            if (currentAlpha < 0)
            {
                Destruct();
            }
        }

        UpdateTransparency();
        UpdateMove();
    }

    private void UpdateTransparency()
    {
        foreach (var rend in renderers)
            rend.color = new Color(1, 1, 1, currentAlpha);
    }

    private void UpdateMove()
    {
        if (isMoving)
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }

    public void UpdateSprite()
    {
        if(troop != null)
            foreach(var rend in renderers)
                rend.sprite = troop.sprite;
    }

    public void Disappear()
    {
        isDisappearing = true;
    }

    public void Destruct()
    {
        if (BattleData.main.heroesByElement != null && BattleData.main.heroesByElement.Length > 0)
        {
            int index = 0;
            foreach (HeroController hero in BattleData.main.heroesByElement[troop.elementId].heroes)
            {
                GameObject particle = Instantiate(Settings.main.particles.prefab,
                    transform.position, Quaternion.identity);
                particle.GetComponent<EnergyParticleController>().InitiateParticle(hero);
                index++;
            }
        }
        Destroy(gameObject);
    }



    private void OnDrawGizmos()
    {
        UpdateSprite();
    }


}
