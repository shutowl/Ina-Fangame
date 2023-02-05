using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Hitbox"))
        {
            col.GetComponentInParent<PlayerHealth>().damage(damage);
        }
    }
}
