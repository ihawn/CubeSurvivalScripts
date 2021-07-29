using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainControl : MonoBehaviour
{
    public bool shouldGenerate, doneGenerating;
    public GameObject[] wallCubes, meadowCubes, beachAndOceanCubes, darkForestCubes, rockyBeachCubes, jungleCubes, desertCubes, oceanCubes;
    public GameObject[][] cubes;
    public GameObject player;
    public StaticGenerator[] sg;
    public float terrainDrawDistance, verticalOffset, terrainRiseSpeed, popupDistance, falloff, power;
    public int cubesPerFrame;
    public float globalCubeWidth, seaLevel;
    public bool[] completedGeneration;
    public int completedJobs = 0;


    public int cubeCount;
    public GameObject closestCube;

    public GameObject[] allCubes;

    public int proximityLevels = 6;

    private void Awake()
    {

        cubes = new GameObject[1][]; //new GameObject[sg.Length + 1][];
        //cubes[0] = wallCubes;
        cubes[0] = meadowCubes;//cubes[1] = meadowCubes;
       /*cubes[2] = beachAndOceanCubes;
        cubes[3] = darkForestCubes;
        cubes[4] = rockyBeachCubes;
        cubes[5] = jungleCubes;
        cubes[6] = desertCubes;
        cubes[7] = oceanCubes;*/


        completedGeneration = new bool[sg.Length];

        for (int i = 0; i < completedGeneration.Length; i++)
            completedGeneration[i] = false;

        cubeCount = 0;
        for (int i = 0; i < cubes.Length; i++)
            cubeCount += cubes[i].Length;

        allCubes = new GameObject[cubeCount];
        int k = 0;
        for (int i = 0; i < cubes.Length; i++)
        {
            for (int j = 0; j < cubes[i].Length; j++)
            {
                allCubes[k] = cubes[i][j];
                allCubes[k].GetComponent<TerrainCubeControler>().cubeID = k;
                k++;
            }
        }

        //Get focal point cube
        closestCube = GetStandingCube(allCubes);

    }


    void Update()
    {
        if (!doneGenerating)
            doneGenerating = CheckForGenerationCompletion();

        closestCube = GetStandingCube(closestCube.GetComponent<TerrainCubeControler>().closestCubes);
        UpdateCubes(closestCube.GetComponent<TerrainCubeControler>().closestCubes);
    }

    public GameObject GetStandingCube(GameObject[] arr)
    {
        float min = Mathf.Infinity;
        GameObject minObj = arr[0];

        for(int i = 0; i < arr.Length; i++)
        {
            float dist = BirdsEyeDistance(arr[i], player);
            if (dist < min)
            {
                min = dist;
                minObj = arr[i];
            }
        }

        return minObj;
    }


    public float BirdsEyeDistance(GameObject p, GameObject c)
    {
        return Vector2.Distance(new Vector2(p.transform.position.x, p.transform.position.z), new Vector2(c.transform.position.x, c.transform.position.z));
    }



    void UpdateCubes(GameObject[] cubes)
    {
        for (int i = 0; i < cubes.Length; i++)
        {
            if (!cubes[i].activeInHierarchy &&
                Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z),
                new Vector2(cubes[i].transform.position.x, cubes[i].transform.position.z)) <= terrainDrawDistance*3f)
            {
                //move cube up
                cubes[i].SetActive(true);
            }
        }
    }

    bool CheckForGenerationCompletion()
    {
        for(int i = 0; i < completedGeneration.Length; i++)
        {
            if (!completedGeneration[i])
                return false;
        }

        return true;
    }
}
