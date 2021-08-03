using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera vcamFar;
    public CinemachineFreeLook freeLookCamera;
    public CinemachineFreeLook aimCam;
    public PlayerController player;
    public CinemachineImpulseSource playerChargeImpulse;
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
            if (player.throwing)
                aimCam.Priority = 1;
            else
                freeLookCamera.Priority = 1;
            camClose = true;
            vcamFar.Priority = 0;
        }
    }

    public void SetViewCamera(bool far)
    {
        if (far)
        {
            camClose = false;
            freeLookCamera.Priority = 0;
            vcamFar.Priority = 1;
        }

        else
        {
            if (player.throwing)
                aimCam.Priority = 1;
            else
                freeLookCamera.Priority = 1;
            camClose = true;
            vcamFar.Priority = 0;
        }
    }

    public void SetAimCamera(bool aiming)
    {
        if (aiming && camClose)
        {
            if (aimCam.Priority == 0)
            {
                aimCam.Priority = 1;
                freeLookCamera.Priority = 0;
            }      
        }
        else if(!aiming && camClose)
        {
            if (aimCam.Priority == 1)
            {
                aimCam.Priority = 0;
                freeLookCamera.Priority = 1;
            }
        }
    }

    public void SetShake()
    {
        playerChargeImpulse.GenerateImpulse();
    }

}
