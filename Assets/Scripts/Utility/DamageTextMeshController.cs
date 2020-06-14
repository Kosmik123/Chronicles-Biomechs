using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextMeshController : DamageTextController
{ 
    private TextMeshProUGUI weakDamageText;
    private TextMeshProUGUI strongDamageText;

    public List<TextMeshProUGUI> damageIndicators = new List<TextMeshProUGUI>();

    // Start is called before the first frame update
    void Start()
    {
        weakDamageText = weakTextAnimation.GetComponentInChildren<TextMeshProUGUI>();
        strongDamageText = strongTextAnimation.GetComponentInChildren<TextMeshProUGUI>();

        weakDamageText.enabled = false;
        weakDamageText.faceColor = Settings.main.damageTextSettings.weakFillColor;
        weakDamageText.outlineColor = Settings.main.damageTextSettings.weakOutlineColor;

        strongDamageText.enabled = false;
        strongDamageText.faceColor = Settings.main.damageTextSettings.strongFillColor;
        strongDamageText.outlineColor = Settings.main.damageTextSettings.strongOutlineColor;

        RectTransform tf = GetComponent<RectTransform>();
        tf.localScale = 0.1f * Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = damageIndicators.Count-1; i >= 0; i--)
        {
            TextMeshProUGUI indicator = damageIndicators[i];
            if(indicator.color.a <= 0)
            {
                Destroy(indicator.transform.parent.gameObject);
                damageIndicators.RemoveAt(i);
            }
        }

        if (isWeak && weakDamageText.color.a <= 0)
        {
            weakDamageText.enabled = false;
            isWeak = false;
        }
        if (isStrong && strongDamageText.color.a <= 0)
        {
            strongDamageText.enabled = false;
            isStrong = false;
        }
    }

    public override void ShowDamage(int damage, DamageStrength type = DamageStrength.NORMAL)
    {
        Vector3 randomPosition = transform.position + new Vector3(
            Random.Range(textGenerationBounds.min.x, textGenerationBounds.max.x),
            Random.Range(textGenerationBounds.min.y, textGenerationBounds.max.x));
        GameObject indicator = Instantiate(Settings.main.damageTextSettings.prefab,
            randomPosition, Quaternion.identity, transform);

        TextMeshProUGUI damageText = indicator.GetComponentInChildren<TextMeshProUGUI>();
        damageIndicators.Add(damageText);
        damageText.text = damage.ToString();

        switch (type)
        {
            case DamageStrength.WEAK:
                damageText.fontSize = Settings.main.damageTextSettings.weakFontSize;
                damageText.faceColor = Settings.main.damageTextSettings.weakFillColor;
                damageText.outlineColor = Settings.main.damageTextSettings.weakOutlineColor;
                if (!isWeak)
                {
                    weakDamageText.enabled = true;
                    weakTextAnimation.Play();
                    isWeak = true;
                }
                break;
            case DamageStrength.STRONG:
                damageText.fontStyle = FontStyles.Bold;
                damageText.fontSize = Settings.main.damageTextSettings.strongFontSize;
                damageText.faceColor = Settings.main.damageTextSettings.strongFillColor;
                damageText.outlineColor = Settings.main.damageTextSettings.strongOutlineColor;
                if(!isStrong)
                {
                    strongDamageText.enabled = true;
                    strongTextAnimation.Play();
                    isStrong = true;
                }
                break;
            default:
                damageText.fontSize = Settings.main.damageTextSettings.normalFontSize;
                damageText.faceColor = Settings.main.damageTextSettings.normalFillColor;
                damageText.outlineColor = Settings.main.damageTextSettings.normalOutlineColor;
                break;
        }

    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + textGenerationBounds.center, textGenerationBounds.extents * 2);
    }
#endif
}

public enum DamageStrength
{
    NORMAL, WEAK, STRONG
}