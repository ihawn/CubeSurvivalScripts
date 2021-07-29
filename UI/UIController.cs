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
            player.closeCam.m_XAxis.m_InputAxisName = "Mouse X";
            player.closeCam.m_YAxis.m_InputAxisName = "Mouse Y";
            
        }
        else
        {
            inMenus = true;
            menu.SetActive(true);
            player.UnlockCurser();
            player.closeCam.m_XAxis.m_InputAxisName = "";
            player.closeCam.m_YAxis.m_InputAxisName = "";
            player.closeCam.m_XAxis.m_InputAxisValue = 0f;
            player.closeCam.m_YAxis.m_InputAxisValue = 0f;
        }
    }


}
