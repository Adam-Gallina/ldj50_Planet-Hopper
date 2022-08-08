using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class MovementBase : CombatBase
{
    [Header("Movement")]
    [SerializeField] protected float moveSpeed;


    protected Rigidbody rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    protected abstract void HandleMovement();

    public void SetVelocity(Vector3 dir, float power)
    {
        rb.velocity = dir.normalized * power;
    }

    public void AddForce(Vector3 dir, float power, ForceMode mode=ForceMode.VelocityChange)
    {
        rb.AddForce(dir.normalized * power, mode);
    }
}
