using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject enemyHealthBar;
    public GameObject dustCloud;

    public List<PlantGrowth> growingSeeds;

    public float timeMultiplier = 1f, time = 0f;

    private void Update()
    {
        time += timeMultiplier * Time.deltaTime;

        UpdateSeedTimers();
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
}
