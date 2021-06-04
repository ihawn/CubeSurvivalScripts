using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera vcamClose, vcamFar;
    public Camera cam;
    public bool camClose = true;

    public void ToggleViewCamera()
    {
        if (camClose)
        {
            camClose = false;
            vcamClose.gameObject.SetActive(false);
            vcamFar.gameObject.SetActive(true);
        }

        else
        {
            camClose = true;
            vcamFar.gameObject.SetActive(false);
            vcamClose.gameObject.SetActive(true);
        }
    }

}
