using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollection : MonoBehaviour
{
    public string itemName;
    public int quantity = 1;
    public bool canCollect = true;
    public float canCollectDelay = 1f;
    public float lerpSpeed = 3f;
    Item thisItem;

    public Vector3 rotationOffset, positionOffset;

    void OnEnable()
    {
        StartCoroutine(Startup());
    }

    IEnumerator CollectMe(GameObject collector)
    {
        if (GetComponent<Rigidbody>() != null)
            GetComponent<Rigidbody>().isKinematic = true;

        while(Vector3.Distance(transform.position, collector.transform.position) > 0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, collector.transform.position, lerpSpeed * Time.deltaTime);
            yield return null;
        }


        collector.transform.parent.GetComponent<Inventory>().GiveItem(itemName, quantity);
        canCollect = false;
        Destroy(gameObject);
    }

    IEnumerator Startup()
    {
        canCollect = false;
        yield return new WaitForSeconds(canCollectDelay);
        canCollect = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(canCollect && collider.gameObject.tag == "Collector")
        {
            Inventory invObj = collider.gameObject.transform.parent.gameObject.GetComponent<Inventory>();
            Item thisItem = invObj.ItemByName(itemName);

            if (thisItem != null && invObj.CanTakeItem(thisItem))
                StartCoroutine(CollectMe(collider.gameObject));
        }
    }
}
