using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    private GameObject player;
    public float size = 6f;
    public float smoothSpeed = 10f;
    public Vector3 offset;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        GetComponent<Camera>().orthographicSize = size;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        //transform.LookAt(target); only in 3D
    }

    public void followPlayer()
    {
        target = player.transform;
    }
}
