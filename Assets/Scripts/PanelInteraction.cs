using UnityEngine;

public class PanelInteraction : MonoBehaviour
{
    [Tooltip("Панель с UI мини-игры, которую нужно показать")]
    public GameObject miniGamePanel;

    [Tooltip("Спрайт, на который заменится текущий после прохождения игры (например, вышка со светом)")]
    public Sprite litPanelSprite; // В вашем случае, может быть спрайт вышки

    private SpriteRenderer objectSpriteRenderer; // Переименовали для ясности
    private bool playerIsNear = false;
    private bool gameCompleted = false;
    private bool isAwaitingFinalClose = false;

    void Start()
    {
        // Находим SpriteRenderer на этом же объекте
        objectSpriteRenderer = GetComponent<SpriteRenderer>();
        if (miniGamePanel != null)
        {
            miniGamePanel.SetActive(false);
        }
    }

    void Update()
    {
        // Проверяем, находится ли игрок рядом и нажал ли он 'E'
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            // Если игра уже пройдена и ждет закрытия
            if (isAwaitingFinalClose)
            {
                CompleteGame();
            }
            // Если игра еще не пройдена
            else if (!gameCompleted)
            {
                // Узнаем, открываем мы панель или закрываем
                bool isOpening = !miniGamePanel.activeSelf;

                // Включаем/выключаем панель
                miniGamePanel.SetActive(isOpening);

                // Если открываем игру - ставим паузу (время = 0). Если закрываем - снимаем паузу (время = 1).
                Time.timeScale = isOpening ? 0f : 1f;
            }
        }
    }

    // Этот метод будет вызван скриптом SwitchMinigameLogic, когда все тумблеры нажаты правильно
    public void PrepareForFinalClose()
    {
        isAwaitingFinalClose = true;
    }

    // Этот метод завершает мини-игру
    public void CompleteGame()
    {
        gameCompleted = true;
        isAwaitingFinalClose = false;

        // Меняем спрайт, если он назначен
        if (objectSpriteRenderer != null && litPanelSprite != null)
        {
            objectSpriteRenderer.sprite = litPanelSprite;
        }

        // Закрываем панель
        miniGamePanel.SetActive(false);

        // Обязательно возвращаем нормальное течение времени
        Time.timeScale = 1f;
    }

    // Когда игрок входит в зону действия
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Убедитесь, что у вашего игрока (или его части) есть тег "golova"
        if (other.CompareTag("golova"))
        {
            playerIsNear = true;
        }
    }

    // Когда игрок выходит из зоны действия
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("golova"))
        {
            playerIsNear = false;

            // Если игрок ушел, закрываем панель и всегда возвращаем время
            miniGamePanel.SetActive(false);
            isAwaitingFinalClose = false;
            Time.timeScale = 1f;
        }
    }
}