using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] CanvasGroup gameOverCanvasGroup;
    [SerializeField] Button gameOverRetryButton;
    [SerializeField] Button gameOverMenuButton;
    [SerializeField] Snek snek;

    [SerializeField] string menuSceneName;

    void Awake()
    {
        EnableGameOverScreen(false);
        gameOverRetryButton.onClick.AddListener(Retry);
        snek.GameOverEvent += () => EnableGameOverScreen(true);
        gameOverMenuButton.onClick.AddListener(Menu);
    }

    void EnableGameOverScreen(bool enable)
    {
        gameOverCanvasGroup.interactable = enable;
        gameOverCanvasGroup.alpha = enable ? 1 : 0;
    }

    void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Menu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
