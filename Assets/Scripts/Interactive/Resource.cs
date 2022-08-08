using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { Metal, Electronics, Uranium }
public class Resource : HealthBase
{
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int resourceValue = 1;

    protected override void Death(GameObject source)
    {
        PlayerController.Instance.AddResource(resourceType, resourceValue);

        Stats.Instance.resourcesCollected++;

        base.Death(source);
    }
}

