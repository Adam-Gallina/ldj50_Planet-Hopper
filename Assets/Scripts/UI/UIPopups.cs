using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopups : MonoBehaviour
{
    public static UIPopups Instance;

    [Header("Mining Indicator")]
    [SerializeField] private GameObject miningIndicator;
    [SerializeField] private Slider miningPercent;
    private Resource miningTarget;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdateMiningIndicator();
    }

    public void SetMiningIndicator(Resource target)
    {
        miningTarget = target;
    }

    private void UpdateMiningIndicator()
    {
        if (miningTarget)
        {
            miningIndicator.SetActive(true);

            miningIndicator.transform.position = Camera.main.WorldToScreenPoint(miningTarget.transform.position);
            miningPercent.value = miningTarget.currHealth / miningTarget.maxHealth;
        }
        else
        {
            miningIndicator.SetActive(false);
        }
    }
}
