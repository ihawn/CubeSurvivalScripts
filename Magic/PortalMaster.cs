using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PortalMaster : MonoBehaviour
{
    public float portalSpeed;

    public IEnumerator Teleport(GameObject portalee, PortalController connectedPortal)
    {
        if (portalee.gameObject.GetComponent<PlayerController>() != null)
        {
            portalee.GetComponent<PlayerController>().portalSparkle.transform.LookAt(connectedPortal.gameObject.transform, Vector3.forward);
        }

        List<GameObject> list = new List<GameObject>();
        SetStuff(false, portalee, connectedPortal);
        SetStuffAllChildren(false, portalee.transform, list, connectedPortal);


        

        while (Vector3.Distance(portalee.transform.position, connectedPortal.realPos) > 1f)
        {
            portalee.transform.position = Vector3.MoveTowards(portalee.transform.position, connectedPortal.realPos, portalSpeed * Time.deltaTime);
            yield return null;
        }
        portalee.transform.rotation = Quaternion.Euler(connectedPortal.transform.rotation.eulerAngles);
        connectedPortal.speedSlack = 1f;

        SetStuff(true, portalee, connectedPortal);
        SetStuffAllChildren(true, portalee.transform, list, connectedPortal);

    }

    void SetStuff(bool b, GameObject portalee, PortalController connectedPortal)
    {
        if (portalee.GetComponent<CharacterController>() != null)
            portalee.GetComponent<CharacterController>().enabled = b;
        if (portalee.GetComponent<Renderer>() != null && portalee.gameObject.GetComponent<VisualEffect>() == null)
            portalee.GetComponent<Renderer>().enabled = b;
        if (portalee.GetComponent<PlayerController>() != null)
        {
            portalee.GetComponent<PlayerController>().portalSparkle.GetComponent<VisualEffect>().SetFloat("Spawn", !b ? 1 : 0);
            portalee.GetComponent<PlayerController>().enabled = b;

            StaticObjects.cc.SetViewCamera(!b);
        }
    }

    void SetStuffAllChildren(bool b, Transform root, List<GameObject> list, PortalController connectedPortal)
    {
        foreach (Transform child in root)
        {
            
            SetStuff(b, child.gameObject, connectedPortal);
            SetStuffAllChildren(b, child, list, connectedPortal);
        }
    }


}
