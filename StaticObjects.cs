using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObjects : MonoBehaviour
{
    public static PlayerController player;
    public static Transform mainCam;
    public static GameManager gm;
    public static TerrainControl tc;
    public static PortalMaster pm;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        gm = FindObjectOfType<GameManager>();
        tc = FindObjectOfType<TerrainControl>();
        pm = FindObjectOfType<PortalMaster>();
        mainCam = player.cam;
    }
}
