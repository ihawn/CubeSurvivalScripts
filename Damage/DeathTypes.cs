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
        }
    }

    static void RockDeath(DamageTaker dt)
    {
        Vector3 bounds = dt.gameObject.GetComponent<MeshRenderer>().bounds.size;

        for(int i = 0; i < dt.dropOnDeath.Length; i++)
        {
            for(int j = 0; j < dt.dropMultOnDeath[i]; j++)
            {
                if(Random.Range(0f,100f) <= dt.dropProbOnDeath[i])
                {
                    GameObject drop = GameObject.Instantiate(dt.dropOnDeath[i], dt.transform.position + new Vector3(Random.Range(-bounds.x / 2, bounds.x / 2), Random.Range(-bounds.y / 2, bounds.y / 2), Random.Range(-bounds.z / 2, bounds.z / 2)), Quaternion.identity);
                }
            }
        }

        GameObject.Destroy(dt.gameObject);
    }
}
