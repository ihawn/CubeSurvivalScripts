using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaker : MonoBehaviour
{
    public string[] damageTags;
    public float hp, maxHp, toughness, damageTick;
    public bool takeContinuousDamage;
    bool canTakeDamage;
    public int id;

    private void Start()
    {
        hp = maxHp;
        canTakeDamage = true;
    }

    IEnumerator InvulnerabilityCounter()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageTick);
        canTakeDamage = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("collision");
        //object was hit by something that can hurt it
        if (canTakeDamage && System.Array.IndexOf(damageTags, other.gameObject.tag) != -1)
        {
            if (!takeContinuousDamage)
            {
                StartCoroutine(InvulnerabilityCounter());
            }

            TakeDamage(other.gameObject);
            CheckForDeath();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("collision");
        //object was hit by something that can hurt it
        if (canTakeDamage && System.Array.IndexOf(damageTags, collision.gameObject.tag) != -1)
        {
            if(!takeContinuousDamage)
            {
                StartCoroutine(InvulnerabilityCounter());
            }

            TakeDamage(collision.gameObject);
            CheckForDeath();
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
        DeathTypes.Die(id);
    }
}
