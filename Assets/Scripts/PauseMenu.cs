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
    }
    private PauseState menu;

    public GameObject pauseUI;
    public Sprite selectedButtonBG;
    public Sprite defaultButtonBG;
    public GameObject[] pauseMenuButtons;
    private int pauseIndex = 0;

    private InputActions input;
    private bool paused = false;

    void Awake()
    {
        input = new InputActions();
        input.UI.Enable();
        PauseGame(false);
        menu = PauseState.pause;
    }

    // Update is called once per frame
    void Update()
    {
        if (input.UI.Pause.WasPressedThisFrame())
        {
            if (!paused) PauseGame(true);   //Pause game
            else PauseGame(false);          //Unpause game
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
                            break;
                        case 1:
                            OpenSettings();
                            break;
                        case 2:
                            ExitGame();
                            break;
                    }
                }
            }
            if (input.UI.Cancel.WasPressedThisFrame())
            {

            }
            if (input.UI.Up.WasPressedThisFrame())
            {
                pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = defaultButtonBG;
                pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                pauseIndex = Mathf.Clamp(--pauseIndex, 0, 2);
                pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = selectedButtonBG;
                pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
            if (input.UI.Down.WasPressedThisFrame())
            {
                pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = defaultButtonBG;
                pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                pauseIndex = Mathf.Clamp(++pauseIndex, 0, 2);
                pauseMenuButtons[pauseIndex].GetComponent<Image>().sprite = selectedButtonBG;
                pauseMenuButtons[pauseIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
            if (input.UI.Left.WasPressedThisFrame())
            {

            }
            if (input.UI.Right.WasPressedThisFrame())
            {

            }
        }


    }

    void PauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0;
            paused = true;
            pauseUI.SetActive(true);
            Debug.Log("Game Paused");
        }
        else
        {
            Time.timeScale = 1;
            paused = false;
            pauseUI.SetActive(false);
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
        Debug.Log("Settings Button Pressed");
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game Button Pressed");
        SceneManager.LoadScene("TitleScreen");
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
        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }
}
