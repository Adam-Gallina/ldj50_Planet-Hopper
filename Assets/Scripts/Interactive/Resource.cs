using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { Metal, Electronics, Uranium, WeaponMod }
public class Resource : HealthBase
{
    [SerializeField] protected ResourceType resourceType;
    [SerializeField] protected int resourceValue = 1;

    protected virtual void OnCollect()
    {
        PlayerController.Instance.manager.AddResource(resourceType, resourceValue);

        Stats.Instance.resourcesCollected++;
    }

    protected override void Death(GameObject source)
    {
        OnCollect();

        base.Death(source);
    }
}

