using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    private GameObject boss;
    RectTransform rect;
    public float rectDuration = 2f;
    private float rectTimer = 0f;
    private float rectTimer2 = 0f;
    public float rectYShown;            //Y position of health bar when boss is on screen
    public float rectYHidden;           //Y position of health bar when boss is off screen


    public Slider HPSlider;
    public Slider delayHPSlider;
    public TextMeshProUGUI bossNameText;

    private float delayDuration;
    private float delayTimer = 0f;

    private ComboMeter comboMeter;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        comboMeter = FindObjectOfType<ComboMeter>();
        delayDuration = comboMeter.maxTime;

        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -100);
        rectTimer2 = 10;
    }

    void Update()
    {
        if(boss != null)
        {
            rectTimer += Time.deltaTime;
            float t = rectTimer / rectDuration;
            t = 1 - Mathf.Pow(1 - t, 3);
            float y = Mathf.Lerp(rectYHidden, rectYShown, t);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, y);   //show boss bar 

            rectTimer2 = 0f;
        }
        else
        {
            rectTimer2 += Time.deltaTime;
            float t = rectTimer2 / rectDuration;
            t = t * t * t;
            float y = Mathf.Lerp(rectYShown, rectYHidden, t);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, y); //hide boss bar
        }
        

        if(comboMeter.GetTime() <= 0)
        {
            delayTimer += Time.deltaTime;
            float t = delayTimer / delayDuration;
            t = Mathf.Sin((t * Mathf.PI) / 2);                                      //https://easings.net/#easeOutSine
            delayHPSlider.value = Mathf.Lerp(delayHPSlider.value, HPSlider.value, t);
        }
        else
        {
            delayTimer = 0;
        }

    }

    public void SetBoss(GameObject boss, float maxHealth, string name)
    {
        this.boss = boss;

        HPSlider.maxValue = maxHealth;
        HPSlider.value = maxHealth;
        delayHPSlider.maxValue = maxHealth;
        delayHPSlider.value = maxHealth;
        bossNameText.text = name;

        Debug.Log("Boss set to: " + boss);

        rectTimer = 0;
    }

    public void SetHP(float health)
    {
        HPSlider.value = health;
    }
}
