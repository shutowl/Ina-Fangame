using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class WIPText : MonoBehaviour
{
    public float lifeTime = 1.0f;
    public float distance = 10f;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.DOAnchorPos(new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + distance), lifeTime/2);

        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        text.DOColor(new Color(text.color.r, text.color.g, text.color.b, 0), lifeTime).SetEase(Ease.InQuad);
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;

        if(lifeTime <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
