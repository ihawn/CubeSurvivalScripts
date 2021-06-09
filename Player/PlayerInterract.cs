using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerInterract : MonoBehaviour
{
    public Inventory playerInventory;
    public Text messageText;
    public GameObject[] ignoreList;
    public GameObject[] choices;
    GameObject lookAt;
    public Camera Cam;
    public string crossHairHit;
    public float messageDuration;

    void Update()
    {
        crossHairHit = LookingAt();
        InterractChoice(crossHairHit);
    }

    void EnableIgnore(bool enable)
    {
        for (int i = 0; i < ignoreList.Length; i++)
        {
            ignoreList[i].SetActive(enable);
        }
    }

    string LookingAt()
    {
        EnableIgnore(false);

        RaycastHit hit;

        if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit))
        {
            Debug.DrawRay(Cam.transform.position, Cam.transform.forward * hit.distance, Color.red);
            Debug.Log("Looking at " + hit.transform.gameObject.tag);

            lookAt = hit.transform.gameObject;
            EnableIgnore(true);
            return hit.transform.gameObject.tag;
        }
        else
        {
            EnableIgnore(true);
            return "";
        }
    }

    void InterractChoice(string choice)
    {

        switch(choice)
        {
            case "keyhole":
                Interractions.Door(
                    playerInventory.CheckForItem("ObsidianKey") != null,
                    lookAt.GetComponent<Keyhole>().attachedDoor,
                    new string[] { "1] Listen at the keyhole", "2] Insert Key" },
                    choices,
                    messageText,
                    messageDuration,
                    playerInventory,
                    lookAt,
                    lookAt.GetComponent<Keyhole>().attachedDoor.keyholeMessage
                    );
                break;

            case "keyhole2":
                Interractions.Door(
                    playerInventory.CheckForItem("ObsidianKey") != null,
                    lookAt.GetComponent<Keyhole>().attachedDoor,
                    new string[] { "1] Listen at the keyhole", "2] Insert Key" },
                    choices,
                    messageText,
                    messageDuration,
                    playerInventory,
                    lookAt,
                    lookAt.GetComponent<Keyhole>().attachedDoor.keyholeMessage2
                    );
                break;


            default:
                Interractions.HideChoices(choices);
                Interractions.Hide(messageText);
                break;
        }

    }



    void InterractWithDoor()
    {

    }
}
