using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitDistribution : MonoBehaviour
{
    public float radius = 5f;
    public GameObject[] fruits;
    List<GameObject> allFruits = new List<GameObject>();
    List<float> fruitSizes = new List<float>();
    public Vector2[] fruitCountRanges;
    public Vector2[] fruitSizeRanges;
    public float fruitDelay = 3f;
    public int[] fruitCounts;
    DamageTaker dt;
    float maxScale;

    bool generatedFruit;


    private void OnEnable()
    {
        if (!generatedFruit)
            StartCoroutine(InitializeFruit());
    }


    void GenerateFruit(float count, GameObject fruit, int id)
    {

        for(int i = 0; i < count; i++)
        {
            RaycastHit hit;
            Vector3 rayPoint = Meth.RandomPointOnSphere(radius, transform.position.x, transform.position.y, transform.position.z);
            if (Physics.Linecast(rayPoint, transform.position, out hit))
            {
                if(hit.transform.gameObject.tag == "Vegetation")
                {
                    fruitCounts[id]++;

                    if (!dt)
                        dt.dropMultOnDeath[id] = fruitCounts[id];

                    GameObject fruitPiece = Instantiate(fruit, hit.point, Quaternion.identity);
                    fruitPiece.GetComponent<FruitController>().wasGrown = true;
                    fruitPiece.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
                    fruitPiece.transform.parent = transform;
                    fruitPiece.transform.localScale = Vector3.zero;
                    allFruits.Add(fruitPiece);

                    Vector2 scales = fruitPiece.GetComponent<ScaleRange>().scales;
                }
            }
        }

       

    }

    IEnumerator InitializeFruit()
    {
        yield return new WaitForSeconds(fruitDelay);

        fruitCounts = new int[fruits.Length];

        dt = gameObject.GetComponent<DamageTaker>();

        if (!dt)
            dt = gameObject.transform.parent.gameObject.GetComponent<DamageTaker>();

        if (!dt)
        {
            for (int i = 0; i < dt.dropProbOnDeath.Length; i++)
            {
                dt.dropProbOnDeath[i] = 100;
            }
        }

        for (int i = 0; i < fruits.Length; i++)
            GenerateFruit(Random.Range(fruitCountRanges[i].x, fruitCountRanges[i].y), fruits[i], i);

        generatedFruit = true;
    }


}
