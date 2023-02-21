using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main purpose is to add contact damage to environment hazards, enemies, and bullets.
public class Hazard : MonoBehaviour
{
    public int damage = 10;             //Amount of damage player recieves
    public float hitstun = 0.5f;        //Amount of hitstun player recieves

    private bool active;

    private void Start()
    {
        active = true;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.CompareTag("Hitbox"))
        {
            if (active)
            {
                if(damage > 0)
                    col.GetComponentInParent<PlayerHealth>().damage(damage, hitstun);
            }
        }
    }

    public void setActive(bool active)
    {
        this.active = active;
    }

    public bool isActive()
    {
        return active;
    }
}
