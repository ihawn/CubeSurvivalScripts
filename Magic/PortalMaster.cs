using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalMaster : MonoBehaviour
{
    public float portalSpeed;

    public IEnumerator Teleport(GameObject portalee, PortalController connectedPortal)
    {
        if (portalee.GetComponent<CharacterController>() != null)
            portalee.GetComponent<CharacterController>().enabled = false;
        if (portalee.GetComponent<Renderer>() != null)
            portalee.GetComponent<Renderer>().enabled = false;
        if (portalee.GetComponent<PlayerController>() != null)
            portalee.GetComponent<PlayerController>().enabled = false;

        while (Vector3.Distance(portalee.transform.position, connectedPortal.realPos) > 1f)
        {
            portalee.transform.position = Vector3.Lerp(portalee.transform.position, connectedPortal.realPos, portalSpeed * Time.deltaTime);
            yield return null;
        }
        portalee.transform.rotation = Quaternion.Euler(connectedPortal.transform.rotation.eulerAngles);
        connectedPortal.speedSlack = 1f;
        //StaticObjects.tc.InitializeCubeLists();

        if (portalee.GetComponent<CharacterController>() != null)
            portalee.GetComponent<CharacterController>().enabled = true;
        if (portalee.GetComponent<Renderer>() != null)
            portalee.GetComponent<Renderer>().enabled = true;
        if (portalee.GetComponent<PlayerController>() != null)
            portalee.GetComponent<PlayerController>().enabled = true; ;

    }
}
