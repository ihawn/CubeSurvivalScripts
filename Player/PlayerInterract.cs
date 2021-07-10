using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerInterract : MonoBehaviour
{
    public Inventory playerInventory;
    public UIController theUIController;
    public Text messageText;
    public GameObject[] ignoreList;
    public GameObject[] choices;
    GameObject lookAt;
    public Camera Cam;
    public string crossHairHit;
    public float messageDuration, lookDistance;

    public bool inChoicesMenu;

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

        /*   RaycastHit[] hits;
           hits = Physics.RaycastAll(Cam.transform.position, Cam.transform.forward, lookDistance);

           for(int i = 0; i < hits.Length; i++)
           {
               RaycastHit hit = hits[i];
               print(hit.transform.gameObject);

               for (int j = 0; j < ignoreTags.Length; j++)
               {
                   if (hit.transform.gameObject.tag != ignoreTags[j])
                   {
                       return hit.transform.gameObject.tag;
                   }
               }
           }

           return "";*/

        RaycastHit hit;

       if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, lookDistance))
        {
            Debug.DrawRay(Cam.transform.position, Cam.transform.forward * hit.distance, Color.red);
            Debug.Log("Looking at " + hit.transform.gameObject);

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
        if (!theUIController.inMenus)
        {
            switch (choice)
            {
                case "keyhole":
                    inChoicesMenu = true;
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
                    inChoicesMenu = true;
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
                    inChoicesMenu = false;
                    Interractions.HideChoices(choices);
                    Interractions.Hide(messageText);
                    break;
            }
        }

    }



    void InterractWithDoor()
    {

    }
}
