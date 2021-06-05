using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera vcamFar;
    public CinemachineFreeLook freeLookCamera;
    public Camera cam;
    public bool camClose = true;

    public void ToggleViewCamera()
    {
        if (camClose)
        {
            camClose = false;
            freeLookCamera.Priority = 0;
            vcamFar.Priority = 1;
        }

        else
        {
            camClose = true;
            freeLookCamera.Priority = 1;
            vcamFar.Priority = 1;
        }
    }

}
