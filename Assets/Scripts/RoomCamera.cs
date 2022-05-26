using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    private new CameraFollow camera;
    public float size;
    private float savedSize;
    public Vector3 offset;
    private Vector3 savedOffset;

    private void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();
        savedOffset = camera.offset;
        savedSize = camera.size;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            camera.target = this.transform;
            camera.offset = offset;
            camera.size = size;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            camera.followPlayer();
            camera.offset = savedOffset;
            camera.size = savedSize;
        }
    }
}
