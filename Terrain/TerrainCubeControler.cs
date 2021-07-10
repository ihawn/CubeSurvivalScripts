using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCubeControler : MonoBehaviour
{
    Vector3 startPos;
    public TerrainControl theTerrainController;
    Coroutine upCo, downCo;
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

    private void OnEnable()
    {
        if (downCo != null)
            StopCoroutine(downCo);

        //upCo = StartCoroutine(MoveToPosition());   
    }


    private void Update()
    {
        if (/*theTerrainController.doneGenerating &&*/ generationDone &&
            BirdsEyeDistance(theTerrainController.player, gameObject) > theTerrainController.terrainDrawDistance && 
            Vector3.Distance(transform.position, startPos) < 0.001f)
        {
            

            if (upCo != null)
                StopCoroutine(upCo);

            //downCo = StartCoroutine(MoveDown());
        }

        if (generationDone)
        {
            float dist = BirdsEyeDistance(theTerrainController.player, gameObject);

            transform.position = new Vector3(transform.position.x,
                   startPos.y-Mathf.Pow(Mathf.Max(dist, theTerrainController.falloff) - theTerrainController.falloff, 2)/theTerrainController.terrainDrawDistance,
                 transform.position.z);

            if (transform.position.y < startPos.y - theTerrainController.verticalOffset - 1f)
                gameObject.SetActive(false);
        }
    }

    float BirdsEyeDistance(GameObject p, GameObject c)
    {
        return Vector2.Distance(new Vector2(p.transform.position.x, p.transform.position.z), new Vector2(c.transform.position.x, c.transform.position.z));
    }

   /* IEnumerator MoveToPosition()
    {
        while(Vector3.Distance(startPos, transform.position) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime * theTerrainController.terrainRiseSpeed);
            yield return null;
        }

        transform.position = startPos;

        foreach(GameObject g in childList)
        {

            if (g.GetComponent<TerrainChild>().hasRigidbody)
            {
                g.AddComponent<Rigidbody>();

                if(g.GetComponent<TerrainChild>().hasKinematicRigidbody)
                {
                    g.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
        }
    }

    IEnumerator MoveDown()
    {
        Vector3 pos = startPos - new Vector3(0f, theTerrainController.verticalOffset, 0f);

        while (Vector3.Distance(pos, transform.position) > 1f)
        {
            transform.position = Vector3.Lerp(transform.position, pos, theTerrainController.terrainRiseSpeed * Time.deltaTime);
            yield return null;
        }

        foreach(GameObject g in childList)
        {

            if(g.GetComponent<Rigidbody>() != null)
            {

                Destroy(g.GetComponent<Rigidbody>());
            }
        }

        transform.position = pos;
        gameObject.SetActive(false);
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<TerrainChild>() != null)
        {
            childList.Add(other.gameObject);
            other.transform.parent = gameObject.transform;
        }
    }
}
