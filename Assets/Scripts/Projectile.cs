using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [HideInInspector] public BulletType type = BulletType.None;

    protected GameObject source;
    protected float speed;
    protected float damage;
    protected string targetTag;

    protected Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            HealthBase hit = other.GetComponentInParent<HealthBase>();
            if (hit && hit.Damage(gameObject, damage))
                Despawn();
        }
        else if (other.gameObject.layer == Constants.EnvironmentLayer ||
                 other.gameObject.layer == Constants.GroundLayer)
        {
            Despawn();
        }
    }

    public void Initialize(GameObject source, float speed, float damage, string targetTag)
    {
        this.source = source;
        this.speed = speed;
        this.damage = damage;
        this.targetTag = targetTag;
    }

    public void SetVelocity(Vector3 dir)
    {
        rb.velocity = dir.normalized * speed;

        if (dir.magnitude > 0)
            transform.forward = dir;
    }

    public void Despawn()
    {
        SetVelocity(Vector3.zero);
        
        BulletPool.Instance.ReturnToBulletPool(this);
    }
}
