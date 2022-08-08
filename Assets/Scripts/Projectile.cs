using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] protected float speed;

    protected CombatBase source;
    protected float damage;
    protected string targetTag;

    protected Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == targetTag
            || other.gameObject.layer == Constants.ShieldLayer)
        {
            HealthBase hit = other.GetComponentInParent<HealthBase>();
            if (hit && hit.Damage(source.gameObject, damage))
                Despawn();
        }
        else if (other.gameObject.layer == Constants.EnvironmentLayer ||
                 other.gameObject.layer == Constants.GroundLayer)
        {
            Despawn();
        }
    }

    public void Initialize(CombatBase source, string targetTag)
    {
        this.source = source;
        this.targetTag = targetTag;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
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

        source.ReturnToBulletPool(this);
    }
}
