using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private float followMod;

    private Transform target;

    void Awake()
    {
        Instance = this;
    }

    void FixedUpdate()
    {
        if (target)
        {
            transform.position += (target.position - transform.position) * followMod;
        }
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }
}
