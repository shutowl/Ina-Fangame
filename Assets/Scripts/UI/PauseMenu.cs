using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public enum PauseState
    {
        pause,
        settings,
        graphics,
        audio,
        exitConfirm
    }
    private PauseState menu;

    public GameObject pauseUI;
    public Sprite selectedButtonBG;
    public Sprite defaultButtonBG;
    public GameObject[] pauseMenuButtons;
    private int pauseIndex = 0;

    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject exitConfirmMenu;
    public TextMeshProUGUI[] settingsLeftText;
    public TextMeshProUGUI[] settingsRightText;
    public GameObject[] exitConfirmButtons;
    bool changeVol = false;
    bool volIncrease = false;
    int volIndex = 0;
    public int masterVol = 100;
    public int BGMVol = 100;
    public int SFXVol = 100;
    public float volRate = 0.1f;
    private float volRateTimer = 0f;

    private InputActions input;
    private bool paused = false;

    void Awake()
    {
        input = new InputActions();
        input.UI.Enable();
        PauseGame(false);
        menu = PauseState.pause;

        settingsMenu.SetActive(false);
        exitConfirmMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (input.UI.Pause.WasPressedThisFrame())
        {
            if(menu == PauseState.pause)
            {
                if (!paused)
                {
                    PauseGame(true);   //Pause game
                    AudioManager.Instance.Play("Pause");
                }
                else
                {
                    PauseGame(false);   //Unpause game
                    AudioManager.Instance.Play("Unpause");
                }
            }
        }

        if (paused)
        {
            if (input.UI.Confirm.WasPressedThisFrame())
            {
                if (menu == PauseState.pause)
                {
                    switch (pauseIndex)
                    {
                        case 0:
                            ResumeGame();
                            AudioManager.Instance.Play("Unpause");
                            break;
                        case 1:
                            OpenSettings();
                            AudioManager.Instance.Play("MenuSelect");
                            break;
                        case 2:
                            OpenExitConfirm();
                            AudioManager.Instance.Play("MenuSelect");
                            break;
                    }
                }
                else if(menu == PauseState.exitConfirm)
                {
                    switch (pauseIndex)
                    {
                        case 0:
                            ExitGame();
                            AudioManager.Instance.Play("MenuSelect");
                            break;
                        case 1:
                            CloseExitConfirm();
                            AudioManager.Instance.Play("MenuSelect");
                            break;
                    }
                }
            }
            else if (input.UI.Cancel.WasPressedThisFrame())
            {
                if(menu == PauseState.pause)
                {
                    ResumeGame();
                    AudioManager.Instance.Play("Unpause");
                }
                else if (menu == PauseState.settings)
                {
                    CloseSettings();
                    AudioManager.Instance.Play("MenuCancel");
                }
                else if(menu == PauseState.exitConfirm)
                {
                    CloseExitConfirm();
                    AudioManager.Instance.Play("MenuCancel");
                }
            }
            else if(input.UI.Up.WasPressedThisFrame())
            {
                if(menu == PauseState.pause)
                {
                    pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = defaultButtonBG;
                    pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                    pauseIndex = Mathf.Clamp(--pauseIndex, 0, 2);
                    pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = selectedButtonBG;
                    pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                }
                else if(menu == PauseState.settings)
                {
                    settingsLeftText[pauseIndex].color = new Color(0.7f, 0.7f, 0.7f);
                    settingsRightText[pauseIndex].color = new Color(0.7f, 0.7f, 0.7f);
                    pauseIndex = Mathf.Clamp(--pauseIndex, 0, 2);
                    settingsLeftText[pauseIndex].color = Color.white;
                    settingsRightText[pauseIndex].color = Color.white;
                }

                AudioManager.Instance.Play("MenuMove");
            }
            else if(input.UI.Down.WasPressedThisFrame())
            {
                if (menu == PauseState.pause)
                {
                    pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = defaultButtonBG;
                    pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                    pauseIndex = Mathf.Clamp(++pauseIndex, 0, 2);
                    pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = selectedButtonBG;
                    pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                }
                else if (menu == PauseState.settings)
                {
                    settingsLeftText[pauseIndex].color = new Color(0.7f, 0.7f, 0.7f);
                    settingsRightText[pauseIndex].color = new Color(0.7f, 0.7f, 0.7f);
                    pauseIndex = Mathf.Clamp(++pauseIndex, 0, 2);
                    settingsLeftText[pauseIndex].color = Color.white;
                    settingsRightText[pauseIndex].color = Color.white;
                }

                AudioManager.Instance.Play("MenuMove");
            }
            else if(input.UI.Left.WasPressedThisFrame())
            {
                if(menu == PauseState.settings)
                {
                    switch (pauseIndex)
                    {
                        case 0: //Master Vol
                            ChangeVolume(false, 0);
                            break;
                        case 1: //BGM Vol
                            ChangeVolume(false, 1);
                            break;
                        case 2: //SFX Vol
                            ChangeVolume(false, 2);
                            break;
                    }
                }
                else if(menu == PauseState.exitConfirm)
                {
                    exitConfirmButtons[pauseIndex].GetComponent<Image>().sprite = defaultButtonBG;
                    exitConfirmButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

                    if (pauseIndex == 0) pauseIndex = 1;
                    else pauseIndex = 0;

                    exitConfirmButtons[pauseIndex].GetComponent<Image>().sprite = selectedButtonBG;
                    exitConfirmButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

                    AudioManager.Instance.Play("MenuMove");
                }
            }
            else if(input.UI.Right.WasPressedThisFrame())
            {
                if (menu == PauseState.settings)
                {
                    switch (pauseIndex)
                    {
                        case 0: //Master Vol
                            ChangeVolume(true, 0);
                            break;
                        case 1: //BGM Vol
                            ChangeVolume(true, 1);
                            break;
                        case 2: //SFX Vol
                            ChangeVolume(true, 2);
                            break;
                    }
                }
                else if (menu == PauseState.exitConfirm)
                {
                    exitConfirmButtons[pauseIndex].GetComponent<Image>().sprite = defaultButtonBG;
                    exitConfirmButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

                    if (pauseIndex == 0) pauseIndex = 1;
                    else pauseIndex = 0;

                    exitConfirmButtons[pauseIndex].GetComponent<Image>().sprite = selectedButtonBG;
                    exitConfirmButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

                    AudioManager.Instance.Play("MenuMove");
                }
            }
            else if(input.UI.Left.WasReleasedThisFrame() || input.UI.Right.WasReleasedThisFrame())
            {
                if (menu == PauseState.settings)
                {
                    switch (pauseIndex)
                    {
                        case 0: //Master Vol
                        case 1: //BGM Vol
                        case 2: //SFX Vol
                            changeVol = false;
                            break;
                    }
                }
            }
        }

        //Changes volume value accordingly
        if (changeVol && volRateTimer <= 0)
        {
            switch (volIndex)
            {
                case 0: //Master
                    if (volIncrease) masterVol = Mathf.Clamp(++masterVol, 0, 100);
                    else masterVol = Mathf.Clamp(--masterVol, 0, 100);
                    AudioManager.Instance.ChangeMasterVolume(masterVol);
                    settingsRightText[0].text = masterVol + "%";
                    break;
                case 1: //BGM
                    if (volIncrease) BGMVol = Mathf.Clamp(++BGMVol, 0, 100);
                    else BGMVol = Mathf.Clamp(--BGMVol, 0, 100);
                    AudioManager.Instance.ChangeBGMVolume(BGMVol);
                    settingsRightText[1].text = BGMVol + "%";
                    break;
                case 2: //SFX
                    if (volIncrease) SFXVol = Mathf.Clamp(++SFXVol, 0, 100);
                    else SFXVol = Mathf.Clamp(--SFXVol, 0, 100);
                    AudioManager.Instance.ChangeSFXVolume(SFXVol);
                    settingsRightText[2].text = SFXVol + "%";
                    break;
            }
            volRateTimer = volRate;
        }
        if(volRateTimer > 0)
        {
            volRateTimer -= Time.unscaledDeltaTime;
        }
    }


    void PauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0;
            paused = true;
            pauseUI.SetActive(true);
            StartCoroutine(FindObjectOfType<PlayerMovement>().PausePlayer(true));
            Debug.Log("Game Paused");
        }
        else
        {
            Time.timeScale = 1;
            paused = false;
            pauseUI.SetActive(false);
            StartCoroutine(FindObjectOfType<PlayerMovement>().PausePlayer(false));
            Debug.Log("Game Unpaused");
        }                
    }

    public void ResumeGame()
    {
        Debug.Log("Resume Game Button Pressed");
        PauseGame(false);
    }

    public void OpenSettings()
    {
        pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = defaultButtonBG;
        pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        pauseMenu.SetActive(false);
        pauseIndex = 0;
        menu = PauseState.settings;
        settingsLeftText[pauseIndex].color = Color.white;
        settingsRightText[pauseIndex].color = Color.white;
        settingsMenu.SetActive(true);
        Debug.Log("Settings Opened");
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        settingsLeftText[pauseIndex].color = new Color(0.7f, 0.7f, 0.7f);
        settingsRightText[pauseIndex].color = new Color(0.7f, 0.7f, 0.7f);
        pauseIndex = 0;
        changeVol = false;
        menu = PauseState.pause;
        pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = selectedButtonBG;
        pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        pauseMenu.SetActive(true);
        Debug.Log("Settings Closed");
    }

    public void OpenExitConfirm()
    {
        pauseMenu.SetActive(false);
        pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = defaultButtonBG;
        pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        pauseIndex = 1;
        menu = PauseState.exitConfirm;
        exitConfirmButtons[pauseIndex].GetComponent<Image>().sprite = selectedButtonBG;
        exitConfirmButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        exitConfirmMenu.SetActive(true);
    }

    public void CloseExitConfirm()
    {
        exitConfirmMenu.SetActive(false);
        exitConfirmButtons[pauseIndex].GetComponent<Image>().sprite = defaultButtonBG;
        exitConfirmButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        pauseIndex = 0;
        menu = PauseState.pause;
        pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = selectedButtonBG;
        pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        pauseMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game Button Pressed");
        SceneManager.LoadScene("TitleScreen");
    }

    private void ChangeVolume(bool increase, int index)
    {
        changeVol = true;
        volIncrease = increase;
        volIndex = index;
    }

    public void MouseOverButton(int index)
    {
        pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = defaultButtonBG;
        pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        pauseIndex = index;
        pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = selectedButtonBG;
        pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }

    private void OnEnable()
    {
        input.Player.Disable();

        masterVol = PlayerPrefs.GetInt("mVol");
        BGMVol = PlayerPrefs.GetInt("bgmVol");
        SFXVol = PlayerPrefs.GetInt("sfxVol");
        settingsRightText[0].text = masterVol + "%";
        settingsRightText[1].text = BGMVol + "%";
        settingsRightText[2].text = SFXVol + "%";
    }

    private void OnDisable()
    {
        input.Player.Enable();

        PlayerPrefs.SetInt("mVol", masterVol);
        PlayerPrefs.SetInt("bgmVol", BGMVol);
        PlayerPrefs.SetInt("sfxVol", SFXVol);
    }
}
