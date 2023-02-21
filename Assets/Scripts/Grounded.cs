using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : MonoBehaviour
{

	private PlayerMovement player;

	void Start()
	{
		player = gameObject.GetComponentInParent<PlayerMovement>();
	}

	//If ground collider touches the floor (tilemap)
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Ground")
		{
			player.grounded = true;
			player.resetAttackTimer();
		}
	}

	//If ground collider is still touching the floor 
	void OnTriggerStay2D(Collider2D col)
	{
		if (col.tag == "Ground")
		{
			player.grounded = true;
		}
	}

	//When ground collider leaves the floor
	void OnTriggerExit2D(Collider2D col)
	{
		if (col.tag == "Ground")
		{
			player.grounded = false;
		}
	}
}