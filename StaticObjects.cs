using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObjects : MonoBehaviour
{
    public static PlayerController player;
    public static Transform mainCam;
    public static GameManager gm;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        gm = FindObjectOfType<GameManager>();
        mainCam = player.cam;
    }
}
