using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeathBarController : MonoBehaviour
{
    private PlayerController player;
    private Transform camera;
    public GameObject health;
    public float scalingLerpSpeed, scaleOnDamage;
    public float startScaleZ, startScaleY;

    void Start()
    {
        player = StaticObjects.player;
        camera = StaticObjects.mainCam;
    }


    void Update()
    {
        transform.LookAt(transform.position + camera.rotation * Vector3.right,
        camera.transform.rotation * Vector3.up);

        if(health.transform.localScale.y > startScaleY)
        {
            health.transform.localScale = Vector3.Lerp(health.transform.localScale, new Vector3(
                startScaleY, startScaleY, health.transform.localScale.z),
                scalingLerpSpeed * Time.deltaTime);
        }
    }

    public void SetHealth(float h, float maxHealth)
    {
        health.transform.localScale = new Vector3(scaleOnDamage,
            scaleOnDamage, (h / maxHealth) * startScaleZ);

        print(h);
        print(maxHealth);
        print(h / maxHealth);
        print(startScaleZ);
    }
}
