using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainControl : MonoBehaviour
{
    public bool shouldGenerate, doneGenerating;
    public GameObject[] wallCubes, meadowCubes, beachAndOceanCubes, darkForestCubes, rockyBeachCubes, jungleCubes, desertCubes, oceanCubes;
    GameObject[][] cubes;
    public GameObject player;
    public StaticGenerator[] sg;
    public float terrainDrawDistance, verticalOffset, terrainRiseSpeed, popupDistance, falloff, power;
    public int cubesPerFrame;
    public float globalCubeWidth, seaLevel;
    public bool[] completedGeneration;


    public bool shouldUpdate;
    public float closeThreshold;
    public int closeCubesPerFrame, farCubesPerFrame;
    List<GameObject> closeCubes = new List<GameObject>();
    List<GameObject> farCubes = new List<GameObject>();

    private void Awake()
    {
        cubes = new GameObject[sg.Length + 1][];
        cubes[0] = wallCubes;
        cubes[1] = meadowCubes;
        cubes[2] = beachAndOceanCubes;
        cubes[3] = darkForestCubes;
        cubes[4] = rockyBeachCubes;
        cubes[5] = jungleCubes;
        cubes[6] = desertCubes;
        cubes[7] = oceanCubes;


        int len = 0;

        for (int i = 0; i < cubes.Length; i++)
            len += cubes[i].Length;

        InitializeCubeLists();
        StartCoroutine(UpdateFarCubes());
        StartCoroutine(UpdateCloseCubes());

        completedGeneration = new bool[sg.Length];

        for (int i = 0; i < completedGeneration.Length; i++)
            completedGeneration[i] = false;

    }


    void Update()
    {
        if (!doneGenerating)
            doneGenerating = CheckForGenerationCompletion();

        UpdateCubes(closeCubes);
    }

    public void InitializeCubeLists()
    {
        closeCubes.Clear();
        farCubes.Clear();

        for (int i = 0; i < cubes.Length; i++)
        {
            for(int j = 0; j < cubes[i].Length; j++)
            {
                if (BirdsEyeDistance(cubes[i][j], player) <= closeThreshold)
                    closeCubes.Add(cubes[i][j]);
                else
                    farCubes.Add(cubes[i][j]);
                
            }
        }
    }

    IEnumerator UpdateFarCubes()
    {
        while (shouldUpdate)
        {
            for (int i = 0; i < farCubes.Count; i++)
            {
                if (BirdsEyeDistance(farCubes[i], player) <= closeThreshold)
                {
                    GameObject temp = farCubes[i];
                    farCubes.Remove(farCubes[i]);
                    closeCubes.Add(temp);
                }

                if (i % farCubesPerFrame == 0)
                    yield return null;
            }
        }
    }

    IEnumerator UpdateCloseCubes()
    {
        while(shouldUpdate)
        {
            for(int i = 0; i < closeCubes.Count; i++)
            {
                if(BirdsEyeDistance(closeCubes[i], player) > closeThreshold)
                {
                    GameObject temp = closeCubes[i];
                    closeCubes.Remove(closeCubes[i]);
                    farCubes.Add(temp);
                }

                if (i % closeCubesPerFrame == 0)
                    yield return null;
            }
        }
    }

    float BirdsEyeDistance(GameObject p, GameObject c)
    {
        return Vector2.Distance(new Vector2(p.transform.position.x, p.transform.position.z), new Vector2(c.transform.position.x, c.transform.position.z));
    }



    void UpdateCubes(List<GameObject> cubes)
    {
        for (int i = 0; i < cubes.Count; i++)
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
