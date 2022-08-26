using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProjectileStats
{
    [HideInInspector] public float speed;
    [HideInInspector] public float damage;
    [HideInInspector] public string targetTag;
}

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [HideInInspector] public BulletType type = BulletType.None;

    protected GameObject source;
    protected ProjectileStats stats;

    protected Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(stats.targetTag))
        {
            HealthBase hit = other.GetComponentInParent<HealthBase>();
            if (hit && hit.Damage(gameObject, stats.damage))
                Despawn();
        }
        else if (other.gameObject.layer == Constants.EnvironmentLayer ||
                 other.gameObject.layer == Constants.GroundLayer)
        {
            Despawn();
        }
    }

    public void Initialize(GameObject source, ProjectileStats stats)
    {
        this.source = source;
        this.stats = stats;
    }

    public void SetVelocity(Vector3 dir)
    {
        rb.velocity = dir.normalized * stats.speed;

        if (dir.magnitude > 0)
            transform.forward = dir;
    }

    public void Despawn()
    {
        SetVelocity(Vector3.zero);
        
        BulletPool.Instance.ReturnToBulletPool(this);
    }
}
