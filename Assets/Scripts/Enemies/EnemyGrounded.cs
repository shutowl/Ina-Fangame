using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrounded : MonoBehaviour
{
	private Tako tako;

	void Start()
	{
		tako = gameObject.GetComponentInParent<Tako>();
	}

	//If ground collider touches the floor (tilemap)
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Ground")
		{
			tako.grounded = true;
		}
	}

	//If ground collider is still touching the floor 
	void OnTriggerStay2D(Collider2D col)
	{
		if (col.tag == "Ground")
		{
			tako.grounded = true;
		}
	}

	//When ground collider leaves the floor
	void OnTriggerExit2D(Collider2D col)
	{
		if (col.tag == "Ground")
		{
			tako.grounded = false;
		}
	}
}