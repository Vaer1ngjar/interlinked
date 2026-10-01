using UnityEngine;

public class TowerInteraction : MonoBehaviour
{
    public GameObject miniGamePanel;
    public Sprite litTowerSprite;
    private SpriteRenderer towerSpriteRenderer;
    private bool playerIsNear = false;
    private bool gameCompleted = false;

    // НОВОЕ СОСТОЯНИЕ: 'true', когда игра выиграна и ждет нажатия 'E' для закрытия.
    private bool isAwaitingFinalClose = false;

    void Start()
    {
        towerSpriteRenderer = GetComponent<SpriteRenderer>();
        if (miniGamePanel != null)
        {
            miniGamePanel.SetActive(false);
        }
    }

    void Update()
    {
        // Вся логика нажатия 'E' теперь здесь
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            // Если мы ждем финального нажатия 'E' для закрытия...
            if (isAwaitingFinalClose)
            {
                CompleteGame();
            }
            // Иначе, если игра еще не пройдена, работаем как обычно (открыть/закрыть)
            else if (!gameCompleted)
            {
                bool isOpening = !miniGamePanel.activeSelf;
                miniGamePanel.SetActive(isOpening);

                Time.timeScale = isOpening ? 0f : 1f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("golova"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("golova"))
        {
            playerIsNear = false;
            miniGamePanel.SetActive(false);
            isAwaitingFinalClose = false; // Сбрасываем состояние, если игрок отошел
            Time.timeScale = 1f;
        }
    }

    // НОВЫЙ МЕТОД: MatchLogic будет вызывать его, когда игра выиграна
    public void PrepareForFinalClose()
    {
        isAwaitingFinalClose = true;
    }

    public void CompleteGame()
    {
        gameCompleted = true;
        isAwaitingFinalClose = false; // Сбрасываем состояние
        towerSpriteRenderer.sprite = litTowerSprite;
        miniGamePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}