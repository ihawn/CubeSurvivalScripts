using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    DamageTaker dt;

    public bool felled;

    void Start()
    {
        dt = GetComponent<DamageTaker>();
    }


}
