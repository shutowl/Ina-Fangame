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
    public GameObject[] optionsLeftText;
    public GameObject[] optionsRightText;
    private int menuIndex = 0;

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
            if (state == menuState.main)
            {
                switch (menuIndex)
                {
                    case 0:
                        StartGame();
                        break;
                    case 1:
                        Practice();
                        break;
                    case 2:
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
                CloseOptions();
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
        SceneManager.LoadScene("LoadingScreen");
        Debug.Log("Start Button Clicked");
    }

    public void Practice()
    {
        Debug.Log("Practice Button Clicked");
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
        Debug.Log("Options Button Clicked");
    }

    public void CloseOptions()
    {
        optionsLeftText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
        optionsRightText[menuIndex].GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
        state = menuState.main;
        menuIndex = 0;
        optionsBox.SetActive(false);
        mainMenuButtons[menuIndex].GetComponent<Image>().sprite = selectedButtonBG;
        mainMenuButtons[menuIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        Debug.Log("Options Menu Closed");
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
        return (integer == 1) ? true : false;
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
}
