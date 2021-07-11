using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaker : MonoBehaviour
{
    public float hp, maxHp, toughness, damageTick;
    public bool takeContinuousDamage;
    public bool canTakeDamage, hpBasedOnSize, hasDebris;
    public int id;

    public GameObject[] dropOnDeath;
    public float[] dropProbOnDeath;
    public float[] dropMultOnDeath;

    public float damageSpeedThreshold = 8f, healthBarOffset, deathParticleSizeMultiplier, dropRateSizeMultiplier;

    GameObject healthBar;

    private void Start()
    {
        if (hpBasedOnSize)
        {
            hp = 50 * GetComponent<Renderer>().bounds.size.magnitude;
            maxHp = hp;
        }
        else
            hp = maxHp;
        canTakeDamage = true;
    }

    IEnumerator InvulnerabilityCounter()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageTick);
        canTakeDamage = true;
    }

    private void Update()
    {
        CheckForDeath();
    }

    private void OnTriggerEnter(Collider other)
    {
        Damage(other.gameObject, other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position));
    }

    private void OnCollisionEnter(Collision collision)
    {
        Damage(collision.gameObject, collision.contacts[0].point);
    }

    void Damage(GameObject hit, Vector3 hitPoint)
    {
        if (canTakeDamage && hit.gameObject.GetComponent<DamageGiver>() != null &&
            ((hit.GetComponent<DamageGiver>().speed >= damageSpeedThreshold && !hit.GetComponent<DamageGiver>().overrideSpeedThreshold) || 
            (hit.GetComponent<DamageGiver>().overrideSpeedThreshold && hit.GetComponent<DamageGiver>().speed >= hit.GetComponent<DamageGiver>().overriddenSpeedThreshold)) 
            && !hit.GetComponent<DamageGiver>().dontGiveDamage)
        {
            if (!takeContinuousDamage)
            {
                StartCoroutine(InvulnerabilityCounter());
            }

            TakeDamage(hit.gameObject);

            if(hasDebris)
            {
                Vector3 debrisDirection = (hit.gameObject.transform.position - transform.position).normalized*360f;
                Quaternion debRot = Quaternion.Euler(debrisDirection + new Vector3(0f, 90f, 0f));
                GameObject debris = Instantiate(StaticObjects.player.debrisParticles, hitPoint, debRot);
            }


            if(healthBar == null)
            {
                Vector3 pos = Vector3.zero;
                RaycastHit hitRay;
                Vector3 rayPos = transform.position + new Vector3(0f, GetComponent<MeshRenderer>().bounds.size.magnitude, 0f);

                if (Physics.Raycast(rayPos, transform.TransformDirection(Vector3.down), out hitRay, Mathf.Infinity))
                {
                    pos = hitRay.point;
                    Debug.DrawRay(rayPos, transform.TransformDirection(Vector3.down) * hitRay.distance, Color.yellow);
                }
                else
                    Debug.Log("Healthbar raycast failed");

                pos += new Vector3(0f, healthBarOffset, 0f);

                pos = new Vector3((hit.transform.position.x + hitPoint.x) / 2, hit.transform.position.y + 2f, (hit.transform.position.z + hitPoint.z) / 2);

                healthBar = Instantiate(StaticObjects.gm.enemyHealthBar, pos, Quaternion.identity);
                healthBar.GetComponent<HeathBarController>().whoCreatedMe = gameObject;
                healthBar.transform.parent = transform;
                
            }


            healthBar.GetComponent<HeathBarController>().SetHealth(hp, maxHp);
        }
    }


    void TakeDamage(GameObject g)
    {
        hp -= g.GetComponent<DamageGiver>().dph / toughness;
    }

    void CheckForDeath()
    {
        if (hp <= 0)
            Death();
    }

    void Death()
    {
        DeathTypes.Die(this);
    }
}
