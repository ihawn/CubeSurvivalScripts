using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour
{
    Animator anim;

    public float distanceToGround, heightOffset;
    public LayerMask layerMask;


    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (anim)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKLeftFootWeight"));
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKLeftFootWeight"));
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKRightFootWeight"));
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKRightFootWeight"));

            //Left foot
            SetFootPosition(AvatarIKGoal.LeftFoot);

            //Right foot
            SetFootPosition(AvatarIKGoal.RightFoot);
        }
    }

    void SetFootPosition(AvatarIKGoal foot)
    {


        RaycastHit hit;

        Ray ray = new Ray(anim.GetIKPosition(foot) + Vector3.up * heightOffset, Vector3.down);

        bool hitGround = false;

        Debug.DrawRay(anim.GetIKPosition(foot) + Vector3.up * heightOffset, Vector3.down, Color.red);

        if (Physics.Raycast(ray, out hit, distanceToGround + heightOffset, layerMask))
        {
            for (int j = 0; j < StaticObjects.player.tagsConsideredGround.Length; j++)
                if (StaticObjects.player.tagsConsideredGround[j] == hit.transform.tag)
                {
                    hitGround = true;
                }

            if (hitGround)
            {
                Vector3 footPosition = hit.point;
                footPosition.y += distanceToGround;
                anim.SetIKPosition(foot, footPosition);
                anim.SetIKRotation(foot, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation); //Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation);
            }
        }
        
    }

}
//0.66, 2.22
//0.3, 2.34