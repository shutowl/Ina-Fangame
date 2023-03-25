using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterSeconds : MonoBehaviour
{
    public float lifetime;

    void Start()
    {
        //If an animation component exists on the game object
        if (gameObject.GetComponent<Animator>() != null)
        {
            lifetime = this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        }

        Destroy(gameObject, lifetime);

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
