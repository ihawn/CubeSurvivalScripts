using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    GameManager gm;

    GameObject whatImGrowingOn;

    public GameObject[] whatDoIGrowInto;
    public float timeToGrowMin, timeToGrowMax, growSpeed, growthOffset;
    public float startTime, endTime;
    public bool grounded, treeInstantly, growing;
    float timeToGrow;

    public float minHeight;

    void Start()
    {
        gm = StaticObjects.gm;
        timeToGrow = Random.Range(timeToGrowMin, timeToGrowMax);
    }

    private void OnEnable()
    {
        if (treeInstantly)
            StartCoroutine(Grow());
    }

    public IEnumerator Grow()
    {
        FruitDistribution fd = GetComponent<FruitDistribution>();

        growing = true;
        GetComponent<ItemCollection>().canCollect = false;
        GameObject plant = Instantiate(whatDoIGrowInto[Random.Range(0, whatDoIGrowInto.Length)], transform.position - Vector3.up*growthOffset, Quaternion.identity);
        plant.transform.parent = whatImGrowingOn.transform;

        plant.GetComponent<MeshCollider>().enabled = false;

        if (plant.GetComponent<DamageTaker>())
            plant.GetComponent<DamageTaker>().enabled = false;

        plant.transform.localScale = Vector3.zero;
       
        GetComponent<MeshRenderer>().enabled = false;

        Vector2 scales = plant.GetComponent<ScaleRange>().scales;
        float endScale = Random.Range(scales.x, scales.y);
       
        if (treeInstantly) //If the grow timer expired when the parent cube was deactivated, plant should grow instantly
        {
            plant.transform.parent = null;
            plant.transform.localScale = Vector3.one * endScale;
            plant.transform.parent = whatImGrowingOn.transform;
        }
        else
        {
            float lastScale = 0f;
            float currentScale = 0f;
            float movementDelta = Mathf.Infinity;
            while (movementDelta > 0.1f*Time.deltaTime)
            {
                if (plant != null && plant.transform.parent.gameObject != null)
                    plant.transform.parent = null;
                else
                    break;
                lastScale = currentScale;               
                plant.transform.localScale = Vector3.Lerp(plant.transform.localScale, Vector3.one * endScale, growSpeed * Time.deltaTime);             
                currentScale = plant.transform.localScale.x;
                movementDelta = Mathf.Abs(currentScale - lastScale);
                plant.transform.parent = whatImGrowingOn.transform;

                yield return null;
            }
        }

        if (plant != null)
        {
            plant.GetComponent<MeshCollider>().enabled = true;

            plant.GetComponent<DamageTaker>().enabled = true;
            plant.GetComponent<DamageTaker>().startScale = transform.localScale.x;
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && !growing && transform.position.y >= minHeight)
        {
            whatImGrowingOn = collision.gameObject;
            grounded = true;
            startTime = gm.time;
            endTime = startTime + timeToGrow;
            gm.growingSeeds.Add(this);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = false;
            gm.growingSeeds.Remove(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       /* if (grounded && other.gameObject.tag != "Player" && other.gameObject.tag != "Ground" && !growing)
            gm.growingSeeds.Remove(this);*/
    }

}
