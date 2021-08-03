using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeathTypes
{
    public static void Die(DamageTaker dt)
    {
        switch(dt.id)
        {
            case 0:
                RockDeath(dt);
                break;

            case 1:
                TreeDeath(dt);
                break;
        }


    }

    static void RockDeath(DamageTaker dt)
    {
        DeathWithDrop(dt);
    }
    
    static void TreeDeath(DamageTaker dt)
    {
        DeathWithDrop(dt);
    }

    static void DeathWithDrop(DamageTaker dt)
    {
        Vector3 bounds = ComputeBounds(dt);

        for (int i = 0; i < dt.dropOnDeath.Length; i++)
        {
            for (int j = 0; j < dt.dropMultOnDeath[i] * dt.dropRateSizeMultiplier * dt.GetComponent<MeshRenderer>().bounds.size.magnitude; j++)
            {
                if (Random.Range(0f, 100f) <= dt.dropProbOnDeath[i])
                {
                    Drop(dt.dropOnDeath[i], bounds, dt);
                }
            }
        }

        GameObject part = GameObject.Instantiate(StaticObjects.gm.dustCloud, dt.transform.position, Quaternion.identity);
        float mult = Mathf.Min(8f, dt.GetComponent<MeshRenderer>().bounds.size.magnitude);
        part.transform.localScale = mult * dt.deathParticleSizeMultiplier * Vector3.one;

        GameObject.Destroy(dt.gameObject);
    }

    static void Drop(GameObject dropObj, Vector3 bounds, DamageTaker dt)
    {
        GameObject drop = GameObject.Instantiate(dropObj, dt.transform.position + new Vector3(Random.Range(-bounds.x / 2, bounds.x / 2), Random.Range(-bounds.y / 2, bounds.y / 2), Random.Range(-bounds.z / 2, bounds.z / 2)), Quaternion.identity);
        drop.transform.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

    }

    public static void DropSeed(DamageTaker dt)
    {
        Vector3 bounds = ComputeBounds(dt);
        for (int i = 0; i < dt.dropOnDeath.Length; i++)
        {
            if(dt.dropOnDeath[i].GetComponent<PlantGrowth>() != null)
            {
                for (int j = 0; j < dt.dropMultOnDeath[i]; j++)
                {
                    if (Random.Range(0f, 100f) <= dt.dropProbOnDeath[i])
                    {
                        Drop(dt.dropOnDeath[i], bounds, dt);
                    }
                }
            }
        }
    }

    public static Vector3 ComputeBounds(DamageTaker dt)
    {
        return dt.gameObject.GetComponent<MeshRenderer>().bounds.size;
    }
}
