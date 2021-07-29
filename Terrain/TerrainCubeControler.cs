using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TerrainCubeControler : MonoBehaviour
{
    Vector3 startPos;
    public TerrainControl theTerrainController;
    public bool generationDone;
    public List<GameObject> childList;
    public int cubeID;

    public GameObject[] closestCubes;


    private void Awake()
    {
        gameObject.layer = 15; 
        startPos = transform.position;


        if (gameObject.tag == "Ground")
            generationDone = false;
        else
            generationDone = true;
    }

    private void Start()
    {
        theTerrainController = StaticObjects.tc;
        closestCubes = new GameObject[(int)Mathf.Round(Mathf.Pow(2 * StaticObjects.tc.proximityLevels - 1, 2))];
        CalculateCubeDistances(ref closestCubes);
    }


    private void Update()
    {

        if ((generationDone && gameObject.tag != "Wall") || (theTerrainController.doneGenerating && gameObject.tag == "Wall"))
        {
            float dist = BirdsEyeDistance(theTerrainController.player, gameObject);

            transform.position = new Vector3(transform.position.x,
                   startPos.y-Mathf.Pow(Mathf.Max(dist, theTerrainController.falloff) - theTerrainController.falloff, theTerrainController.power)/theTerrainController.terrainDrawDistance,
                 transform.position.z);

            if (transform.position.y < startPos.y - theTerrainController.verticalOffset - 1f)
                gameObject.SetActive(false);
        }
    }

    float BirdsEyeDistance(GameObject p, GameObject c)
    {
        return Vector2.Distance(new Vector2(p.transform.position.x, p.transform.position.z), new Vector2(c.transform.position.x, c.transform.position.z));
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<TerrainChild>() != null && !other.gameObject.GetComponent<TerrainChild>().hasParent)
        {
            childList.Add(other.gameObject);
            other.transform.parent = gameObject.transform;
            other.gameObject.GetComponent<TerrainChild>().hasParent = true;
        }
    }


    void CalculateCubeDistances(ref GameObject[] arr)
    {
        Dictionary<GameObject, float> cubeDict = new Dictionary<GameObject, float>();

        int k = 0;
        for (int i = 0; i < theTerrainController.cubes.Length; i++)
        {
            for (int j = 0; j < theTerrainController.cubes[i].Length; j++)
            {
                cubeDict[theTerrainController.cubes[i][j]] = theTerrainController.BirdsEyeDistance(gameObject, theTerrainController.cubes[i][j]);
                k++;

            }
        }

        var shortestCubes = cubeDict.OrderBy(pair => pair.Value).Take(arr.Length).ToDictionary(pair => pair.Key, pair => pair.Value);

        int p = 0;
        foreach (KeyValuePair<GameObject, float> dictEntry in shortestCubes)
        {
            arr[p] = dictEntry.Key;
            p++;
        }

        theTerrainController.completedJobs++;
    }
}
