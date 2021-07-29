using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitController : MonoBehaviour
{
    public bool wasGrown;
    void Start()
    {
        if(wasGrown)
            StartCoroutine(Grow());
    }

    IEnumerator Grow()
    {
        Vector2 scales = GetComponent<ScaleRange>().scales;
        float scale = Random.Range(scales.x, scales.y);
        transform.localScale = Vector3.zero;

        while(transform.localScale.x < scale - 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, scale * Vector3.one, 2f * Time.deltaTime);
            yield return null;
        }
    }
}
