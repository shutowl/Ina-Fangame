using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    public TextMeshProUGUI NPCText;
    public float cutSceneDelay;
    private bool nearNPC;
    public Dialogue dialogue;
    public Vector2[] sections;  //Use 2d array of int later
    public int repeat = 0;

    public IEnumerator TriggerDialogue(float delay)
    {
        yield return new WaitForSeconds(delay);
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    public IEnumerator TriggerDialogue(float delay, int start, int end)
    {
        yield return new WaitForSeconds(delay);
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, start, end);
        if(repeat + 1 < sections.Length)
            repeat++;
    }

    //Player passes through trigger point
    private void OnTriggerEnter2D(Collider2D col)
    {
        //Area trigger
        if(tag != "NPC" && col.tag == "Player")
        {
            StartCoroutine(TriggerDialogue(cutSceneDelay));
            GetComponent<BoxCollider2D>().enabled = false;

            //Player in cutscene mode
            col.GetComponent<PlayerMovement>().setCutsceneState(cutSceneDelay + 0.2f);
        }
        //NPC trigger
        if (tag == "NPC" && col.tag == "Player")
        {
            nearNPC = true;
            NPCText.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (tag == "NPC" && col.tag == "Player")
        {
            nearNPC = false;
            NPCText.enabled = false;
        }
    }

    private void Update()
    {
        if (nearNPC)
        {
            if (FindObjectOfType<PlayerMovement>().currentState == PlayerMovement.playerState.moving)
            {
                if (FindObjectOfType<PlayerMovement>().getInputActions().Player.Interact.WasPressedThisFrame())
                {
                    StartCoroutine(TriggerDialogue(0f, (int)sections[repeat].x, (int)sections[repeat].y));

                    //Player in cutscene mode
                    FindObjectOfType<PlayerMovement>().setCutsceneState(0f);
                }
            }
        }
    }



}
