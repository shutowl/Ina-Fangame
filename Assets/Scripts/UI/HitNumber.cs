using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitNumber : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float duration;
    float durationTimer;
    int direction = 1;
    Rigidbody2D rb;
    GameObject player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        rb.velocity = Vector2.zero;
        direction = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? -1 : 1;
        rb.AddForce(new Vector2(Random.Range(0f, 200f) * direction, Random.Range(300f, 400f)));
        GetComponent<Canvas>().worldCamera = Camera.main;

        durationTimer = duration;
    }

    void Update()
    {
        durationTimer -= Time.deltaTime;
        float t = durationTimer / duration;

        text.color = new Color(text.color.r, text.color.g, text.color.b, t);

        if(durationTimer <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void SetSize(float damage)
    {
        this.text.fontSize = Mathf.Clamp(damage / 4, 5f, 12f);
    }
}
