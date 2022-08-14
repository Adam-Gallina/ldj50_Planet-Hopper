using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private float followMod;

    private Transform target;

    [Header("Focus")]
    [SerializeField] private Transform focusCam;
    [SerializeField] private float defaultFocusTime = 1;
    private float focusTime;
    [SerializeField] private Vector3 focusPos;
    [SerializeField] private Vector3 focusRot = new Vector3(90, 0, 0);
    private Vector3 defaultPos;
    private Vector3 defaultRot;
    private Vector3 targetPos;
    private Vector3 targetRot;
    private Vector3 startPos;
    private Vector3 startRot;
    private float currFocusTime = 0;
    private bool focusing = false;
    private UnityAction onFocusEnd;

    void Awake()
    {
        Instance = this;

        if (!focusCam)
        {
            Debug.LogWarning("No Zoom camera set up, will be unable to do zoom animations");
            return;
        }

        targetPos = defaultPos = focusCam.localPosition;
        targetRot = defaultRot = focusCam.localEulerAngles;
    }

    private void Update()
    {
        if (target)
        {
            if (focusCam && focusCam.position != targetPos && focusCam.localEulerAngles != targetRot)
            {
                currFocusTime += Time.unscaledDeltaTime;

                float pt = currFocusTime / focusTime;
                if (pt > 1)
                    pt = 1;

                focusCam.localPosition = startPos + (targetPos - startPos) * pt;
                focusCam.localEulerAngles = startRot + (targetRot - startRot) * pt;

                if (pt == 1 && onFocusEnd != null)
                {
                    onFocusEnd.Invoke();
                    onFocusEnd = null;
                }
            }
        }
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
    
    public void FocusOnTarget(Transform t, float zoomHeight, float zoomTime=-1, UnityAction onFocusEnd = null)
    {
        SetTarget(t);
        FocusOnTarget(zoomHeight, zoomTime, onFocusEnd);
    }

    public void FocusOnTarget(float zoomHeight, float zoomTime = -1, UnityAction onFocusEnd=null)
    {
        focusTime = zoomTime != -1 ? zoomTime : defaultFocusTime;

        // If already partway in an animation, don't take the full normal time to return to normal
        if (currFocusTime >= focusTime || currFocusTime == 0)
        {
            startPos = focusCam.localPosition;
            startRot = focusCam.localEulerAngles;
            currFocusTime = 0;
        }
        else 
        {
            this.onFocusEnd?.Invoke();
            if (!focusing)
            {
                startPos = targetPos;
                startRot = targetRot;
                currFocusTime = focusTime - currFocusTime;
            }
        }

        targetPos = new Vector3(focusPos.x, zoomHeight, focusPos.z);
        targetRot = new Vector3(focusRot.x, target.localEulerAngles.y, focusRot.z);
        this.onFocusEnd = onFocusEnd;
        SmoothFocusRot();

        focusing = true;
    }

    public void UnfocusTarget(float zoomTime = -1, UnityAction onFocusEnd = null)
    {
        focusTime = zoomTime != -1 ? zoomTime : defaultFocusTime;

        focusing = false;

        // If already partway in an animation, don't take the full normal time to return to normal
        if (currFocusTime < focusTime)
        {
            startPos = targetPos;
            startRot = targetRot;
            currFocusTime = focusTime - currFocusTime;
        }
        else
        {
            this.onFocusEnd?.Invoke();
            startPos = focusCam.localPosition;
            startRot = focusCam.localEulerAngles;
            currFocusTime = 0;
        }

        targetPos = defaultPos;
        targetRot = defaultRot;
        this.onFocusEnd = onFocusEnd;
        SmoothFocusRot();
    }

    private void SmoothFocusRot()
    {
        if (startRot.y - targetRot.y < -180)
            startRot.y += 360;
        else if (startRot.y - targetRot.y > 180)
            startRot.y -= 360;
    }
}
