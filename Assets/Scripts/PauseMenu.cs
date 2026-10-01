// PauseMenuManager.cs
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [Tooltip("Перетащи сюда черную полупрозрачную панель-фон")]
    public GameObject backgroundFadePanel;

    [Tooltip("Перетащи сюда панель или объект, содержащий кнопки")]
    public GameObject buttonsPanel;

    [Header("Настройки")]
    [Tooltip("Клавиша для вызова паузы")]
    public KeyCode pauseKey = KeyCode.Escape;
    [Tooltip("Точное название сцены главного меню")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Start()
    {
        // Убедимся, что в начале игры все выключено, а время идет
        isPaused = false;
        if (backgroundFadePanel != null) backgroundFadePanel.SetActive(false);
        if (buttonsPanel != null) buttonsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        // Включаем оба объекта: и фон, и кнопки
        if (backgroundFadePanel != null) backgroundFadePanel.SetActive(true);
        if (buttonsPanel != null) buttonsPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        // Выключаем оба объекта
        if (backgroundFadePanel != null) backgroundFadePanel.SetActive(false);
        if (buttonsPanel != null) buttonsPanel.SetActive(false);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("SceneFader не найден! Выполняется простой переход.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}