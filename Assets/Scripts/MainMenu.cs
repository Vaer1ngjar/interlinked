// MainMenu.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Обязательно для работы со сценами

public class MainMenu : MonoBehaviour
{
    [Header("Настройки Сцен")]
    [Tooltip("Точное название сцены, которая идет после меню (например, 'Briefing')")]
    public string firstSceneName = "Briefing";

    /// <summary>
    /// Этот метод нужно привязать к кнопке "Play" или "Начать игру".
    /// </summary>
    public void PlayGame()
    {
        // Проверяем, существует ли наш "бессмертный" SceneFader
        if (SceneFader.Instance != null)
        {
            // Если да, используем его для красивого перехода
            SceneFader.Instance.FadeToScene(firstSceneName);
        }
        else
        {
            // Если вдруг его нет (например, при запуске из другой сцены),
            // делаем простой, резкий переход, чтобы игра не сломалась.
            Debug.LogError("SceneFader не найден! Выполняется простой переход.");
            SceneManager.LoadScene(firstSceneName);
        }
    }

    /// <summary>
    /// Этот метод нужно привязать к кнопке "Exit" или "Выход".
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Получена команда на выход из игры...");

        // Эта команда закрывает приложение.
        // ВАЖНО: Она не работает в редакторе Unity, только в скомпилированной игре (.exe).
        Application.Quit();
    }
}