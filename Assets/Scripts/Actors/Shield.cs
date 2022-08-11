using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : HealthBase
{
    [Header("Shield")]
    [SerializeField] protected bool useParentTag = true;
    public float shieldRechargeRate;
    public float shieldRechargeDelay;
    protected float lastHit;
    protected bool rechargingShield;
    [SerializeField] protected float shieldRadius;
    [SerializeField] protected float shieldHitRadius;

    [Header("Shield Anim")]
    [SerializeField] private float flickerTime;
    [SerializeField] private float unflickerTime;
    [SerializeField] private float minTimeToFlicker;
    [SerializeField] private float maxTimeToFlicker;
    private float nextFlicker;
    [SerializeField] private int flickerCount;
    private bool flickering = false;

    private Renderer r;
    protected SphereCollider coll;

    private void OnValidate()
    {
        transform.localScale = new Vector3(shieldRadius, shieldRadius, shieldRadius) * 2;
        if (useParentTag && transform.parent)
            gameObject.tag = transform.parent.tag;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, shieldRadius - shieldHitRadius);
    }

    private void Awake()
    {
        r = GetComponent<Renderer>();
        r.enabled = false;

        nextFlicker = Time.time + Random.Range(minTimeToFlicker, maxTimeToFlicker);

        coll = GetComponent<SphereCollider>();
    }

    protected virtual void Update()
    {
        coll.enabled = !rechargingShield;

        if (Time.time > nextFlicker)
        {
            nextFlicker = Time.time + Random.Range(minTimeToFlicker, maxTimeToFlicker);
            Flicker();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (currHealth < maxHealth && Time.time >= lastHit + shieldRechargeDelay && shieldRechargeDelay != -1)
        {
            RechargeShield();
            if (currHealth >= maxHealth)
            {
                currHealth = maxHealth;

                if (rechargingShield)
                {
                    rechargingShield = false;
                }
            }
        }
    }

    protected virtual void RechargeShield()
    {
        currHealth += shieldRechargeRate;
    }

    private void Flicker()
    {
        if (!flickering && !rechargingShield)
            StartCoroutine(DoFlicker());
    }

    private IEnumerator DoFlicker()
    {
        flickering = true;

        for (int i = 0; i < flickerCount; i++)
        {
            r.enabled = true;
            yield return new WaitForSeconds(flickerTime);
            r.enabled = false;
            yield return new WaitForSeconds(unflickerTime);
        }

        flickering = false;
    }

    public override bool Damage(GameObject source, float damage)
    {
        if (Vector3.Distance(transform.position, source.transform.position) < shieldRadius - shieldHitRadius)
            return false;

        lastHit = Time.time;
        Flicker();

        currHealth -= damage;
        if (currHealth <= 0)
        {
            currHealth = 0;
            rechargingShield = true;
            coll.enabled = false;
        }

        return true;
    }
}

