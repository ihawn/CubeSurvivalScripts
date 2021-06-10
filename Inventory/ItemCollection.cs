using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollection : MonoBehaviour
{
    public string itemName;
    public int quantity = 1;
    public bool canCollect = true;

    public Vector3 rotationOffset, positionOffset;

    void OnEnable()
    {
        canCollect = true;
    }

    IEnumerator CollectMe(GameObject collector)
    {
        collector.transform.parent.GetComponent<Inventory>().GiveItem(itemName, quantity);
        canCollect = false;
        Destroy(gameObject);
        
        yield return null;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(canCollect && collider.gameObject.tag == "Collector")
        {
            StartCoroutine(CollectMe(collider.gameObject));
        }
    }
}
