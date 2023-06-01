using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerBomb : MonoBehaviour
{
    public float startRadius;
    public float endRadius;
    public float bombDuration;
    public float damageRate;        //enemies in range are damaged every [damageRate] seconds
    float rateTimer = 0f;

    void Start()
    {
        transform.localScale = new Vector2(startRadius, startRadius);
        transform.DOScale(endRadius, bombDuration/2).SetEase(Ease.OutCubic);
        transform.DOScale(0, bombDuration/2).SetEase(Ease.InQuint).SetDelay(bombDuration/2);

        rateTimer = 0f;
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

    private void OnTriggerStay2D(Collider2D col)
    {
        rateTimer -= Time.deltaTime;

        if (col.CompareTag("Enemy"))
        {
            if(rateTimer <= 0)
            {
                //Damage Enemy
                col.GetComponent<Enemy>().TakeDamage(10, 0, 0.5f);
                rateTimer = damageRate;
            }
        }
    }
}
