using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public bool inMenus;
    public GameObject menu;
    public PlayerController player;

    public void ToggleMenu()
    {
        if(inMenus)
        {
            inMenus = false;
            menu.SetActive(false);
            player.LockCurser();
        }
        else
        {
            inMenus = true;
            menu.SetActive(true);
            player.UnlockCurser();
        }
    }
}
