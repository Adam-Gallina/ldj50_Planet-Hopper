using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : Shield
{
    protected override void Update()
    {
        base.Update();

        if (!rechargingShield)
            coll.enabled = Vector3.Distance(PlayerController.Instance.transform.position, transform.position) > shieldRadius;
    }
}
