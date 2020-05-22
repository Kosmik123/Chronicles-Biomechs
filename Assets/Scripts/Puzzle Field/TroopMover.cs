using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopMover : MonoBehaviour
{
    public Troop troop;

    [Header("Sprite")]
    public SpriteRenderer[] renderers;
    public float appearSpeed;

    [Header("Movement")]
    public float maxY;
    public bool isMoving;
    private float moveSpeed;

    void Start()
    {
        foreach (var rend in renderers)
            rend.color = new Color(1, 1, 1, 0);

        moveSpeed = Settings.main.troops.moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var rend in renderers)
            rend.color = new Color(1, 1, 1, rend.color.a + appearSpeed * Time.deltaTime);

        if (isMoving)
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        if (Mathf.Abs(transform.position.y) > maxY)
            Destroy(gameObject);
    }

    public void UpdateSprite()
    {
        if(troop != null)
            foreach(var rend in renderers)
                rend.sprite = troop.sprite;
    }












    private void OnDrawGizmos()
    {
        UpdateSprite();
    }


}
