using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBarController : MonoBehaviour
{
    [Header("Settings")]

    public Gradient colorByValueGradient;

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
    private SpriteRenderer barCurrent;
    [SerializeField] 
    private SpriteRenderer barFill;

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
            hpBarBracket.enabled = hpBarEmpty.enabled = barCurrent.enabled = barFill.enabled = false;
        }
        else
        {
            hpBarBracket.enabled = hpBarEmpty.enabled = barCurrent.enabled = barFill.enabled = true;

            barCurrent.transform.localScale = new Vector3(
                Mathf.Max(displayedValue, value),
                barCurrent.transform.localScale.y,
                barCurrent.transform.localScale.z);

            float fillPercent = instantChange ?
            value :
            Mathf.Min(displayedValue, value);
           
            barFill.transform.localScale = new Vector3(
                fillPercent,
                barFill.transform.localScale.y,
                barFill.transform.localScale.z);

            Color currentColor = colorByValueGradient.Evaluate(displayedValue);
            currentColor.a = barCurrent.color.a;
            barCurrent.color = currentColor;
            
            Color fillColor = colorByValueGradient.Evaluate(value);
            fillColor.a = barFill.color.a;
            barFill.color = fillColor;
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
}
