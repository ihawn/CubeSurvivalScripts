using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PortalController : MonoBehaviour
{
    public bool activatePortalNextFrame, portalActive, doneActivating;
    public float minPortalSeparation;
    TerrainControl tc;

    public GameObject[] portalRocks; 
    public GameObject rockContainer;
    public VisualEffect portalEffect, portalActivation;
    public VisualEffect[] rockActivation;
    public PortalController connectedPortal;
    public Transform portalTo;
    public float rotationSpeed, rotationOffset, rotationExponent, rotateRange, rotationAcceleration;
    public float speedSlack;
    public Vector3 rotationDirection;
    Vector3[] rockStartPos;
    Vector3[] rockTargets;
    Quaternion[] rockStartRotation;
    public float rockLerpSpeed, rockPulseSpeed, rockPulseEpsilon, sigma, mu;
    public Vector3 minRockRange, maxRockRange;

    public string[] canPortalList;


    private void Start()
    {
        tc = FindObjectOfType<TerrainControl>();
        GetRockStartPositions();
        portalEffect.SetFloat("Global Spawn Multiplier", 0f);
        portalActivation.SetFloat("Rate", 0f);

        SetRockSparkleRate(0);
    }

    void Update()
    {
        if(activatePortalNextFrame && !portalActive)
        {
            ActivatePortal();
            connectedPortal.ActivatePortal();
        }

        if(portalActive && doneActivating)
        {
            MoveFloatingRocks();
            UpdateActivationEffects();
        }
    }

    void SetRockSparkleRate(int rate)
    {
        for (int i = 0; i < rockActivation.Length; i++)
        {
            rockActivation[i].SetFloat("Rate", rate);
        }
    }


    //Block controlling portal movement
    void GetRockStartPositions()
    {
        rockStartPos = new Vector3[portalRocks.Length];
        rockStartRotation = new Quaternion[portalRocks.Length];
        rockTargets = new Vector3[portalRocks.Length];
        
        for(int i = 0; i < rockStartPos.Length; i++)
        {
            rockStartPos[i] = portalRocks[i].transform.localPosition;
            rockStartRotation[i] = portalRocks[i].transform.localRotation;
            rockTargets[i] = GenerateNewRockTarget(i);
        }
    }

    void ActivatePortal()
    {
        portalActive = true;
        

        StartCoroutine(ActivatePortalRocks());
    }

    Vector3 GenerateNewRockTarget(int i)
    {
        return rockStartPos[i] + new Vector3(Random.Range(minRockRange.x, maxRockRange.x), Random.Range(minRockRange.y, maxRockRange.y), Random.Range(minRockRange.z, maxRockRange.z));
    }

    void RemoveRockRigidbodies()
    {
        for(int i = 0; i < portalRocks.Length; i++)
        {
            if (portalRocks[i].GetComponent<Rigidbody>() != null)
                Destroy(portalRocks[i].GetComponent<Rigidbody>());
        }
    }

    void MoveFloatingRocks()
    {
        for(int i = 0; i < portalRocks.Length; i++)
        {
            portalRocks[i].transform.localPosition = Vector3.Slerp(portalRocks[i].transform.localPosition, rockTargets[i], rockPulseSpeed * Time.deltaTime * NormalDistribution(Vector3.Distance(portalRocks[i].transform.localPosition, rockTargets[i])));

            if (Vector3.Distance(portalRocks[i].transform.localPosition, rockTargets[i]) <= rockPulseEpsilon)
                rockTargets[i] = GenerateNewRockTarget(i);
        }
    }

    float NormalDistribution(float x)
    {
        return (1 / (sigma * Mathf.Sqrt(2f * Mathf.PI))) * Mathf.Exp(-Mathf.Pow((x - mu), 2f) / Mathf.Pow(2f * sigma, 2f));
    }

    public void UpdateActivationEffects()
    {
        if (Vector3.Distance(tc.player.transform.position, transform.position) <= minPortalSeparation)
        {
            if (Vector3.Distance(tc.player.transform.position, transform.position) <= rotateRange && speedSlack < 1)
            {
                speedSlack += Time.deltaTime * rotationAcceleration;
                portalEffect.SetFloat("Global Spawn Multiplier", 1f);

                connectedPortal.speedSlack += Time.deltaTime * rotationAcceleration;
                connectedPortal.portalEffect.SetFloat("Global Spawn Multiplier", 1f);
            }
            else if (Vector3.Distance(tc.player.transform.position, transform.position) > rotateRange && speedSlack > 0)
            {
                speedSlack -= Time.deltaTime * rotationAcceleration;
                portalEffect.SetFloat("Global Spawn Multiplier", 0f);

                connectedPortal.speedSlack -= Time.deltaTime * rotationAcceleration;
                connectedPortal.portalEffect.SetFloat("Global Spawn Multiplier", 0f);
            }
        }
        
        
        rockContainer.transform.Rotate(speedSlack * rotationDirection * rotationSpeed * Time.deltaTime / Mathf.Pow(Vector3.Distance(tc.player.transform.position, transform.position) + rotationOffset, rotationExponent));

    }

    IEnumerator ActivatePortalRocks()
    {
        RemoveRockRigidbodies();
        portalActivation.SetFloat("Rate", 1f);
        SetRockSparkleRate(250);

        while (Vector3.Distance(portalRocks[0].transform.localPosition, rockStartPos[0]) > 0.01f)
        {
            if(Vector3.Distance(portalRocks[0].transform.localPosition, rockStartPos[0]) < 0.1f)
                SetRockSparkleRate(500);

            for (int i = 0; i < rockStartPos.Length; i++)
            {
                portalRocks[i].transform.localPosition = Vector3.Lerp(portalRocks[i].transform.localPosition, rockStartPos[i], rockLerpSpeed * Time.deltaTime);
                portalRocks[i].transform.localRotation = Quaternion.Lerp(portalRocks[i].transform.localRotation, rockStartRotation[i], rockLerpSpeed * Time.deltaTime);
            }

            yield return null;
        }

        portalActivation.SetFloat("Rate", 0f);
        SetRockSparkleRate(0);
        doneActivating = true;
    }
    //End movement block

    //Block controlling portal functionallity
    bool CollisionCanTeleport(string tag)
    {
        if (System.Array.IndexOf(canPortalList, tag) != -1)
            return true;
        else
            return false;
    }

    void Teleport(GameObject portalee)
    {
        if(portalee.GetComponent<CharacterController>() != null)
            portalee.GetComponent<CharacterController>().enabled = false;
        

        portalee.transform.position = connectedPortal.portalTo.position;
        portalee.transform.rotation = Quaternion.Euler(connectedPortal.transform.rotation.eulerAngles);
        connectedPortal.speedSlack = 1f;

        if (portalee.GetComponent<CharacterController>() != null)
            portalee.GetComponent<CharacterController>().enabled = true; ;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (portalActive && doneActivating && CollisionCanTeleport(other.gameObject.tag))
            Teleport(other.gameObject);
    }

    
    //End functionallity block
}
