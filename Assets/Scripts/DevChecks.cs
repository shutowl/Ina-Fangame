using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DevChecks : MonoBehaviour
{
    private float avgFPS;
    public float updateRate = 0.5f;
    private float updateRateTimer = 0f;
    public TextMeshProUGUI FPSText;

    void Update()
    {
        if (updateRateTimer <= updateRate)
        {
            updateRateTimer += Time.deltaTime;
        }
        else
        {
            avgFPS = (int)(1f / Time.unscaledDeltaTime);
            FPSText.text = "FPS: " + avgFPS;

            updateRateTimer = 0f;
        }
    }
}
