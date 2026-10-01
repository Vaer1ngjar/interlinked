// GameController.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class GameController : MonoBehaviour
{
    [Header("Настройки игры")]
    public float maxTime = 10f;
    public int minSequenceLength = 4;
    public int maxSequenceLength = 7;
    public int sequencesToWin = 3;

    [Header("Настройки штрафа")]
    public float timePenalty = 2.0f;
    public float failureDisplayDuration = 0.5f;

    [Header("Звуки и UI")]
    public AudioClip failureSound;
    public Text progressText;

    [Header("Спрайты стрелок")]
    public Sprite upArrow;
    public Sprite downArrow;
    public Sprite leftArrow;
    public Sprite rightArrow;

    [Header("UI Элементы (перетащить со сцены)")]
    public GameObject arrowPrefab;
    public Transform sequenceContainer;
    public Slider timerSlider;

    [Header("Связь с активатором")]
    public MiniGameActivator activator;

    private enum Direction { Up, Down, Left, Right }
    private List<Direction> currentSequence = new List<Direction>();
    private List<Image> spawnedArrowImages = new List<Image>();
    private int currentIndex = 0;
    private float currentTime;
    private bool isShowingFailure = false;
    private int sequencesCompleted = 0;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        sequencesCompleted = 0;
        UpdateProgressText();
        GenerateNewSequence();
    }

    void Update()
    {
        if (isShowingFailure) return;

        currentTime -= Time.unscaledDeltaTime;
        timerSlider.value = currentTime / maxTime;

        // --- ИЗМЕНЕНИЕ ЛОГИКИ: Время вышло ---
        if (currentTime <= 0)
        {
            // Время вышло - это полный проигрыш. Закрываем игру.
            HandleGameOver();
        }
        // -------------------------------------
        
        ProcessInput();
    }
    
    // ... (GenerateNewSequence без изменений) ...
    #region Unchanged Methods
    void GenerateNewSequence()
    {
        isShowingFailure = false;
        currentTime = maxTime;
        currentIndex = 0;
        currentSequence.Clear();

        foreach (Transform child in sequenceContainer)
        {
            Destroy(child.gameObject);
        }
        spawnedArrowImages.Clear();
        
        int currentSequenceLength = Random.Range(minSequenceLength, maxSequenceLength + 1);

        for (int i = 0; i < currentSequenceLength; i++)
        {
            Direction randomDir = (Direction)Random.Range(0, 4);
            currentSequence.Add(randomDir);

            GameObject arrowObject = Instantiate(arrowPrefab, sequenceContainer);
            Image arrowImage = arrowObject.GetComponent<Image>();
            arrowImage.sprite = GetSpriteForDirection(randomDir);
            spawnedArrowImages.Add(arrowImage);
        }
    }
    #endregion
    
    void ProcessInput()
    {
        if (isShowingFailure) return;
        // ... (остальной код метода без изменений)
        #region Unchanged ProcessInput
        Direction? input = null;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) input = Direction.Up;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) input = Direction.Down;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) input = Direction.Left;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) input = Direction.Right;
        
        if (input.HasValue)
        {
            if (input.Value == currentSequence[currentIndex])
            {
                spawnedArrowImages[currentIndex].color = Color.green;
                currentIndex++;

                if (currentIndex >= currentSequence.Count)
                {
                    OnSequenceComplete();
                }
            }
            else
            {
                // Неправильный ввод - это ошибка, но не конец игры.
                OnInputFailure();
            }
        }
        #endregion
    }

    void OnSequenceComplete()
    {
        sequencesCompleted++;
        UpdateProgressText();

        if (sequencesCompleted >= sequencesToWin)
        {
            Debug.Log("Все последовательности пройдены!");
            if(activator != null)
            {
                activator.CompleteInteraction();
            }
        }
        else
        {
            Debug.Log("Раунд " + sequencesCompleted + " пройден!");
            GenerateNewSequence();
        }
    }
    
    // --- НОВЫЙ МЕТОД: Ошибка ввода ---
    void OnInputFailure()
    {
        if (audioSource != null && failureSound != null)
        {
            audioSource.PlayOneShot(failureSound);
        }

        if (!isShowingFailure)
        {
            StartCoroutine(InputFailureRoutine());
        }
    }
    // ---------------------------------
    
    // --- НОВЫЙ МЕТОД: Конец игры по времени ---
    void HandleGameOver()
    {
        // Блокируем дальнейшие действия, чтобы не было ошибок
        if (isShowingFailure) return;
        isShowingFailure = true;

        Debug.Log("Время вышло! Проигрыш.");

        if (audioSource != null && failureSound != null)
        {
            audioSource.PlayOneShot(failureSound);
        }

        // Вызываем новый безопасный метод закрытия
        if (activator != null)
        {
            activator.ForceCloseOnFailure();
        }
    }
    // -----------------------------------------

    // --- Корутина теперь ТОЛЬКО для ошибки ввода ---
    private IEnumerator InputFailureRoutine()
    {
        isShowingFailure = true;
        currentTime -= timePenalty; // Отнимаем время

        // Не сбрасываем прогресс раундов при ошибке ввода
        // sequencesCompleted = 0; 
        
        foreach (var img in spawnedArrowImages)
        {
            img.color = Color.red;
        }

        yield return new WaitForSecondsRealtime(failureDisplayDuration);

        foreach (var img in spawnedArrowImages)
        {
            img.color = Color.white;
        }

        // Сбрасываем только ввод текущей последовательности
        currentIndex = 0;
        isShowingFailure = false;
    }
    
    // ... (UpdateProgressText и GetSpriteForDirection без изменений)
    #region Unchanged Helper Methods
    private void UpdateProgressText()
    {
        if (progressText != null)
        {
            progressText.text = sequencesCompleted + " / " + sequencesToWin;
        }
    }

    Sprite GetSpriteForDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return upArrow;
            case Direction.Down: return downArrow;
            case Direction.Left: return leftArrow;
            case Direction.Right: return rightArrow;
        }
        return null;
    }
    #endregion
}