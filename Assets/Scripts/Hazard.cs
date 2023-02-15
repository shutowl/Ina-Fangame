using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damage = 10;
    public float hitstun = 0.5f;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Hitbox"))
        {
            col.GetComponentInParent<PlayerHealth>().damage(damage, hitstun);
        }
    }
}
