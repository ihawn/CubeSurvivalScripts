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
    public PlayerController player;
    public GameObject playerHand, aimFollow;
    public Camera Cam;
    public Image crosshairWide;
    public string crossHairHit;
    public float messageDuration, lookDistance;

    public bool inChoicesMenu;

    void Update()
    {
        crossHairHit = LookingAt();
        InterractChoice(crossHairHit);
        UpdateCrossHair();
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

       if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, lookDistance))
        {
            Debug.DrawRay(Cam.transform.position, Cam.transform.forward * hit.distance, Color.red);
           // Debug.Log("Looking at " + hit.transform.gameObject);

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


    void UpdateCrossHair()
    {
        if (player.bigCharge || player.smallCharge)
        {
            RaycastHit hit;
            if (Physics.Raycast(aimFollow.transform.position, Cam.transform.forward, out hit, lookDistance*3))
            {
                transform.position = Vector3.Lerp(transform.position, Cam.WorldToScreenPoint(hit.point), 10f*Time.deltaTime);
            }

            if(player.bigCharge)
                crosshairWide.gameObject.transform.localScale = Vector3.Lerp(crosshairWide.gameObject.transform.localScale, Vector3.one, 10f * Time.deltaTime);
            if(player.smallCharge && !player.bigCharge)
                crosshairWide.gameObject.transform.localScale = Vector3.Lerp(crosshairWide.gameObject.transform.localScale, Vector3.one*1.7f, 10f * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(Screen.width / 2, Screen.height / 2, 0f), 10f * Time.deltaTime);
            crosshairWide.gameObject.transform.localScale = Vector3.Lerp(crosshairWide.gameObject.transform.localScale, Vector3.zero, 10f * Time.deltaTime);
        }
    }


    void InterractWithDoor()
    {

    }
}
