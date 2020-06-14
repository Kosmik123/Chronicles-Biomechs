using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public abstract class DamageTextController : MonoBehaviour
{
    [Header("To Link")]
    public Animation weakTextAnimation;
    public Animation strongTextAnimation;

    [Header("Settings")]
    public Bounds textGenerationBounds;

    [Header("States")]
    public bool isWeak;
    public bool isStrong;

    abstract public void ShowDamage(int damage, DamageStrength type);

}



public class DamageUITextController : DamageTextController
{ 
    private Text weakDamageText;
    private Text strongDamageText;

    public List<Text> damageIndicators = new List<Text>();

    // Start is called before the first frame update
    void Start()
    {
        weakDamageText = weakTextAnimation.GetComponentInChildren<Text>();
        strongDamageText = strongTextAnimation.GetComponentInChildren<Text>();

        weakDamageText.enabled = false;
        weakDamageText.color = Settings.main.damageTextSettings.weakFillColor;
        weakDamageText.GetComponent<Outline>().effectColor = 
            Settings.main.damageTextSettings.weakOutlineColor;

        strongDamageText.enabled = false;
        strongDamageText.color = Settings.main.damageTextSettings.strongFillColor;
        strongDamageText.GetComponent<Outline>().effectColor =
            Settings.main.damageTextSettings.strongOutlineColor;

        RectTransform tf = GetComponent<RectTransform>();
        tf.localScale = 0.1f * Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = damageIndicators.Count-1; i >= 0; i--)
        {
            Text indicator = damageIndicators[i];
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

        Text damageText = indicator.GetComponentInChildren<Text>();
        Outline outline = indicator.GetComponentInChildren<Outline>();
        damageIndicators.Add(damageText);
        damageText.text = damage.ToString();

        switch (type)
        {
            case DamageStrength.WEAK:
                damageText.fontSize = Settings.main.damageTextSettings.weakFontSize;
                damageText.color = Settings.main.damageTextSettings.weakFillColor;
                outline.effectColor = Settings.main.damageTextSettings.weakOutlineColor;
                if (!isWeak)
                {
                    weakDamageText.enabled = true;
                    weakTextAnimation.Play();
                    isWeak = true;
                }
                break;
            case DamageStrength.STRONG:
                damageText.fontStyle = FontStyle.Bold;
                damageText.fontSize = Settings.main.damageTextSettings.strongFontSize;
                damageText.color = Settings.main.damageTextSettings.strongFillColor;
                outline.effectColor = Settings.main.damageTextSettings.strongOutlineColor;
                if(!isStrong)
                {
                    strongDamageText.enabled = true;
                    strongTextAnimation.Play();
                    isStrong = true;
                }
                break;
            default:
                damageText.fontSize = Settings.main.damageTextSettings.normalFontSize;
                damageText.color = Settings.main.damageTextSettings.normalFillColor;
                outline.effectColor = Settings.main.damageTextSettings.normalOutlineColor;
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
