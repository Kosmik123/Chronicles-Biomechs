using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBarController : MonoBehaviour
{
    [Header("Settings")]
    public Gradient colorByValueGradient;
    public bool showAlways = false;
    public bool instantChange;

    public float riseSpeed;
    public float fallSpeed;

    [Header("Pulse")]
    public bool pulsingAtFull;
    public float pulseSpeed;
    public Gradient pulseGradient;

    [Header("To Link")]
    [SerializeField] 
    private SpriteRenderer barCurrent;
    [SerializeField] 
    private SpriteRenderer barFill;
    [SerializeField] 
    private SpriteRenderer[] otherRenderers;

    [Header("States")]
    [SerializeField]
    private float value;
    [SerializeField]  
    private float displayedValue;

    public bool isActive = true;

    void Start()
    {
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
            UpdateValues();

        float lowerValue = Mathf.Min(displayedValue, value);
        float higherValue = Mathf.Max(displayedValue, value);
    

        if (!showAlways && value >= 1)
        {
            foreach(var rend in otherRenderers)
                rend.enabled = false;
            barCurrent.enabled = barFill.enabled = false;
        }
        else
        {
            foreach (var rend in otherRenderers)
                rend.enabled = isActive;
            barCurrent.enabled = barFill.enabled = isActive;

            if (isActive)
            {
                barCurrent.transform.localScale = new Vector3(
                  higherValue,
                  barCurrent.transform.localScale.y,
                  barCurrent.transform.localScale.z);

                float fillPercent = instantChange ? value : lowerValue;
                barFill.transform.localScale = new Vector3(
                    fillPercent,
                    barFill.transform.localScale.y,
                    barFill.transform.localScale.z);

                Color currentColor = colorByValueGradient.Evaluate(higherValue);
                currentColor.a = barCurrent.color.a;
                barCurrent.color = currentColor;

                Color fillColor = (pulsingAtFull && value >= 1) ?
                    pulseGradient.Evaluate((Mathf.Sin(Time.time * pulseSpeed) + 1) / 2) :
                    colorByValueGradient.Evaluate(lowerValue);

                fillColor.a = barFill.color.a;
                barFill.color = fillColor;
            }
        }
    }

    private void UpdateValues()
    {
        if (displayedValue > value + 0.005f)
            displayedValue -= (displayedValue - value + 0.01f) * fallSpeed * Time.deltaTime;
        else if (displayedValue < value - 0.005f)
            displayedValue += (value - displayedValue + 0.01f) * riseSpeed * Time.deltaTime;
        else
            displayedValue = value;

        value = Mathf.Clamp01(value);
        displayedValue = Mathf.Clamp01(displayedValue);
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
