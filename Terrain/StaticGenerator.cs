using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticGenerator : MonoBehaviour
{
    public int biomeID;

    public TerrainControl tc;
    public int cubesPerFrame;

    
    public GameObject[] rocks;
    public bool hasRocks;
    public int maxRocks;
    public float rockProbability, minRockSize, maxRockSize, yMinRockOffset, yMaxRockOffset, rockClearingScale;
    public Vector3 minRockRotation, maxRockRotation;
    public Vector2 rockHeightRange;


    public GameObject[] smallRocks;
    public bool hasSmallRocks;
    public int maxSmallRocks;
    public float smallRockProbability, minSmallRockSize, maxSmallRockSize, yMinSmallRockOffset, yMaxSmallRockOffset, smallRockClearingScale;
    public Vector3 minSmallRockRotation, maxSmallRockRotation;
    public Vector2 smallRockHeightRange;


    public GameObject[] trees;
    public bool hasTrees;
    public int maxTrees;
    public float treesProbability, minTreesize, maxTreesize, yMinTreesOffset, yMaxTreesOffset, treeClearingScale;
    public Vector3 minTreesRotation, maxTreesRotation;
    public Vector2 treeHeightRange;


    public GameObject[] grass;
    public bool hasGrass;
    public int maxGrass;
    public float grassProbability, minGrassSize, maxGrassSize, yMinGrassOffset, yMaxGrassOffset, grassClearingScale;
    public Vector3 minGrassRotation, maxGrassRotation;
    public Vector2 grassHeightRange;


    public GameObject[] miscPlants;
    public bool hasMiscPlants;
    public int maxMiscPlants;
    public float miscPlantsProbability, minMiscPlantsize, maxMiscPlantssize, yMinMiscPlantsOffset, yMaxMiscPlantsOffset, miscPlantsClearingScale;
    public Vector3 minMiscPlantsRotation, maxMiscPlantsRotation;
    public Vector2 miscPlantsHeightRange;


    public GameObject[] cattails;
    public bool hasCattails;
    public int maxCattails;
    public float cattailsProbability, minCattailssize, maxCattailssize, yMinCattailsOffset, yMaxCattailsOffset, cattailsClearingScale;
    public Vector3 minCattailsRotation, maxCattailsRotation;
    public Vector2 cattailHeightRange;





    private void Awake()
    {
        if (tc.shouldGenerate)
        {
            switch (biomeID)
            {
                case 1:
                    StartCoroutine(Generate(tc.meadowCubes));
                    break;

                case 2:
                    StartCoroutine(Generate(tc.beachAndOceanCubes));
                    break;

                case 3:
                    StartCoroutine(Generate(tc.darkForestCubes));
                    break;

                case 4:
                    StartCoroutine(Generate(tc.rockyBeachCubes));
                    break;

                case 5:
                    StartCoroutine(Generate(tc.jungleCubes));
                    break;

                case 6:
                    StartCoroutine(Generate(tc.desertCubes));
                    break;
            }
        }
    }


    void CubeGenerate(GameObject cube, GameObject[] objects, Vector3 minRot, Vector3 maxRot, int maxCount, float prob, float minSize, float maxSize, float yMinOffset, float yMaxOffset, Vector2 cutoffRange, float scale)
    {
        Vector3 cubePos = cube.transform.position;
        Vector3 bounds = Vector3.one * tc.globalCubeWidth;
        
        for (int i = 0; i < maxCount; i++)
        {
            bool isGround = false;
            float randX = Random.Range(-bounds.x, bounds.x);
            float randZ = Random.Range(-bounds.z, bounds.z);
            float xPos = cubePos.x + randX;
            float zPos = cubePos.z + randZ;
            float height = GetYHeight(xPos, zPos, out isGround) + Random.Range(yMinOffset, yMaxOffset);
            float perlin = Mathf.Round(Mathf.PerlinNoise(xPos * scale, zPos * scale));


            if (height >= cutoffRange.x && height <= cutoffRange.y && isGround && Random.Range(0f, 100f) <= prob*perlin)
            {
                Vector3 pos = new Vector3(xPos, height, zPos);
                Quaternion rot = Quaternion.Euler(new Vector3(Random.Range(minRot.x, maxRot.x), Random.Range(minRot.y, maxRot.y), Random.Range(minRot.z, maxRot.z)));
                GameObject obj = Instantiate(objects[Random.Range(0, objects.Length)], pos, rot);

                float size = Random.Range(minSize, maxSize);
                float num = size / maxSize;
                obj.transform.localScale = Vector3.one * (Mathf.Exp(10f * num) / Mathf.Exp(10f) + num / 4.8f + 0.5f) * size;
                obj.transform.parent = cube.transform;
            }
        }
    }


    float GetYHeight(float xPos, float zPos, out bool isGround)
    {
        RaycastHit hit;
        Vector3 pos = Vector3.zero;
        Vector3 rayPos = new Vector3(xPos, 40f, zPos);
        isGround = false;

        if (Physics.Raycast(rayPos, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            pos = hit.point;

            if (hit.transform.tag == "Ground")
                isGround = true;

            Debug.DrawRay(rayPos, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
           // Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(rayPos, transform.TransformDirection(Vector3.down) * 1000, Color.white);
           // Debug.Log("Did not Hit");
        }

        return pos.y;
    }


    public IEnumerator Generate(GameObject[] cubes)
    {
        tc.doneGenerating = false;

        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].GetComponent<TerrainCubeControler>().generationDone = true;

            if (cubes[i].GetComponent<MeshCollider>() == null)
                break;

            if (hasRocks)
                CubeGenerate(cubes[i], rocks, minRockRotation, maxRockRotation, maxRocks, rockProbability, minRockSize, maxRockSize, yMinRockOffset, yMaxRockOffset, rockHeightRange, rockClearingScale);

            if (hasSmallRocks)
                CubeGenerate(cubes[i], smallRocks, minSmallRockRotation, maxSmallRockRotation, maxSmallRocks, smallRockProbability, minSmallRockSize, maxSmallRockSize, yMinSmallRockOffset, yMaxSmallRockOffset, smallRockHeightRange, smallRockClearingScale);

            if (hasTrees)
                CubeGenerate(cubes[i], trees, minTreesRotation, maxTreesRotation, maxTrees, treesProbability, minTreesize, maxTreesize, yMinTreesOffset, yMaxTreesOffset, treeHeightRange, treeClearingScale);

            if(hasCattails)
                CubeGenerate(cubes[i], cattails, minCattailsRotation, maxCattailsRotation, maxCattails, cattailsProbability, minCattailssize, maxCattailssize, yMinCattailsOffset, yMaxCattailsOffset, cattailHeightRange, cattailsClearingScale);

            if(hasGrass)
                CubeGenerate(cubes[i], grass, minGrassRotation, maxGrassRotation, maxGrass, grassProbability, minGrassSize, maxGrassSize, yMinGrassOffset, yMaxGrassOffset, grassHeightRange, grassClearingScale);

            if (hasMiscPlants)
                CubeGenerate(cubes[i], miscPlants, minMiscPlantsRotation, maxMiscPlantsRotation, maxMiscPlants, miscPlantsProbability, minMiscPlantsize, maxMiscPlantssize, yMinMiscPlantsOffset, yMaxMiscPlantsOffset, miscPlantsHeightRange, miscPlantsClearingScale);

            

            if (i % cubesPerFrame == 0)
                yield return null;
        }

        tc.completedGeneration[biomeID - 1] = true;
    }
}
