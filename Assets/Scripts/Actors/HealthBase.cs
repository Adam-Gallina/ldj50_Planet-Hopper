using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthBase : MonoBehaviour
{
    [Header("Basic Stats")]
    public float maxHealth;
    [HideInInspector] public float currHealth;

    protected virtual void Start()
    {
        currHealth = maxHealth;
    }


    public virtual bool Damage(GameObject source, float damage)
    {
        currHealth -= damage;

        if (currHealth <= 0)
        {
            Death(source);
        }

        return true;
    }

    protected virtual void Death(GameObject source)
    {
        Destroy(gameObject);
    }
}
