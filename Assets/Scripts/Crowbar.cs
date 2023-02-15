using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowbar : MonoBehaviour
{
    public int n1Damage = 30;
    public int n2Damage = 20;
    public int n3Damage = 10;
    public int nairDamage = 30;
    public int dairDamage = 30;

    PlayerMovement player;

    void Start()
    {
        player = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            Enemy enemy = col.GetComponent<Enemy>();

            switch (player.getAttackNum())
            {
                case 1:     //Neutral 1
                    enemy.TakeDamage(n1Damage, 0.3f);
                    break;
                case 2:     //Neutral 2
                    enemy.TakeDamage(n2Damage, 0.2f);
                    break;
                case 3:     //Neutral 3
                    enemy.TakeDamage(n3Damage, 0.3f);
                    break;
                case 4:     //NAir
                    enemy.TakeDamage(nairDamage, 0.1f);
                    break;
                case 5:     //DAir
                    enemy.TakeDamage(dairDamage, 0.1f);
                    player.Bounce();
                    break;
            }
        }
    }
}
