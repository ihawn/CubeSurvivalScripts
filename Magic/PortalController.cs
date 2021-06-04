using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public bool activatePortalNextFrame, portalActive, doneActivating;
    TerrainControl tc;

    public GameObject[] portalRocks;
    public GameObject rockContainer;
    public float rotationSpeed, rotationOffset;
    public Vector3 rotationDirection;
    Vector3[] rockStartPos;
    Vector3[] rockTargets;
    Quaternion[] rockStartRotation;
    public float rockLerpSpeed, rockPulseSpeed, rockPulseEpsilon, sigma, mu;
    public Vector3 minRockRange, maxRockRange;


    private void Start()
    {
        tc = FindObjectOfType<TerrainControl>();
        GetRockStartPositions();
    }

    void Update()
    {
        if(activatePortalNextFrame && !portalActive)
        {
            ActivatePortal();
        }

        if(portalActive && doneActivating)
        {
            MoveFloatingRocks();
           // RotatePortal();
        }
    }

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

    void RotatePortal()
    {
        rockContainer.transform.Rotate(rotationDirection * rotationSpeed * Time.deltaTime / (Vector3.Distance(tc.player.transform.position, rockContainer.transform.position) + rotationOffset));
    }

    IEnumerator ActivatePortalRocks()
    {
        RemoveRockRigidbodies();

        while (Vector3.Distance(portalRocks[0].transform.localPosition, rockStartPos[0]) > 0.01f)
        {
            for (int i = 0; i < rockStartPos.Length; i++)
            {
                portalRocks[i].transform.localPosition = Vector3.Lerp(portalRocks[i].transform.localPosition, rockStartPos[i], rockLerpSpeed * Time.deltaTime);
                portalRocks[i].transform.localRotation = Quaternion.Lerp(portalRocks[i].transform.localRotation, rockStartRotation[i], rockLerpSpeed * Time.deltaTime);
            }

            yield return null;
        }

        doneActivating = true;
    }

}
