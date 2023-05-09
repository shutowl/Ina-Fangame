using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class StatisticsMenu : MonoBehaviour
{
    public GameObject StageUI;
    public GameObject[] buttons;
    int index = 0;
    public float duration = 2f;
    public Sprite defaultButtonBG;
    public Sprite selectedButtonBG;

    [Header("Statistics")]
    public TextMeshProUGUI timeTakenText;
    [SerializeField] float timeTaken;
    public TextMeshProUGUI maxComboText;
    public TextMeshProUGUI timesHitText;

    RectTransform rect;
    bool menuOpen = false;
    InputActions input;
    ComboMeter comboMeter;
    PlayerMovement playerScript;

    void Start()
    {
        input = new InputActions();
        input.UI.Enable();
        rect = StageUI.GetComponent<RectTransform>();
        comboMeter = FindObjectOfType<ComboMeter>();
        playerScript = FindObjectOfType<PlayerMovement>();
        menuOpen = false;
        rect.anchoredPosition = new Vector2(0, -900f);
        StageUI.SetActive(false);
        index = 0;

        timeTaken = 0f;
        //Max combo stat grabbed from ComboMeter
        //Time hit stat grabbed from PlayerMovement
    }

    void Update()
    {
        if (menuOpen)
        {
            if (input.UI.Confirm.WasPressedThisFrame())
            {
                switch (index)
                {
                    //Retry
                    case 0:
                        RetryStage();
                        break;
                    //Back to Title
                    case 1:
                        BackToTitle();
                        break;
                }

                AudioManager.Instance.Play("MenuSelect");
            }
            if (input.UI.Cancel.WasPressedThisFrame())
            {
                //Move to Back to Title button
                buttons[0].GetComponent<Image>().sprite = defaultButtonBG;
                buttons[0].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

                buttons[1].GetComponent<Image>().sprite = selectedButtonBG;
                buttons[1].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                index = 1;

                AudioManager.Instance.Play("MenuCancel");
            }
            if (input.UI.Left.WasPressedThisFrame() || input.UI.Right.WasPressedThisFrame())
            {
                buttons[index].GetComponent<Image>().sprite = defaultButtonBG;
                buttons[index].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

                if (index == 1) index = 0;
                else index = 1;

                buttons[index].GetComponent<Image>().sprite = selectedButtonBG;
                buttons[index].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

                AudioManager.Instance.Play("MenuMove");
            }
        }
        else
        {
            timeTaken += Time.deltaTime;
        }
    }

    public void OpenStatistics()
    {
        menuOpen = true;
        rect.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutCubic);

        int minutes = (int)(timeTaken / 60);
        int seconds = (int)(timeTaken % 60);
        int ms = (int)(timeTaken * 1000 % 1000);
        timeTakenText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, ms);

        maxComboText.text = "" + comboMeter.GetMaxCombo();

        timesHitText.text = "" + playerScript.GetTimesHit();

        StageUI.SetActive(true);
    }

    void RetryStage()
    {
        PlayerPrefs.SetString("nextScene", "StageTest");
        SceneManager.LoadScene("LoadingScreen");
    }

    void BackToTitle()
    {
        PlayerPrefs.SetString("nextScene", "TitleScreen");
        SceneManager.LoadScene("LoadingScreen");
    }
}
