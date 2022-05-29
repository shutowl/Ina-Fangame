using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    [Header("Position")]
    public Vector2 shownPosition;
    public Vector2 hiddenPosition;

    private Queue<string> sentences;
    private Queue<float> textSpeeds;
    public float defaultTextSpeed = 0.04f;

    private bool typing;
    private bool moving;
    private string lastSentence;
    Coroutine lastRoutine = null;

    void Start()
    {
        sentences = new Queue<string>();
        textSpeeds = new Queue<float>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        foreach (float textSpeed in dialogue.textSpeeds)
        {
            textSpeeds.Enqueue(textSpeed);
        }

        if (moving)
        {
            StopCoroutine(lastRoutine);
            transform.position = hiddenPosition;
            moving = false;
        }
        lastRoutine = StartCoroutine(moveDialogueBox(shownPosition, 1f));

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (typing)
        {
            StopCoroutine(lastRoutine);
            dialogueText.text = lastSentence;
            typing = false;
            return;
        }

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        lastSentence = sentence;
        float textSpeed = defaultTextSpeed;

        if (textSpeeds.Count != 0)
            textSpeed = textSpeeds.Dequeue();

        lastRoutine = StartCoroutine(TypeSentence(sentence, textSpeed));
    }

    void EndDialogue()
    {
        FindObjectOfType<PlayerMovement>().currentState = PlayerMovement.playerState.moving;
        lastRoutine = StartCoroutine(moveDialogueBox(hiddenPosition, 1.0f));
        Debug.Log("End of conversation");
    }

    public void setShownPosition()
    {
        shownPosition = new Vector2(GetComponent<RectTransform>().position.x, GetComponent<RectTransform>().position.y);
    }
    public void setHiddenPosition()
    {
        hiddenPosition = new Vector2(GetComponent<RectTransform>().position.x, GetComponent<RectTransform>().position.y);
    }

    //easeOutQuint Function: https://easings.net/#easeOutQuart
    IEnumerator moveDialogueBox(Vector2 targetPos, float duration)
    {
        moving = true;

        float time = 0;
        Vector2 startPosition = transform.position;
        while (time < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPos, 1 - Mathf.Pow(1 - (time / duration), 5));
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;

        moving = false;
    }

    IEnumerator TypeSentence(string sentence, float delay)
    {
        typing = true;

        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(delay);
        }

        typing = false;
    }

}

[CustomEditor(typeof(DialogueManager))]
public class customInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueManager manager = (DialogueManager)target;
        if(GUILayout.Button("Set Shown Position"))
        {
            manager.setShownPosition();
        }
        if (GUILayout.Button("Set Hidden Position"))
        {
            manager.setHiddenPosition();
        }
    }
}
