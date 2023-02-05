using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private bool isNear = false;
    public float duration = 1f;
    public bool fadeIn = true;
    private float timer = 0;

    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIn)
        {
            if (isNear && timer < duration)
            {
                timer += Time.deltaTime;
            }
            else if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
        }
        else
        {
            timer = 1;
        }

        text.color = new Color(1, 1, 1, Mathf.Clamp(timer/duration, 0, 1));
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            isNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            isNear = false;
        }
    }
}
