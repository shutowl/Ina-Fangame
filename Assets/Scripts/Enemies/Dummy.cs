using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dummy : MonoBehaviour
{

    public TextMeshProUGUI DPSText;
    public float showText = 2f;         //How long text should stay on screen
    private float textTimer = 0;
    private float DPSTimer = 0;
    private float DPSSum = 0;

    // Start is called before the first frame update
    void Start()
    {
        DPSText.color = new Color(255, 255, 255, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(textTimer <= 0){
            DPSSum = 0;
            DPSTimer = 0;
            textTimer = 0;
        }
        else{
            DPSTimer += Time.deltaTime;
            textTimer -= Time.deltaTime;
        }
        DPSText.color = new Color(1, 1, 1, (textTimer / showText));
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        //Bullet enters Dummy
        if(col.CompareTag("Player Bullet"))
        {
            //DPSText.color = new Color(255, 255, 255, 255);
            int damage = col.GetComponent<aoBullet>().damage;
            DPSSum += damage;
            DPSText.text = "Damage: " + damage;
            if(DPSSum/DPSTimer > 10000){
                DPSText.text += "\nDPS: " + damage;
            }
            else{
                DPSText.text += "\nDPS: " + DPSSum/DPSTimer;
            }
            textTimer = showText;
        }

        //Crowbar enters Dummy
    }
}
