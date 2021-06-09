using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public bool inMenus;
    public GameObject menu;

    public void ToggleMenu()
    {
        if(inMenus)
        {
            inMenus = false;
            menu.SetActive(false);
        }
        else
        {
            inMenus = true;
            menu.SetActive(true);
        }
    }
}
