using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeathBarController : MonoBehaviour
{
    private PlayerController player;
    private Transform camera;
    public GameObject health, whoCreatedMe;
    public float scalingLerpSpeed, scaleOnDamage;
    public float startScaleZ, startScaleY;
    public float radius, radiusOffsetMultiplier;
    Vector3 normalPoint;

    void Start()
    {
        player = StaticObjects.player;
        camera = StaticObjects.mainCam;

        float xySize = new Vector2(whoCreatedMe.GetComponent<MeshRenderer>().bounds.size.x, whoCreatedMe.GetComponent<MeshRenderer>().bounds.size.z).magnitude;
        radius = radiusOffsetMultiplier * xySize;

        normalPoint = whoCreatedMe.transform.position + new Vector3(radius, 0f, 0f);
    }


    void Update()
    {
        transform.LookAt(transform.position + camera.rotation * Vector3.right,
        camera.transform.rotation * Vector3.up);

        if (health.transform.localScale.y > startScaleY)
        {
            health.transform.localScale = Vector3.Lerp(health.transform.localScale, new Vector3(
                startScaleY, startScaleY, health.transform.localScale.z),
                scalingLerpSpeed * Time.deltaTime);
        }

        UpdateOrbitalPosition();
    }

    void UpdateOrbitalPosition()
    {
        float D = OverheadDistance(normalPoint, camera.position);
        float P = OverheadDistance(whoCreatedMe.transform.position, camera.position);
        float B = OverheadDistance(whoCreatedMe.transform.position, normalPoint);
        float theta = Mathf.Acos((D * D - P * P - B * B) / (-2 * P * B));
        if (camera.transform.position.z - whoCreatedMe.transform.position.z < 0)
            theta = 2*Mathf.PI - theta;

        float x = radius * Mathf.Cos(theta);
        float z = radius * Mathf.Sin(theta);

        try
        {
            if(!float.IsNaN(whoCreatedMe.transform.position.x) && !float.IsNaN(x))
            {
                transform.position = new Vector3(whoCreatedMe.transform.position.x, 0f, whoCreatedMe.transform.position.z) +
                new Vector3(x, transform.position.y, z);
            }
        }
        catch { }


    }

    public void SetHealth(float h, float maxHealth)
    {
        health.transform.localScale = new Vector3(scaleOnDamage,
            scaleOnDamage, (h / maxHealth) * startScaleZ);

    }

    float OverheadDistance(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(new Vector3(v1.x, 0f, v1.z), new Vector3(v2.x, 0f, v2.z));
    }

}
