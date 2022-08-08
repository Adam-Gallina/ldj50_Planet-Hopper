using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerEnemy : BasicEnemy
{
    [Header("Scanning")]
    [SerializeField] private float chargeDelay;
    [SerializeField] private float scanDelay;
    [SerializeField] private float timeRemoval;
    [SerializeField] private float scanCooldown;
    [SerializeField] private float startScanDist;
    [SerializeField] private float maxScanDist;
    private float scanStart;
    private bool scanning = false;
    [SerializeField] private Transform scanSource;
    [SerializeField] private LineRenderer scanLine;

    protected override void Fire()
    {
        float toPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
        if (scanning)
        {
            if (toPlayer > maxScanDist)
            {
                scanning = false;
                scanLine.enabled = false;
                return;
            }

            if (Time.time > scanStart)
            {
                scanLine.enabled = true;
                scanLine.SetPositions(new Vector3[] { scanSource.position, PlayerController.Instance.transform.position + new Vector3(0, Random.Range(-3, 3), 0) });

                if (Time.time >= scanStart + scanDelay)
                {
                    scanning = false;
                    scanLine.enabled = false;

                    Mothership.Instance.elapsedTime += timeRemoval;
                    spawned = false;
                    spawnEnd = Time.time + scanCooldown;
                }
            }
        }
        else
        {
            if (toPlayer < startScanDist)
            {
                scanStart = Time.time + chargeDelay;
                scanning = true;
            }
        }
    }
}
