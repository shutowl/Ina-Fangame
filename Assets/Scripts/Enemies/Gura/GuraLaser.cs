using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class GuraLaser : MonoBehaviour
{
    private VolumetricLineBehavior line;
    private EdgeCollider2D hurtbox;
    public float lifeTime = 2f;
    private float lifeTimeTimer = 0f;
    public float width = 3f;
    public Vector3 startPos;
    public Vector3 endPos;
    public bool explode = false;    //creates a fountain of bullets at contact with ground
    public bool indicator = false;


    void Start()
    {
        line = GetComponent<VolumetricLineBehavior>();
        hurtbox = GetComponent<EdgeCollider2D>();
        line.SetStartAndEndPoints(startPos, endPos);
        hurtbox.SetPoints(new List<Vector2>() { startPos, endPos });

        lifeTimeTimer = lifeTime;

        if (indicator)
        {
            line.LineColor = new Color(0f, 0.5f, 0.5f, 1f);
            line.LightSaberFactor = 0.9f;
            line.LineWidth = 0.5f;
            hurtbox.enabled = false;
        }
        else
        {
            hurtbox.enabled = true;
        }
    }

    void Update()
    {
        lifeTimeTimer -= Time.deltaTime;

        if (indicator)
        {
            line.LineColor = new Color(0f, 0.5f, 0.5f, 1f);
            line.LightSaberFactor = 0.9f;
            line.LineWidth = 0.5f;
            lifeTimeTimer = 999f;
        }
        else
        {
            line.LineWidth = (1 - Mathf.Pow(1 - lifeTimeTimer / lifeTime, 5)) * width;
            hurtbox.edgeRadius = (1 - Mathf.Pow(1 - lifeTimeTimer / lifeTime, 5)) * width * 0.15f;
        }

        line.SetStartAndEndPoints(startPos, endPos);

        if (lifeTimeTimer <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetPositions(Vector3 start, Vector3 end)
    {
        startPos = start;
        endPos = end;
    }

    public void SetLifeTime(float lifeTime)
    {
        this.lifeTime = lifeTime;
    }

    public void SetWidth(float width)
    {
        this.width = width;
    }

    public void SetLightFactor(float light)
    {
        line.LightSaberFactor = light;
    }
}