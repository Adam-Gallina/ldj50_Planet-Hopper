using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgEnemy : BasicEnemy
{
    [SerializeField] private float distToStartAttack;
    [SerializeField] private float distToEndAttack;

    private bool attacking;

    protected override void FixedUpdate()
    {
        if (!attacking || !spawned)
        {
            HandleMovement();

            if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= distToStartAttack)
            {
                attacking = true;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) > distToEndAttack)
                attacking = false;
        }
    }

    protected override void Fire()
    {
        if (attacking)
            base.Fire();
    }
}
