using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCubeControler : MonoBehaviour
{
    Vector3 startPos;
    public TerrainControl theTerrainController;
    public bool generationDone;
    public List<GameObject> childList;

    private void Awake()
    {
        gameObject.layer = 15; 
        startPos = transform.position;

        if (gameObject.tag == "Ground")
            generationDone = false;
        else
            generationDone = true;
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
        if (other.gameObject.GetComponent<TerrainChild>() != null)
        {
            childList.Add(other.gameObject);
            other.transform.parent = gameObject.transform;
        }
    }
}
