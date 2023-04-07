using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainMenuController : MonoBehaviour
{
    public enum menuState
    {
        main,
        stage,
        difficulty,
        options
    }
    public menuState currentState;
    /* General Arrow Indexes:
     * MAIN MENU
     * 0 = Start Game
     * 1 = Practice
     * 2 = Options
     * 3 = Exit Game
     * STAGES
     * 4 = Tutorial
     * 5 = Stage 1
     * DIFFICULTIES
     * 6 = Easy
     * 7 = Normal
     * 8 = Hard
     * 9 = Impossible
     */
    public GameObject arrow;
    public List<Vector2> arrowPositions;
    public GameObject stages;
    public GameObject difficulties;
    private int arrowIndex;
    private MoveMenuItems moveMenu;

    void Start()
    {
        moveMenu = GetComponent<MoveMenuItems>();
        arrowIndex = 0;
        arrow.GetComponent<RectTransform>().anchoredPosition = arrowPositions[arrowIndex];
        currentState = menuState.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))    //Navigate Up
        {
            if (currentState == menuState.main)
            {
                if (arrowIndex == 0) arrowIndex = 3;    //If at top, loop back to bottom
                else arrowIndex = Mathf.Clamp(--arrowIndex, 0, 3);
            }
            else if (currentState == menuState.stage)
            {
                if (arrowIndex == 4) arrowIndex = 5;
                else arrowIndex = Mathf.Clamp(--arrowIndex, 4, 5);
            }
            else if (currentState == menuState.difficulty)
            {
                if (arrowIndex == 6) arrowIndex = 9;
                else arrowIndex = Mathf.Clamp(--arrowIndex, 6, 9);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))    //Navigate Down
        {
            if (currentState == menuState.main)
            {
                if (arrowIndex == 3) arrowIndex = 0;    //If at bottom, loop back to top
                else arrowIndex = Mathf.Clamp(++arrowIndex, 0, 3);
            }
            else if (currentState == menuState.stage)
            {
                if (arrowIndex == 5) arrowIndex = 4;
                else arrowIndex = Mathf.Clamp(++arrowIndex, 4, 5);
            }
            else if (currentState == menuState.difficulty)
            {
                if (arrowIndex == 9) arrowIndex = 6;
                else arrowIndex = Mathf.Clamp(++arrowIndex, 6, 9);
            }

        }
        arrow.GetComponent<RectTransform>().anchoredPosition = arrowPositions[arrowIndex];  //Moves between first and last positions

        //Main Inputs
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))  //Select Current Option
        {
            switch (arrowIndex)
            {
                //MAIN
                case 0:
                    stages.SetActive(true);
                    currentState = menuState.stage;
                    arrowIndex = 4;
                    break;
                case 1:
                    EnterBossRush();
                    break;
                case 2:
                    OpenOptions();
                    break;
                case 3:
                    ExitGame();
                    break;
                //STAGES
                case 4: //Tutorial
                    difficulties.SetActive(true);
                    currentState = menuState.difficulty;
                    arrowIndex = 6;
                    break;
                case 5: //Stage 1
                    difficulties.SetActive(true);
                    currentState = menuState.difficulty;
                    arrowIndex = 6;
                    break;
                //DIFFICULTIES
                case 6:
                    StartGame();
                    break;
                case 7:
                    StartGame();
                    break;
                case 8:
                    StartGame();
                    break;
                case 9:
                    StartGame();
                    break;
            }
        }

        //Back Button
        if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape)))
        {
            if(!moveMenu.OptionsIsHidden())
                OpenOptions();
            else if (currentState == menuState.stage)
            {
                stages.SetActive(false);
                currentState = menuState.main;
                arrowIndex = 0;
            }
            else if(currentState == menuState.difficulty)
            {
                difficulties.SetActive(false);
                currentState = menuState.stage;
                arrowIndex = 4;
            }
        }

        //Stage Menu Inputs


        //Options Menu Inputs
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space)) && !moveMenu.OptionsIsHidden())
        {
            
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("StageTest");
        Debug.Log("Start Button Clicked");
    }

    public void EnterBossRush()
    {
        Debug.Log("Boss Rush Button Clicked");
    }

    public void OpenOptions()
    {
        moveMenu.MoveOptionsBox();
        Debug.Log("Options Button Clicked");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit Button Clicked");
    }

    public void MouseOverButton(int index)
    {
        arrowIndex = index;
    }



    //Custom Inspector button to add current arrow position into the list
    public void AddArrowPosition()
    {
        arrowPositions.Add(arrow.GetComponent<RectTransform>().anchoredPosition);
    }
}

[CustomEditor(typeof(MainMenuController))]
public class TitleScreenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MainMenuController titleScript = (MainMenuController)target;
        if (GUILayout.Button("Add Arrow Position"))
        {
            titleScript.AddArrowPosition();
        }
    }
}
