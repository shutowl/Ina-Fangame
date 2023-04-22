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
    public GameObject optionsBox;
    public RectTransform optionsBG;
    public float optionsOpenDuration = 0.2f;
    private bool optionBoxIsMoving = false;
    public GameObject[] optionsLeftText;
    public GameObject[] optionsRightText;
    private int menuIndex = 0;

    public GameObject difficultyBox;
    public RectTransform difficultyBG;
    public float difficultyOpenDuration = 0.2f;
    public GameObject[] difficultyText;
    public TextMeshProUGUI difficultyDescription;
    private int difficultyLevel = 0;

    [Header("Options Variables")]
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

    private InputActions input;

    void Awake()
    {
        input = new InputActions();
        input.UI.Enable();
        state = menuState.main;
        optionsBox.SetActive(false);
        difficultyBox.SetActive(false);
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
            }
            else if(state == menuState.difficulty)
            {
                StartGame();
            }
            else if (state == menuState.main)
            {
                switch (menuIndex)
                {
                    case 0:
                        OpenDifficulty();
                        break;
                    case 1:
                        Practice();
                        break;
                    case 2:
                        if(!optionBoxIsMoving)
                            OpenOptions();
                        break;
                    case 3:
                        ExitGame();
                        break;
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
            }
            if (state == menuState.options)
            {
                if (!optionBoxIsMoving)
                    CloseOptions();
            }
            if(state == menuState.difficulty)
            {
                CloseDifficulty();
            }
        }
        if (input.UI.Up.WasPressedThisFrame())
        {
            if(state == menuState.main)
            {
                mainMenuButtons[menuIndex].GetComponent<Image>().sprite = defaultButtonBG;
                mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                menuIndex = Mathf.Clamp(--menuIndex, 0, 3);
                mainMenuButtons[menuIndex].GetComponent<Image>().sprite = selectedButtonBG;
                mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
            if(state == menuState.options)
            {
                optionsLeftText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
                optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
                menuIndex = Mathf.Clamp(--menuIndex, 0, 7);
                optionsLeftText[menuIndex].GetComponent<TextMeshProUGUI>().color = Color.white;
                optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().color = Color.white;

                if(changeVol) changeVol = false;
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
            }
        }
        if (input.UI.Down.WasPressedThisFrame())
        {
            if (state == menuState.main)
            {
                mainMenuButtons[menuIndex].GetComponent<Image>().sprite = defaultButtonBG;
                mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                menuIndex = Mathf.Clamp(++menuIndex, 0, 3);
                mainMenuButtons[menuIndex].GetComponent<Image>().sprite = selectedButtonBG;
                mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
            if (state == menuState.options)
            {
                optionsLeftText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
                optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
                menuIndex = Mathf.Clamp(++menuIndex, 0, 7);
                optionsLeftText[menuIndex].GetComponent<TextMeshProUGUI>().color = Color.white;
                optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().color = Color.white;

                if(changeVol) changeVol = false;
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
                    optionsRightText[5].GetComponent<TextMeshProUGUI>().text = masterVol + "%";
                    break;
                case 1: //BGM
                    if (volIncrease) BGMVol = Mathf.Clamp(++BGMVol, 0, 100);
                    else BGMVol = Mathf.Clamp(--BGMVol, 0, 100);
                    optionsRightText[6].GetComponent<TextMeshProUGUI>().text = BGMVol + "%";
                    break;
                case 2: //SFX
                    if (volIncrease) SFXVol = Mathf.Clamp(++SFXVol, 0, 100);
                    else SFXVol = Mathf.Clamp(--SFXVol, 0, 100);
                    optionsRightText[7].GetComponent<TextMeshProUGUI>().text = SFXVol + "%";
                    break;
            }
        }
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("difficulty", difficultyLevel);
        SceneManager.LoadScene("LoadingScreen");
        //Debug.Log("Started game with difficulty level: " + difficultyLevel);
    }

    public void Practice()
    {
        Debug.Log("Practice Button Clicked");
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
        state = menuState.main;
        menuIndex = 0;
        mainMenuButtons[menuIndex].GetComponent<Image>().sprite = selectedButtonBG;
        mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

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

        StartCoroutine(MoveOptionsBox(false, optionsOpenDuration));
        //Debug.Log("Options Menu Closed");
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
