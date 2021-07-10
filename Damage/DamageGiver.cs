using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGiver : MonoBehaviour
{
    public string[] damageTags;
    public float dph;
    public float speed;
    Vector3 lastPos = Vector3.zero, currentPos = Vector3.zero;
    public bool dpsDependsOnSpeed;
    public float dphSpeedMultiplier;

    private void Update()
    {
        currentPos = transform.position;

        if(lastPos != Vector3.zero)
        {
            speed = (currentPos - lastPos).magnitude / Time.deltaTime;
        }

        lastPos = currentPos;

        if (dpsDependsOnSpeed)
            dph = speed * dphSpeedMultiplier;
    }
}
