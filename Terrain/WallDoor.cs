using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDoor : MonoBehaviour
{
    public bool breakNextFrame;
    public GameObject[] bricks;
    public GameObject wallParent, stairs, keyhole1, keyhole2;
    public float smallScale, scaleSpeed, stairMoveSpeed, stairOffset;
    float stairHeight;
    bool doorOpened = false;
    public float brickMass;
    public string keyholeMessage, keyholeMessage2;
    public bool noDoor;

    private void Start()
    {
        stairHeight = stairs.transform.position.y;
        stairs.transform.position = new Vector3(stairs.transform.position.x, stairHeight - stairOffset, stairs.transform.position.z);
    }

    void Update()
    {
        if (breakNextFrame && !doorOpened && !noDoor)
        {
            StartCoroutine(OpenDoor());
        }

        stairs.transform.parent = wallParent.transform;
    }

    IEnumerator RaiseStairs()
    {
        while(stairs.transform.position.y < stairHeight)
        {
            stairs.transform.position += new Vector3(0f, 1f, 0f) * stairMoveSpeed * (Time.deltaTime);
            yield return null;
        }

    }

    IEnumerator OpenDoor()
    {
        doorOpened = true;

        keyhole1.gameObject.tag = "Untagged";
        keyhole2.gameObject.tag = "Untagged";

        StartCoroutine(RaiseStairs());

        while(bricks[0].transform.localScale.x > smallScale)
        {
            for(int i = 0; i < bricks.Length; i++)
            {
                bricks[i].transform.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;
            }

            yield return null;
        }

        Destroy(wallParent.GetComponent<BoxCollider>());
        wallParent.AddComponent<MeshCollider>();
        for(int i = 0; i < bricks.Length; i++)
        {
            bricks[i].AddComponent<Rigidbody>();
            bricks[i].GetComponent<Rigidbody>().mass = brickMass;
            bricks[i].AddComponent<BoxCollider>();
        }

    }
}
