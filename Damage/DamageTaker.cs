using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaker : MonoBehaviour
{
    public float hp, maxHp, toughness, damageTick;
    public bool takeContinuousDamage;
    public bool canTakeDamage, hpBasedOnSize;
    public int id;

    public GameObject[] dropOnDeath;
    public float[] dropProbOnDeath;
    public float[] dropMultOnDeath;

    private void Start()
    {
        if (hpBasedOnSize)
            hp = 50*GetComponent<Renderer>().bounds.size.magnitude;
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
        Damage(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Damage(collision.gameObject);
    }

    void Damage(GameObject hit)
    {
        if (canTakeDamage && hit.gameObject.GetComponent<DamageGiver>() != null)
        {
            if (!takeContinuousDamage)
            {
                StartCoroutine(InvulnerabilityCounter());
            }

            TakeDamage(hit.gameObject);
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
