using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChild : MonoBehaviour
{
    public bool hasRigidbody;
    public bool hasKinematicRigidbody;
    public bool hasParent;

    private void Start()
    {
        if(GetComponent<Rigidbody>() != null)
        {
            if(GetComponent<Rigidbody>().isKinematic)
            {
                hasKinematicRigidbody = true;
            }
        }
    }
}
