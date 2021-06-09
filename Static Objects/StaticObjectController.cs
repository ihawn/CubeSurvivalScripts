using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObjectController : MonoBehaviour
{
    public float size;
    public bool destroyIfTouchOwnKind;

    private void Start()
    {
        size = Vector3.Magnitude(GetComponent<Renderer>().bounds.size);
        StartCoroutine(DestroyDelay());
    }

    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(0.2f);

     //   DestroyIfTouchingLarger();
    }

    void DestroyIfNearWall()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, size);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.tag == "Wall")
            {

            }
        }
    }

    void DestroyIfTouchingLarger()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, size);

        for(int i = 0; i < hitColliders.Length; i++)
        {
            if(hitColliders[i].GetComponent<StaticObjectController>() != null &&
                hitColliders[i].GetComponent<StaticObjectController>().size > size &&
                hitColliders[i].gameObject.tag != "Ground" &&
                hitColliders[i].gameObject.tag != "Water")
            {
                if((destroyIfTouchOwnKind && hitColliders[i].gameObject.tag == gameObject.tag) ||
                    (!destroyIfTouchOwnKind && hitColliders[i].gameObject.tag != gameObject.tag) ||
                        (destroyIfTouchOwnKind && hitColliders[i].gameObject.tag != gameObject.tag))
                    Destroy(this.gameObject);
            }
        }
    }
}
