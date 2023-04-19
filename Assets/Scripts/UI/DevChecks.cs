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
    public bool enableFPS = true;
    public TextMeshProUGUI FPSText;

    public bool enableStageText = true;
    public TextMeshProUGUI stageText;
    private StageSpawn stageScript;

    private void Start()
    {
        stageScript = FindObjectOfType<StageSpawn>();
    }

    void Update()
    {
        if (enableFPS)
        {
            FPSText.enabled = true;
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
        else
        {
            FPSText.enabled = false;
        }

        if (enableStageText)
        {
            stageText.enabled = true;
            stageText.text = "Stage: " + stageScript.GetCurrentStage() + "\n" +
                 "Waves Left: " + stageScript.GetCurrentWave() + "\n" +
                 "Time Left: " + Mathf.Clamp(stageScript.GetTimeLeft(), 0, 100).ToString("F2");
        }
        else
        {
            stageText.enabled = false;
        }

    }
}
