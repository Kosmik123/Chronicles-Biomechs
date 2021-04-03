using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleViewController : MonoBehaviour
{
    [Header("To Link")]
    public SpriteRenderer background;
    Camera cam;

    public float cameraSizeScale;
    public float backgroundSizeScale;

    float cameraAspect;
    
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (cam.aspect != cameraAspect)
        {
            cameraAspect = cam.aspect;
            ResizeView();
        }
    }

    private void ResizeView()
    {
         
        float newCamSize = cameraSizeScale / cameraAspect;
        cam.orthographicSize = newCamSize;

        background.transform.localScale = backgroundSizeScale / cameraAspect * Vector3.one;
        
        
        
        
        
        
    }
}
