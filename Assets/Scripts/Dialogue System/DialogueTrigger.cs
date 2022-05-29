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

    public IEnumerator TriggerDialogue(float delay)
    {
        yield return new WaitForSeconds(delay);
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    //Player passes through trigger point
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(tag != "NPC" && col.tag == "Player")
        {
            StartCoroutine(TriggerDialogue(1f));
            GetComponent<BoxCollider2D>().enabled = false;

            //Player in cutscene mode
            StartCoroutine(col.GetComponent<PlayerMovement>().setCutsceneState(cutSceneDelay));
        }
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
                    StartCoroutine(TriggerDialogue(0f));

                    //Player in cutscene mode
                    StartCoroutine(FindObjectOfType<PlayerMovement>().setCutsceneState(0f));
                }
            }
        }
    }



}
