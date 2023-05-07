using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public enum menuState
    {
        main,
        stage,
        difficulty,
        options
    }
    public menuState state;


    public Sprite selectedButtonBG;
    public Sprite defaultButtonBG;
    public GameObject[] mainMenuButtons;
    private int menuIndex = 0;

    [Header("Difficulty Variables")]
    public GameObject difficultyBox;
    public RectTransform difficultyBG;
    public float difficultyOpenDuration = 0.2f;
    public GameObject[] difficultyText;
    public TextMeshProUGUI difficultyDescription;
    private int difficultyLevel = 0;

    [Header("Options Variables")]
    public GameObject optionsBox;
    public RectTransform optionsBG;
    public float optionsOpenDuration = 0.2f;
    private bool optionBoxIsMoving = false;
    public GameObject[] optionsLeftText;
    public GameObject[] optionsRightText;
    public bool option1 = true;
    public bool option2 = true;
    public bool option3 = false;
    public int resolution = 0;
    public int fullscreen = 0;
    public int masterVol = 100;
    public int BGMVol = 100;
    public int SFXVol = 100;
    bool changeVol = false;
    bool volIncrease = false;
    int volIndex = 0;

    [Header("Stage Select Variables")]
    public GameObject stageSelectBox;
    public GameObject[] row1Stages;
    public GameObject[] row2Stages;
    private int stageRow = 0;
    private int stageCol = 0;
    public TextMeshProUGUI[] stageStats;

    public GameObject WIPText;

    private InputActions input;

    void Awake()
    {
        input = new InputActions();
        input.UI.Enable();
        state = menuState.main;
        optionsBox.SetActive(false);
        difficultyBox.SetActive(false);
        stageSelectBox.SetActive(false);
        Time.timeScale = 1;

        //Set Default Values
        PlayerPrefs.GetInt("option1", 1);
        PlayerPrefs.GetInt("option2", 1);
        PlayerPrefs.GetInt("option3", 1);
        PlayerPrefs.GetInt("mVol", 70);
        PlayerPrefs.GetInt("bgmVol", 70);
        PlayerPrefs.GetInt("sfxVol", 70);
    }

    void Update()
    {
        if (input.UI.Confirm.WasPressedThisFrame())
        {
            if (state == menuState.options)
            {
                switch (menuIndex)
                {
                    case 0:
                        option1 = !option1;
                        optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().text = (option1) ? "Yes" : "No";
                        break;
                    case 1:
                        option2 = !option2;
                        optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().text = (option2) ? "Yes" : "No";
                        break;
                    case 2:
                        option3 = !option3;
                        optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().text = (option3) ? "Yes" : "No";
                        break;
                }

                AudioManager.Instance.Play("MenuMove");
            }
            else if(state == menuState.difficulty)
            {
                SelectStage(stageRow, stageCol);
                AudioManager.Instance.Play("MenuSelect");
            }
            else if (state == menuState.main)
            {
                switch (menuIndex)
                {
                    case 0:
                        OpenStageSelect();
                        AudioManager.Instance.Play("MenuSelect");
                        break;
                    case 1:
                        OpenTutorial();
                        AudioManager.Instance.Play("MenuError");
                        break;
                    case 2:
                        if (!optionBoxIsMoving)
                        {
                            OpenOptions();
                            AudioManager.Instance.Play("MenuSelect");
                        }
                        break;
                    case 3:
                        OpenCredits();
                        AudioManager.Instance.Play("MenuError");
                        break;
                    case 4:
                        ExitGame();
                        AudioManager.Instance.Play("MenuSelect");
                        break;
                }
            }
            else if(state == menuState.stage)
            {
                if (stageRow == 0 && stageCol == 1)     //Only Gura stage is working for now
                {
                    OpenDifficulty();
                    AudioManager.Instance.Play("MenuSelect");
                }
                else
                {
                    ShowWIPText();
                    AudioManager.Instance.Play("MenuError");
                    Debug.Log("Stage still WIP!");
                    //Play an error sound or smth
                }
            }
        }
        if (input.UI.Cancel.WasPressedThisFrame())
        {
            if (state == menuState.main)
            {
                mainMenuButtons[menuIndex].GetComponent<Image>().sprite = defaultButtonBG;
                mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                menuIndex = 3; //Moves to exit
                mainMenuButtons[menuIndex].GetComponent<Image>().sprite = selectedButtonBG;
                mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

                AudioManager.Instance.Play("MenuCancel");
            }
            if (state == menuState.options)
            {
                if (!optionBoxIsMoving)
                {
                    CloseOptions();
                    AudioManager.Instance.Play("MenuCancel");
                }
            }
            if(state == menuState.stage)
            {
                CloseStageSelect();
                AudioManager.Instance.Play("MenuCancel");
            }
            if(state == menuState.difficulty)
            {
                CloseDifficulty();
                AudioManager.Instance.Play("MenuCancel");
            }
        }
        if (input.UI.Up.WasPressedThisFrame())
        {
            if(state == menuState.main)
            {
                mainMenuButtons[menuIndex].GetComponent<Image>().sprite = defaultButtonBG;
                mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                menuIndex = Mathf.Clamp(--menuIndex, 0, mainMenuButtons.Length);
                mainMenuButtons[menuIndex].GetComponent<Image>().sprite = selectedButtonBG;
                mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

                AudioManager.Instance.Play("MenuMove");
            }
            if(state == menuState.options)
            {
                optionsLeftText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
                optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
                menuIndex = Mathf.Clamp(--menuIndex, 0, 7);
                optionsLeftText[menuIndex].GetComponent<TextMeshProUGUI>().color = Color.white;
                optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().color = Color.white;

                if(changeVol) changeVol = false;

                AudioManager.Instance.Play("MenuMove");
            }
            if(state == menuState.difficulty)
            {
                Color color = difficultyText[menuIndex].GetComponent<TextMeshProUGUI>().color;
                difficultyText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, 0.5f);
                menuIndex = Mathf.Clamp(--menuIndex, 0, 3);
                difficultyLevel = Mathf.Clamp(--difficultyLevel, 0, 3);
                color = difficultyText[menuIndex].GetComponent<TextMeshProUGUI>().color;
                difficultyText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, 1f);

                switch (menuIndex)
                {
                    case 0:
                        difficultyDescription.text = "Taking it easy!\nRecommended for players who are new to action and bullet hell games. " +
                            "Enemies will attack slower and fire less projectiles than normal.";
                        break;
                    case 1:
                        difficultyDescription.text = "The intended difficulty!\nRecommended for players experienced with action and bullet hell " +
                            "games.";
                        break;
                    case 2:
                        difficultyDescription.text = "Getting tougher!\nFor tough players looking for a challenge. " +
                            "Enemy attack patterns and speed are buffed!";
                        break;
                    case 3:
                        difficultyDescription.text = "Take at your own risk...";
                        break;
                }

                AudioManager.Instance.Play("MenuMove");
            }
            if (state == menuState.stage)
            {
                switch (stageRow)
                {
                    case 0:
                        //Does nothing
                        break;
                    case 1:
                        row2Stages[stageCol].GetComponent<Image>().sprite = defaultButtonBG;
                        row2Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                        stageRow = Mathf.Clamp(--stageRow, 0, 1);
                        stageCol = Mathf.Clamp(stageCol, 0, 4);
                        row1Stages[stageCol].GetComponent<Image>().sprite = selectedButtonBG;
                        row1Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                        break;
                    case 2:
                        //in case row 3 exists in the future
                        break;
                }
                UpdateStageInfo(stageRow, stageCol);

                AudioManager.Instance.Play("MenuMove");
            }
        }
        if (input.UI.Down.WasPressedThisFrame())
        {
            if (state == menuState.main)
            {
                mainMenuButtons[menuIndex].GetComponent<Image>().sprite = defaultButtonBG;
                mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                menuIndex = Mathf.Clamp(++menuIndex, 0, mainMenuButtons.Length-1);
                mainMenuButtons[menuIndex].GetComponent<Image>().sprite = selectedButtonBG;
                mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

                AudioManager.Instance.Play("MenuMove");
            }
            if (state == menuState.options)
            {
                optionsLeftText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
                optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
                menuIndex = Mathf.Clamp(++menuIndex, 0, 7);
                optionsLeftText[menuIndex].GetComponent<TextMeshProUGUI>().color = Color.white;
                optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().color = Color.white;

                if(changeVol) changeVol = false;

                AudioManager.Instance.Play("MenuMove");
            }
            if (state == menuState.difficulty)
            {
                Color color = difficultyText[menuIndex].GetComponent<TextMeshProUGUI>().color;
                difficultyText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, 0.5f);
                menuIndex = Mathf.Clamp(++menuIndex, 0, 3);
                difficultyLevel = Mathf.Clamp(++difficultyLevel, 0, 3);
                color = difficultyText[menuIndex].GetComponent<TextMeshProUGUI>().color;
                difficultyText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, 1f);

                switch (menuIndex)
                {
                    case 0:
                        difficultyDescription.text = "Taking it easy!\nRecommended for players who are new to action and bullet hell games. " +
                            "Enemies will attack slower and fire less projectiles than normal.";
                        break;
                    case 1:
                        difficultyDescription.text = "The intended difficulty!\nRecommended for players experienced with action and bullet hell " +
                            "games.";
                        break;
                    case 2:
                        difficultyDescription.text = "Getting tougher!\nFor tough players looking for a challenge. " +
                            "Enemy attack patterns and speed are buffed!";
                        break;
                    case 3:
                        difficultyDescription.text = "Take at your own risk...";
                        break;
                }

                AudioManager.Instance.Play("MenuMove");
            }
            if(state == menuState.stage)
            {
                switch (stageRow)
                {
                    case 0:
                        row1Stages[stageCol].GetComponent<Image>().sprite = defaultButtonBG;
                        row1Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                        stageRow = Mathf.Clamp(++stageRow, 0, 1);
                        stageCol = Mathf.Clamp(stageCol, 0, 5);
                        row2Stages[stageCol].GetComponent<Image>().sprite = selectedButtonBG;
                        row2Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                        break;
                    case 1:
                        //Move to row 3 if more are added later
                        break;
                }
                UpdateStageInfo(stageRow, stageCol);

                AudioManager.Instance.Play("MenuMove");
            }
        }
        if (input.UI.Left.WasPressedThisFrame())
        {
            if (state == menuState.options)
            {
                switch (menuIndex)
                {
                    case 0: //Option 1
                        option1 = !option1;
                        optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().text = (option1) ? "Yes" : "No";
                        break;
                    case 1: //Option 2
                        option2 = !option2;
                        optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().text = (option2) ? "Yes" : "No";
                        break;
                    case 2: //Option 3
                        option3 = !option3;
                        optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().text = (option3) ? "Yes" : "No";
                        break;
                    case 3: //Resolution

                        break;
                    case 4: //Display

                        break;
                    case 5: //Master Vol
                        ChangeVolume(false, 0);
                        break;
                    case 6: //BGM Vol
                        ChangeVolume(false, 1);
                        break;
                    case 7: //SFX Vol
                        ChangeVolume(false, 2);
                        break;
                }
            }
            if (state == menuState.stage)
            {
                switch (stageRow)
                {
                    case 0: //Row 1 (Myth)
                        row1Stages[stageCol].GetComponent<Image>().sprite = defaultButtonBG;
                        row1Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                        stageCol = Mathf.Clamp(--stageCol, 0, 4);
                        row1Stages[stageCol].GetComponent<Image>().sprite = selectedButtonBG;
                        row1Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                        break;
                    case 1: //Row 2 (Council)
                        row2Stages[stageCol].GetComponent<Image>().sprite = defaultButtonBG;
                        row2Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                        stageCol = Mathf.Clamp(--stageCol, 0, 5);
                        row2Stages[stageCol].GetComponent<Image>().sprite = selectedButtonBG;
                        row2Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                        break;
                }
                UpdateStageInfo(stageRow, stageCol);

                AudioManager.Instance.Play("MenuMove");
            }
        }
        if (input.UI.Right.WasPressedThisFrame())
        {
            if (state == menuState.options)
            {
                switch (menuIndex)
                {
                    case 0: //Option 1
                        option1 = !option1;
                        optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().text = (option1) ? "Yes" : "No";
                        break;
                    case 1: //Option 2
                        option2 = !option2;
                        optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().text = (option2) ? "Yes" : "No";
                        break;
                    case 2: //Option 3
                        option3 = !option3;
                        optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().text = (option3) ? "Yes" : "No";
                        break;
                    case 3: //Resolution

                        break;
                    case 4: //Display

                        break;
                    case 5: //Master Vol
                        ChangeVolume(true, 0);
                        break;
                    case 6: //BGM Vol
                        ChangeVolume(true, 1);
                        break;
                    case 7: //SFX Vol
                        ChangeVolume(true, 2);
                        break;
                }
            }
            if (state == menuState.stage)
            {
                switch (stageRow)
                {
                    case 0: //Row 1 (Myth)
                        row1Stages[stageCol].GetComponent<Image>().sprite = defaultButtonBG;
                        row1Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                        stageCol = Mathf.Clamp(++stageCol, 0, 4);
                        row1Stages[stageCol].GetComponent<Image>().sprite = selectedButtonBG;
                        row1Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                        break;
                    case 1: //Row 2 (Council)
                        row2Stages[stageCol].GetComponent<Image>().sprite = defaultButtonBG;
                        row2Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                        stageCol = Mathf.Clamp(++stageCol, 0, 5);
                        row2Stages[stageCol].GetComponent<Image>().sprite = selectedButtonBG;
                        row2Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                        break;
                }
                UpdateStageInfo(stageRow, stageCol);

                AudioManager.Instance.Play("MenuMove");
            }
        }
        if (input.UI.Left.WasReleasedThisFrame() || input.UI.Right.WasReleasedThisFrame())
        {
            if(state == menuState.options)
            {
                switch (menuIndex)
                {
                    case 5: //Master Vol
                    case 6: //BGM Vol
                    case 7: //SFX Vol
                        changeVol = false;
                        break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //Changes volume value accordingly
        if (changeVol)
        {
            switch (volIndex)
            {
                case 0: //Master
                    if (volIncrease) masterVol = Mathf.Clamp(++masterVol, 0, 100);
                    else masterVol = Mathf.Clamp(--masterVol, 0, 100);
                    AudioManager.Instance.ChangeMasterVolume(masterVol);
                    optionsRightText[5].GetComponent<TextMeshProUGUI>().text = masterVol + "%";
                    break;
                case 1: //BGM
                    if (volIncrease) BGMVol = Mathf.Clamp(++BGMVol, 0, 100);
                    else BGMVol = Mathf.Clamp(--BGMVol, 0, 100);
                    AudioManager.Instance.ChangeBGMVolume(BGMVol);
                    optionsRightText[6].GetComponent<TextMeshProUGUI>().text = BGMVol + "%";
                    break;
                case 2: //SFX
                    if (volIncrease) SFXVol = Mathf.Clamp(++SFXVol, 0, 100);
                    else SFXVol = Mathf.Clamp(--SFXVol, 0, 100);
                    AudioManager.Instance.ChangeSFXVolume(SFXVol);
                    optionsRightText[7].GetComponent<TextMeshProUGUI>().text = SFXVol + "%";
                    break;
            }
        }
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("difficulty", difficultyLevel);
        PlayerPrefs.SetInt("stageRow", stageRow);
        PlayerPrefs.SetInt("stageCol", stageCol);

        SceneManager.LoadScene("LoadingScreen");
        //Debug.Log("Started game with difficulty level: " + difficultyLevel);
    }

    public void OpenTutorial()
    {
        ShowWIPText();
        Debug.Log("Tutorial Button Clicked");
    }

    void OpenStageSelect()
    {
        mainMenuButtons[menuIndex].GetComponent<Image>().sprite = defaultButtonBG;
        mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        state = menuState.stage;
        menuIndex = 0;
        stageRow = 0;
        stageCol = 0;
        stageSelectBox.SetActive(true);
        row1Stages[0].GetComponent<Image>().sprite = selectedButtonBG;
        row1Stages[0].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        UpdateStageInfo(stageRow, stageCol);
    }

    void CloseStageSelect()
    {
        switch (stageRow)
        {
            case 0:
                row1Stages[stageCol].GetComponent<Image>().sprite = defaultButtonBG;
                row1Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                break;
            case 1:
                row2Stages[stageCol].GetComponent<Image>().sprite = defaultButtonBG;
                row2Stages[stageCol].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                break;
        }
        stageSelectBox.SetActive(false);
        state = menuState.main;
        menuIndex = 0;
        mainMenuButtons[menuIndex].GetComponent<Image>().sprite = selectedButtonBG;
        mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }

    /*
     * stageState[0] = Name
     * stageState[0] = Generation
     * stageState[0] = Description
     */
    void UpdateStageInfo(int row, int col)
    {
        switch (row)
        {
            case 0:
                switch (col)
                {
                    case 0: //Myth
                        stageStats[0].text = "Name: Myth Collab";
                        stageStats[1].text = "Generation: Myth";
                        stageStats[2].text = "Still work in progress!";
                        break;
                    case 1: //Gura
                        stageStats[0].text = "Name: Gawr Gura";
                        stageStats[1].text = "Generation: Myth";
                        stageStats[2].text = "This small and aerodynamic shark girl is quick on her feet and will not hesitate " +
                            "to jump at you when given the chance! Gura will fire beams of water through her trident and spray water " +
                            "bullets everywhere.";
                        break;
                    case 2: //Calli
                        stageStats[0].text = "Name: Mori Calliope";
                        stageStats[1].text = "Generation: Myth";
                        stageStats[2].text = "Still work in progress!";
                        break;
                    case 3: //Ame
                        stageStats[0].text = "Name: Amelia Watson";
                        stageStats[1].text = "Generation: Myth";
                        stageStats[2].text = "Still work in progress!";
                        break;
                    case 4: //Kiara
                        stageStats[0].text = "Name: Takanashi Kiara";
                        stageStats[1].text = "Generation: Myth";
                        stageStats[2].text = "Still work in progress!";
                        break;
                }
                break;
            case 1:
                switch (col)
                {
                    case 0: //Council
                        stageStats[0].text = "Name: Council Collab";
                        stageStats[1].text = "Generation: Council";
                        stageStats[2].text = "Still work in progress!";
                        break;
                    case 1: //Bae
                        stageStats[0].text = "Name: Hakos Baelz";
                        stageStats[1].text = "Generation: Council";
                        stageStats[2].text = "Still work in progress!";
                        break;
                    case 2: //Mumei
                        stageStats[0].text = "Name: Nanashi Mumei";
                        stageStats[1].text = "Generation: Council";
                        stageStats[2].text = "Still work in progress!";
                        break;
                    case 3: //Fauna
                        stageStats[0].text = "Name: Ceres Fauna";
                        stageStats[1].text = "Generation: Council";
                        stageStats[2].text = "Still work in progress!";
                        break;
                    case 4: //Kronii
                        stageStats[0].text = "Name: Ouro Kronii";
                        stageStats[1].text = "Generation: Council";
                        stageStats[2].text = "Still work in progress!";
                        break;
                    case 5: //Sana
                        stageStats[0].text = "Name: Tsukumo Sana";
                        stageStats[1].text = "Generation: Council";
                        stageStats[2].text = "Still work in progress!";
                        break;
                }
                break;
        }
    }

    void SelectStage(int row, int col)
    {
        Debug.Log("Selected (" + row + ", " + col + ")");
        StartGame();
    }

    void OpenDifficulty()
    {
        mainMenuButtons[menuIndex].GetComponent<Image>().sprite = defaultButtonBG;
        mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        state = menuState.difficulty;
        menuIndex = 0;
        difficultyLevel = 0;
        difficultyBox.SetActive(true);
        Color color = difficultyText[menuIndex].GetComponent<TextMeshProUGUI>().color;
        difficultyText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, 1);
        difficultyDescription.text = "Taking it easy!\nRecommended for players who are new to action and bullet hell games. " +
                    "Enemies will attack slower and fire less projectiles than normal.";

        StartCoroutine(MoveDifficultyBox(true, difficultyOpenDuration));
        //Debug.Log("Difficulty Menu Opened");
    }

    void CloseDifficulty()
    {
        Color color = difficultyText[menuIndex].GetComponent<TextMeshProUGUI>().color;
        difficultyText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, 0.5f);
        state = menuState.stage;
        menuIndex = 0;

        StartCoroutine(MoveDifficultyBox(false, difficultyOpenDuration));
        //Debug.Log("Difficulty Menu Closed");
    }

    public void OpenOptions()
    {
        mainMenuButtons[menuIndex].GetComponent<Image>().sprite = defaultButtonBG;
        mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        state = menuState.options;
        menuIndex = 0;
        optionsBox.SetActive(true);
        optionsLeftText[menuIndex].GetComponent<TextMeshProUGUI>().color = Color.white;
        optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().color = Color.white;

        StartCoroutine(MoveOptionsBox(true, optionsOpenDuration));
        //Debug.Log("Options Button Opened");
    }

    public void CloseOptions()
    {
        optionsLeftText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
        optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
        state = menuState.main;
        menuIndex = 0;
        mainMenuButtons[menuIndex].GetComponent<Image>().sprite = selectedButtonBG;
        mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

        OnDisable();
        StartCoroutine(MoveOptionsBox(false, optionsOpenDuration));
        //Debug.Log("Options Menu Closed");
    }

    public void OpenCredits()
    {
        ShowWIPText();
        Debug.Log("Credits Button Clicked");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit Button Clicked");
    }

    public void MouseOverButton(int index)
    {
        mainMenuButtons[menuIndex].GetComponent<Image>().sprite = defaultButtonBG;
        mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        menuIndex = index;
        mainMenuButtons[menuIndex].GetComponent<Image>().sprite = selectedButtonBG;
        mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }

    private void ChangeVolume(bool increase, int index)
    {
        changeVol = true;
        volIncrease = increase;
        volIndex = index;
    }

    private int BoolToInt(bool boolean){
        return (boolean) ? 1 : 0;
    }

    private bool IntToBool(int integer){
        return (integer == 1);
    }

    private void ShowWIPText()
    {
        Instantiate(WIPText, GameObject.FindGameObjectWithTag("Canvas").transform, false);
    }

    //Load variables on entering scene
    private void OnEnable()
    {
        option1 = IntToBool(PlayerPrefs.GetInt("option1"));
        option2 = IntToBool(PlayerPrefs.GetInt("option2"));
        option3 = IntToBool(PlayerPrefs.GetInt("option3"));
        masterVol = PlayerPrefs.GetInt("mVol");
        BGMVol = PlayerPrefs.GetInt("bgmVol");
        SFXVol = PlayerPrefs.GetInt("sfxVol");

        optionsRightText[0].GetComponent<TextMeshProUGUI>().text = (option1) ? "Yes" : "No";
        optionsRightText[1].GetComponent<TextMeshProUGUI>().text = (option2) ? "Yes" : "No";
        optionsRightText[2].GetComponent<TextMeshProUGUI>().text = (option3) ? "Yes" : "No";
        optionsRightText[5].GetComponent<TextMeshProUGUI>().text = masterVol + "%";
        optionsRightText[6].GetComponent<TextMeshProUGUI>().text = BGMVol + "%";
        optionsRightText[7].GetComponent<TextMeshProUGUI>().text = SFXVol + "%";

        Debug.Log("Variables loaded: " + option1 + " " + option2 + " " + option3 + " " + masterVol + " " + BGMVol + " " + SFXVol);
    }

    //Save variables on exiting scene
    private void OnDisable()
    {
        PlayerPrefs.SetInt("option1", BoolToInt(option1));
        PlayerPrefs.SetInt("option2", BoolToInt(option2));
        PlayerPrefs.SetInt("option3", BoolToInt(option3));

        PlayerPrefs.SetInt("mVol", masterVol);
        PlayerPrefs.SetInt("bgmVol", BGMVol);
        PlayerPrefs.SetInt("sfxVol", SFXVol);

        Debug.Log("Variables saved: " + option1 + " " + option2 + " " + option3 + " " + masterVol + " " + BGMVol + " " + SFXVol);
    }

    IEnumerator MoveOptionsBox(bool on, float duration)
    {
        optionBoxIsMoving = true;

        if (on) //Open Options box
        {
            float startPos = 0;
            float endPos = 586;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                t = Mathf.Sin((t * Mathf.PI) / 2);
                optionsBG.sizeDelta = new Vector2(2560, Mathf.LerpUnclamped(startPos, endPos, t));
                //optionsBG.GetComponent<RectTransform>() = Vector2.LerpUnclamped(startPosition, targetPosition, t);
                yield return null;
            }
            optionsBG.sizeDelta = new Vector2(2560, 586);
        }
        else    //Close Options box
        {
            float startPos = 586;
            float endPos = 0;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                t = Mathf.Sin((t * Mathf.PI) / 2);
                optionsBG.sizeDelta = new Vector2(2560, Mathf.LerpUnclamped(startPos, endPos, t));
                //optionsBG.GetComponent<RectTransform>() = Vector2.LerpUnclamped(startPosition, targetPosition, t);
                yield return null;
            }
            optionsBG.sizeDelta = new Vector2(2560, 586);
            optionsBox.SetActive(false);
        }

        optionBoxIsMoving = false;
    }

    IEnumerator MoveDifficultyBox(bool on, float duration)
    {
        optionBoxIsMoving = true;

        if (on) //Open Options box
        {
            float startPos = 0;
            float endPos = 586;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                t = Mathf.Sin((t * Mathf.PI) / 2);
                difficultyBG.sizeDelta = new Vector2(2560, Mathf.LerpUnclamped(startPos, endPos, t));
                //optionsBG.GetComponent<RectTransform>() = Vector2.LerpUnclamped(startPosition, targetPosition, t);
                yield return null;
            }
            difficultyBG.sizeDelta = new Vector2(2560, 586);
        }
        else    //Close Options box
        {
            float startPos = 586;
            float endPos = 0;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                t = Mathf.Sin((t * Mathf.PI) / 2);
                difficultyBG.sizeDelta = new Vector2(2560, Mathf.LerpUnclamped(startPos, endPos, t));
                //optionsBG.GetComponent<RectTransform>() = Vector2.LerpUnclamped(startPosition, targetPosition, t);
                yield return null;
            }
            difficultyBG.sizeDelta = new Vector2(2560, 586);
            difficultyBox.SetActive(false);
        }

        optionBoxIsMoving = false;
    }

    public int GetDifficultyLevel()
    {
        return difficultyLevel;
    }
}
