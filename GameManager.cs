using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject enemyHealthBar;
    public GameObject sun;
    public GameObject dustCloud;
    public float dayLength = 1440f; //24 minutes

    public List<PlantGrowth> growingSeeds;
    public List<TreeController> agingTrees;
    public int treesPerFrame;

    public float timeMultiplier = 1f, time = 0f;


    private void Awake()
    {
        StartCoroutine(UpdateTrees());
        StartCoroutine(TreeListGarbageCollector());
    }

    private void Update()
    {
        time += timeMultiplier * Time.deltaTime;

        UpdateSeedTimers();
        UpdateSun();
    }

    void UpdateSun()
    {
        sun.transform.Rotate(Vector3.right * Time.deltaTime * timeMultiplier* 360f / dayLength);
    }

    void UpdateSeedTimers()
    {
        for(int i = 0; i < growingSeeds.Count; i++)
        {
            PlantGrowth seed = growingSeeds[i];

            if (seed == null)
                growingSeeds.Remove(seed);

            //Check if time is expired
            if(time >= seed.endTime)
            {
                if (seed != null)
                {
                    if (seed.gameObject.activeInHierarchy)
                    {
                        seed.StartCoroutine(seed.Grow());
                    }
                    else
                    {
                        seed.treeInstantly = true;
                    }

                    growingSeeds.Remove(seed);
                }
            }
        }
    }

    IEnumerator UpdateTrees()
    {
        while (true)
        {
            for (int i = 0; i < agingTrees.Count; i++)
            {
                if (agingTrees[i] != null)
                {
                    if (!agingTrees[i].isActiveAndEnabled)
                    {
                        agingTrees[i].endTime += time - agingTrees[i].lastRememberedTime;
                        agingTrees[i].lastRememberedTime = time;
                    }

                    if (!agingTrees[i].initialized)
                        agingTrees[i].InitializeTree();


                    if (time >= agingTrees[i].endTime)
                        agingTrees[i].TreeDeath();
                }

                if(i % treesPerFrame == 0)
                    yield return null;
            }

            yield return null;
        }
    }

    IEnumerator TreeListGarbageCollector()
    {
        while (true)
        {
            for (int i = 0; i < agingTrees.Count; i++)
            {
                if (agingTrees[i] == null)
                {
                    agingTrees.RemoveAt(i);
                    yield return null;
                }
            }

            yield return null;
        }

    }
}
