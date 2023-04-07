using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMenuItems : MonoBehaviour
{
    [SerializeField] RectTransform optionsBox;
    [SerializeField] float animationDuration = 0.5f;

    public Vector2 hiddenPosition;
    public Vector2 shownPosition;

    bool isHidden = true;
    bool isMoving = false;

    void Start()
    {
        hiddenPosition = optionsBox.anchoredPosition;
    }
/*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            MoveOptionsBox();
        }
    }
*/
    public void MoveOptionsBox()
    {
        if (!isMoving)
        {
            if (isHidden)
            {
                StartCoroutine(MoveOptionsBox(shownPosition, animationDuration));
                isHidden = false;
            }
            else
            {
                StartCoroutine(MoveOptionsBox(hiddenPosition, animationDuration));
                isHidden = true;
            }
        }
    }

    public bool OptionsIsHidden()
    {
        return isHidden;
    }

    IEnumerator MoveOptionsBox(Vector2 targetPosition, float duration)
    {
        isMoving = true;

        Vector2 startPosition = optionsBox.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = Mathf.Sin((t * Mathf.PI) / 2);
            optionsBox.anchoredPosition = Vector2.LerpUnclamped(startPosition, targetPosition, t);
            yield return null;
        }

        optionsBox.anchoredPosition = targetPosition;
        isMoving = false;
    }

    public void SetHiddenPosition()
    {
        hiddenPosition = optionsBox.anchoredPosition;
    }

    public void SetShownPosition()
    {
        shownPosition = optionsBox.anchoredPosition;
    }
}
