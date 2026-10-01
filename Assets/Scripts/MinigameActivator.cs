// MiniGameActivator.cs
using UnityEngine;
using UnityEngine.Events;

// Требуем, чтобы на объекте был SpriteRenderer, чтобы избежать ошибок
[RequireComponent(typeof(SpriteRenderer))]
public class MiniGameActivator : MonoBehaviour
{
    public enum ActivationType
    {
        SimpleToggle,
        CompleteAfterWin
    }

    [Header("Основные настройки")]
    public ActivationType activationType = ActivationType.SimpleToggle;
    public GameObject miniGamePanel;

    // --- ВОТ ЭТОТ БЛОК ОТВЕЧАЕТ ЗА ПРОВЕРКУ СПРАЙТА ---
    [Header("Условие активации (Опционально)")]
    [Tooltip("Перетащи сюда спрайт, который ДОЛЖЕН стоять на вышке для ее активации. Если оставить пустым, будет работать всегда.")]
    public Sprite requiredSprite;
    // ----------------------------------------------------

    [Header("События")]
    public UnityEvent OnGameCompleted;
    
    private SpriteRenderer towerSpriteRenderer;
    private bool playerIsNear = false;
    private bool gameIsCompleted = false;
    private bool isAwaitingFinalClose = false;

    void Awake()
    {
        // Получаем ссылку на SpriteRenderer этого объекта
        towerSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (miniGamePanel != null)
        {
            miniGamePanel.SetActive(false);
        }
    }

    // --- В ЭТОМ МЕТОДЕ ПРОИСХОДИТ ПРОВЕРКА ---
    void Update()
    {
        if (!playerIsNear || gameIsCompleted) return;

        // Если нажата клавиша E И спрайт на вышке правильный
        if (Input.GetKeyDown(KeyCode.E) && IsSpriteCorrect())
        {
            if (activationType == ActivationType.CompleteAfterWin && isAwaitingFinalClose)
            {
                CompleteInteraction();
            }
            else
            {
                TogglePanel();
            }
        }
    }
    // -----------------------------------------

    // --- ЭТОТ МЕТОД ВЫПОЛНЯЕТ САМУ ПРОВЕРКУ ---
    private bool IsSpriteCorrect()
    {
        // Если поле "requiredSprite" не заполнено в инспекторе, то проверка не нужна, разрешаем всегда.
        if (requiredSprite == null)
        {
            return true;
        }

        // Возвращаем true, только если текущий спрайт совпадает с требуемым
        return towerSpriteRenderer.sprite == requiredSprite;
    }
    // -----------------------------------------

    private void TogglePanel()
    {
        bool isOpening = !miniGamePanel.activeSelf;
        miniGamePanel.SetActive(isOpening);
        Time.timeScale = isOpening ? 0f : 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            isAwaitingFinalClose = false;
            if (miniGamePanel != null)
            {
                miniGamePanel.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }
    
    public void ForceCloseOnFailure()
    {
        if (miniGamePanel != null)
        {
            miniGamePanel.SetActive(false);
        }
        Time.timeScale = 1f;
        isAwaitingFinalClose = false;
    }

    public void CompleteInteraction()
    {
        if (gameIsCompleted) return;
        
        gameIsCompleted = true;
        isAwaitingFinalClose = false;
        
        if (miniGamePanel != null)
        {
           miniGamePanel.SetActive(false);
           Time.timeScale = 1f;
        }

        OnGameCompleted.Invoke();
    }
    
    public void PrepareForFinalClose()
    {
        isAwaitingFinalClose = true;
    }
}