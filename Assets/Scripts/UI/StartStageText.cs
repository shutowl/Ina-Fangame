using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class StartStageText : MonoBehaviour
{
    public float distance = 10f;
    public float duration = 3f;

    public GameObject mainTextBox;
    public GameObject subTextBox;
    TextMeshProUGUI mainText;
    TextMeshProUGUI subText;

    void Start()
    {
        mainText = mainTextBox.GetComponent<TextMeshProUGUI>();
        subText = subTextBox.GetComponent<TextMeshProUGUI>();
        mainText.color = new Color(1, 1, 1, 0);
        subText.color = new Color(1, 1, 1, 0);

        RectTransform rect = GetComponent<RectTransform>();
        rect.DOAnchorPos(new Vector2(rect.anchoredPosition.x + distance, rect.anchoredPosition.y), duration);

        mainText.DOColor(new Color(mainText.color.r, mainText.color.g, mainText.color.b, 1), duration / 2).SetEase(Ease.OutQuad);
        subText.DOColor(new Color(subText.color.r, subText.color.g, subText.color.b, 1), duration / 2).SetEase(Ease.OutQuad).SetDelay(1.5f);

        mainText.DOColor(new Color(mainText.color.r, mainText.color.g, mainText.color.b, 0), duration / 2).SetEase(Ease.InQuad).SetDelay(duration / 2);
        subText.DOColor(new Color(subText.color.r, subText.color.g, subText.color.b, 0), duration / 2).SetEase(Ease.InQuad).SetDelay(duration / 2);
    }
}
