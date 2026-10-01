// EndGameConsole.cs
using UnityEngine;

public class EndGameConsole : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Точное название финальной сцены (например, 'Credits' или 'EndScene')")]
    public string endSceneName;
    
    [Tooltip("Объект, который будет светиться, когда консоль можно использовать")]
    public GameObject indicatorLight;

    private bool playerIsNear = false;
    private bool isActivated = false; // Флаг, чтобы не сработать дважды

    void Start()
    {
        // Выключаем индикатор в начале
        if (indicatorLight != null)
        {
            indicatorLight.SetActive(false);
        }
    }

    void Update()
    {
        // Если игрок рядом, консоль еще не активирована и он нажал 'E'
        if (playerIsNear && !isActivated && Input.GetKeyDown(KeyCode.E))
        {
            ActivateConsole();
        }
    }

    private void ActivateConsole()
    {
        isActivated = true;
        Debug.Log("Финальная консоль активирована! Запускаем переход на сцену: " + endSceneName);

        // Проверяем, что наш SceneFader существует
        if (SceneFader.Instance != null)
        {
            // Вызываем его метод для перехода на финальную сцену
            SceneFader.Instance.FadeToScene(endSceneName);
        }
        else
        {
            // Если вдруг SceneFader'а нет, делаем простой переход
            Debug.LogError("SceneFader не найден! Выполняется простой переход без затемнения.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(endSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что в триггер вошла именно голова
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            // Включаем индикатор, чтобы игрок понял, что можно взаимодействовать
            if (indicatorLight != null)
            {
                indicatorLight.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            // Выключаем индикатор, когда игрок отходит
            if (indicatorLight != null)
            {
                indicatorLight.SetActive(false);
            }
        }
    }
}