using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mothership : BasicEnemy
{
    public static Mothership Instance;

    [SerializeField] private float distFromPlayer;
    [SerializeField] private float rotationSpeed;

    [Header("Mothership Delay")]
    [SerializeField] private float startTimer;
    [SerializeField] private float startWarpDelay;
    [SerializeField] private float minWarpDelay;
    [SerializeField] private float difficultyDelayMod = 0.05f;
    [HideInInspector] public float totalTime;
    [HideInInspector] public float elapsedTime;

    protected override void Awake()
    {
        base.Awake();

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        HideMothership();

        elapsedTime = 0;
        totalTime = startTimer / GameController.DifficultyMod;
    }

    public void HideMothership()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        spawned = false;
    }
    public void ShowMothership()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        spawned = true;

        transform.position = PlayerController.Instance.transform.position + (Vector3.zero - PlayerController.Instance.transform.position).normalized * transform.localScale.x * 25;
    }

    private void Update()
    {
        if (GameController.Instance.paused)
            return;

        if (!spawned)
        {
            elapsedTime += Time.deltaTime * (1 + GameController.Instance.completedLevels * difficultyDelayMod);

            if (elapsedTime >= totalTime)
            {
                elapsedTime = totalTime;
                ShowMothership();
            }
            else
            {
                return;
            }
        }

        Fire();
    }

    protected override void FixedUpdate()
    {
        if (!spawned)
            return;

        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) > distFromPlayer)
            HandleMovement();

        rb.AddTorque(transform.up * rotationSpeed, ForceMode.Impulse);
    }

    public void OnNextLevel()
    {
        if (spawned)
            HideMothership();

        totalTime += minWarpDelay + (startWarpDelay - minWarpDelay) * 1 / Mathf.Pow(GameController.Instance.completedLevels, 0.5f);
    }

    protected override void Death(GameObject source)
    {
        LevelUI.Instance.ToggleMenu(Menu.win);

        GameController.Instance.SetPause(true, false);

        base.Death(source);
    }
}
