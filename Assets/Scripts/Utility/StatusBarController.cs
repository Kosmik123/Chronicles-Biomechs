using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBarController : MonoBehaviour
{
    [Header("Settings")]

    [Range(0, 1)]
    public float lowValue = 0;
    public Color lowValueColor = Color.red;

    [Range(0, 1)]
    public float highValue = 1;
    public Color highValueColor = Color.green;

    public bool showAlways = false;
    public bool instantChange;

    [Range(0, 1)]
    public float riseSpeed;
    
    [Range(0, 1)]
    public float fallSpeed;


    [Header("To Link")]
    [SerializeField] 
    private SpriteRenderer hpBarBracket;
    [SerializeField] 
    private SpriteRenderer hpBarEmpty;
    [SerializeField] 
    private SpriteRenderer hpBarCurrent;
    [SerializeField] 
    private SpriteRenderer hpBarFill;

    [Header("States")]
    [SerializeField]
    private float value;
    [SerializeField]  
    private float displayedValue;


    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if (displayedValue > value + 0.005f)
            displayedValue -= fallSpeed * Time.deltaTime;
        else if (displayedValue < value - 0.005f)
            displayedValue += riseSpeed * Time.deltaTime;
        else
            displayedValue = value;

        value = Mathf.Clamp01(value);
        displayedValue = Mathf.Clamp01(displayedValue);


        if (value >= 1 && !showAlways)
        {
            hpBarBracket.enabled = hpBarEmpty.enabled = hpBarCurrent.enabled = hpBarFill.enabled = false;
        }
        else
        {
            hpBarBracket.enabled = hpBarEmpty.enabled = hpBarCurrent.enabled = hpBarFill.enabled = true;

            hpBarCurrent.transform.localScale = new Vector3(
                Mathf.Max(displayedValue, value),
                hpBarCurrent.transform.localScale.y,
                hpBarCurrent.transform.localScale.z);

            hpBarFill.transform.localScale = new Vector3(
                Mathf.Min(displayedValue, value),
                hpBarFill.transform.localScale.y,
                hpBarFill.transform.localScale.z);

            hpBarCurrent.color = GetBarColor(displayedValue, hpBarCurrent.color.a);    
            hpBarFill.color = GetBarColor(value, hpBarFill.color.a);
        }
    }

    public void SetValue(float val, bool immediate = false)
    {
        value = val;
        if (immediate)
            displayedValue = val;
    }

    public float GetDisplayedValue()
    {
        return displayedValue;
    }

    public bool IsEmpty()
    {
        return displayedValue <= 0;
    }


    public Color GetBarColor(float percent, float colorAlpha = 1f)
    {
        float range = highValue - lowValue;
        Color result = ((percent - lowValue) / range * highValueColor +
            (highValue - percent) / range * lowValueColor) * 1.8f;
        result.a = colorAlpha;
        return result;
    }



}
