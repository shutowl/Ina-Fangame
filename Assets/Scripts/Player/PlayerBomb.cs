using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerBomb : MonoBehaviour
{
    public float startRadius;
    public float endRadius;
    public float bombDuration;

    void Start()
    {
        transform.localScale = new Vector2(startRadius, startRadius);
        transform.DOScale(endRadius, bombDuration/2).SetEase(Ease.OutCubic);
        transform.DOScale(0, bombDuration/2).SetEase(Ease.InQuint).SetDelay(bombDuration/2);
    }

    void Update()
    {
        bombDuration -= Time.deltaTime;
        //size grows from startRadius to endRadius

        if(bombDuration <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetDuration(float duration)
    {
        bombDuration = duration;
    }

    public void SetRadius(float minRadius, float maxRadius)
    {
        startRadius = minRadius;
        endRadius = maxRadius;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Hazard"))
        {
            Destroy(col.gameObject);
        }
    }
}
