using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ComboMeter : MonoBehaviour
{
    public GameObject comboMeter;
    public Slider comboSlider;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI comboStopText;
    private bool stopTime = false;
    RectTransform rect;
    public float maxTime = 10f;
    private int hitCount = 0;
    private float timer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        stopTime = false;
        comboSlider.maxValue = maxTime;
        rect = GetComponent<RectTransform>();
        comboStopText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if(timer <= 0)
        {
            rect.eulerAngles = new Vector2(0, 90);
            ResetCombo();
        }
        else
        {
            if(!stopTime)
                timer -= Time.deltaTime;
        }

        comboSlider.value = timer;

    }

    public void AddCombo()
    {
        rect.eulerAngles = Vector2.zero;
        timer = maxTime;
        hitCount++;

        comboText.text = hitCount + " HITS!";
    }

    public void ResetCombo()
    {
        hitCount = 0;
        timer = 0;
    }

    public void SetStop(bool stop)
    {
        stopTime = stop;

        comboStopText.text = (stop) ? "Pause!" : "";
    }
}
