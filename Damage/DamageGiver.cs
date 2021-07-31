using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGiver : MonoBehaviour
{
    public TrailRenderer[] trails;
    Coroutine trailCo;
    bool canDisableTrail = true;
    public float trailOffDelay;
    public string[] damageTags;
    public string itemName;
    public float dph;
    public float speed;
    Vector3 lastPos = Vector3.zero, currentPos = Vector3.zero;
    public bool dpsDependsOnSpeed, overrideSpeedThreshold, dontGiveDamage;
    public float dphSpeedMultiplier, overriddenSpeedThreshold;


    private void Start()
    {
        try
        {
            dph = StaticObjects.player.GetComponent<Inventory>().ItemByName(itemName).stats["DPH"];
        }
        catch { }
    }

    private void FixedUpdate()
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

    private void Update()
    {
        if(trails.Length > 0)
        {
            if (!dontGiveDamage && !trails[0].emitting)
            {
                for (int i = 0; i < trails.Length; i++)
                    trails[i].emitting = true;
                
            }

            if (dontGiveDamage && trails[0].emitting && canDisableTrail)
            {
                if (trailCo != null)
                    StopCoroutine(trailCo);
                trailCo = StartCoroutine(DelayTrailOff());
            }
        }

    }

    IEnumerator DelayTrailOff()
    {
        canDisableTrail = false;
        yield return new WaitForSeconds(trailOffDelay);
        for (int i = 0; i < trails.Length; i++)
            trails[i].emitting = false;
        canDisableTrail = true;
    }

}
