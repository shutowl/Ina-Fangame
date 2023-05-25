using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerSanity : MonoBehaviour
{
    public float maxSanity;
    float curSanity;
    public float regenRate = 0.05f;
    public float skillAmount;               //Amount of sanity required to use skill (May change depending on skill later on?)
    PlayerMovement player;

    public TextMeshProUGUI sanityText;
    public Slider sanitySlider;
    float timer = 0f;
    public float consumeDuration;
    bool consuming;

    InputActions input;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerMovement>();
        curSanity = maxSanity;
        sanitySlider.value = sanitySlider.maxValue = maxSanity;
        sanityText.text = curSanity + "/" + maxSanity;
        consuming = false;

        input = new InputActions();
        input.Player.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (input.Player.DefenseSkill.WasPressedThisFrame())  //Debug: Use Skill
        {
            if(curSanity > skillAmount && !consuming)
            {
                UseDefenseSkill();
            }
        }

        if (!consuming && !player.IsPaused())
        {
            curSanity = sanitySlider.value = Mathf.Clamp(curSanity + regenRate, 0, maxSanity);
            sanityText.text = curSanity.ToString("F0") + "/" + maxSanity;
        }
    }

    public void UseDefenseSkill()
    {
        player.StartDefenseSkill();
        StartCoroutine(ConsumeSanity(50f));
    }

    //Decreases sanity by a certain amount
    IEnumerator ConsumeSanity(float amount)
    {
        consuming = true;

        float start = curSanity;
        float end = Mathf.Clamp(curSanity - amount, 0, maxSanity);
        timer = 0;

        while(timer < consumeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / consumeDuration;
            t = Mathf.Sin((t * Mathf.PI) / 2);                                      //https://easings.net/#easeOutSine
            curSanity = sanitySlider.value = Mathf.Lerp(start, end, t);
            sanityText.text = curSanity.ToString("F0") + "/" + maxSanity;
            yield return null;
        }

        curSanity = sanitySlider.value = end;
        sanityText.text = curSanity.ToString("F0") + "/" + maxSanity;

        consuming = false;
    }
}
