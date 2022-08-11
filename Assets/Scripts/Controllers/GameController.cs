using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public static float DifficultyMod = 1;

    [HideInInspector] public bool paused = false;

    [HideInInspector] public int completedLevels;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Physics.IgnoreLayerCollision(Constants.LaserLayer, Constants.LaserLayer);
        Physics.IgnoreLayerCollision(Constants.EnvironmentLayer, Constants.ShieldLayer);
        Physics.IgnoreLayerCollision(Constants.EnemyLayer, Constants.ShieldLayer);
        Physics.IgnoreLayerCollision(Constants.PlayerLayer, Constants.ShieldLayer);
        Physics.IgnoreLayerCollision(Constants.ShieldLayer, Constants.ShieldLayer);
        Physics.IgnoreLayerCollision(Constants.MothershipLayer, Constants.EnvironmentLayer);

        if (SceneManager.GetActiveScene().buildIndex == Constants.LevelScene)
            LevelController.Instance.StartLevel();
    }

    public void NextLevel()
    {
        StartCoroutine(LoadScene(Constants.LevelScene, LevelController.Instance.StartLevel));
        SetPause(false, false);
        completedLevels++;

        Stats.Instance.planetsVisited++;

        Mothership.Instance?.OnNextLevel();
        PlayerController.Instance?.CheckForBreak();
    }

    public void Restart()
    {
        Destroy(PlayerController.Instance.gameObject);
        Destroy(Mothership.Instance.gameObject);
        Destroy(Stats.Instance.gameObject);
        NextLevel();
        completedLevels = 0;
        SetPause(false, false);
    }

    public void ReturnToMain()
    {
        Destroy(PlayerController.Instance.gameObject);
        Destroy(Mothership.Instance.gameObject);
        Destroy(Stats.Instance.gameObject);
        StartCoroutine(LoadScene(Constants.MainMenuScene, () => Destroy(gameObject)));
        SetPause(false, false);
    }

    private IEnumerator LoadScene(int targetScene)
    {
        AsyncOperation scene = SceneManager.LoadSceneAsync(targetScene);
        yield return new WaitUntil(() => scene.isDone);
    }
    private IEnumerator LoadScene(int targetScene, UnityAction callback)
    {
        yield return LoadScene(targetScene);

        callback.Invoke();
    }

    public void TogglePause(bool updateMenu = true)
    {
        SetPause(!paused, updateMenu);
    }

    public void SetPause(bool pauseState, bool updateMenu = true)
    {
        paused = pauseState;

        Time.timeScale = paused ? 0 : 1;

        if (updateMenu)
            LevelUI.Instance.ToggleMenu(Menu.pause);
        else
            LevelUI.Instance.SetMenu(Menu.pause, false);
    }
}
