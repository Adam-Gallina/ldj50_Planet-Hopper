using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Dropdown difficulty;
    [SerializeField] private float[] difficultyScaling = new float[] { 0.75f, 1, 2 };
    [SerializeField] private Button[] tutorialButtons;
    [SerializeField] private GameObject[] tutorialPanels;

    public void StartBtn()
    {
        GameController.DifficultyMod = difficultyScaling[difficulty.value];

        SceneManager.LoadScene(Constants.LevelScene);
    }

    public void QuitBtn()
    {
        Application.Quit();
    }

    public void ChangeTab(int tab)
    {
        for (int i = 0; i < tutorialButtons.Length; i++)
        {
            tutorialButtons[i].interactable = i != tab;
            tutorialPanels[i].SetActive(i == tab);
        }
    }
}
