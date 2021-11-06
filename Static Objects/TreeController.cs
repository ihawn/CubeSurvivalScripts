using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    DamageTaker dt;
    public LeafDrop[] leafDrops;
    public GameObject deadVersion;
    public float lifeTimeMin = 100f, lifeTimeMax = 150f, startTime, endTime, lastRememberedTime;
    public bool initialized, shouldDieOnEnable;
    float lifeTime;


    void Start()
    {
        for (int i = 0; i < leafDrops.Length; i++)
            leafDrops[i].gameObject.SetActive(false);

        dt = GetComponent<DamageTaker>();

        StaticObjects.gm.agingTrees.Add(this);
    }


    public void InitializeTree()
    {
        lifeTime = Random.Range(lifeTimeMin, lifeTimeMax);
        startTime = StaticObjects.gm.time;
        endTime = startTime + lifeTime;
        lastRememberedTime = StaticObjects.gm.time;
        initialized = true;
    }

    //When the tree turns into a dead tree
    public void TreeDeath()
    {
        if (gameObject.activeInHierarchy)
            VisableTreeDeath(true);
    }


    void VisableTreeDeath(bool hasEffects)
    {   
        
        if(deadVersion != null && transform.parent != null) //This is the alive version
        {
            //Make dead version
            GameObject deadTree = Instantiate(deadVersion, transform.position, transform.rotation);
            deadTree.transform.parent = transform.parent;
            deadTree.transform.localScale = transform.localScale;
            StaticObjects.gm.agingTrees.Add(deadTree.GetComponent<TreeController>());

            //Drop seeds
            dt = GetComponent<DamageTaker>();
            DeathTypes.DropSeed(dt);

            //Drop leaves
            if (hasEffects)
            {
                for(int i = 0; i < leafDrops.Length; i++)
                {
                    leafDrops[i].gameObject.SetActive(true);
                    leafDrops[i].transform.parent = null;
                }
            }
        }

        Destroy(gameObject);
    }


}
