// MatchLogic.cs
using UnityEngine;
using UnityEngine.UI;

public class MatchLogic : MonoBehaviour
{
    public static MatchLogic instance;

    public int maxPoints = 3;
    public Text pointsText;
    public GameObject levelCompleteUI;

    [Header("Связь с активатором")]
    public MiniGameActivator activator;

    private int currentPoints = 0;

    void Awake()
    {
        instance = this;
    }
    
    void OnEnable()
    {
        currentPoints = 0;
        UpdatePointsText();
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(false);
        }
    }

    // --- НОВЫЙ МЕТОД ---
    // Этот метод вызывается автоматически каждый раз, когда панель ВЫКЛЮЧАЕТСЯ
    void OnDisable()
    {
        CleanupLines();
    }
    // -------------------

    public void AddPoint()
    {
        currentPoints++;
        UpdatePointsText();

        if (currentPoints >= maxPoints)
        {
            if (levelCompleteUI != null)
            {
                levelCompleteUI.SetActive(true);
            }
            if (activator != null)
            {
                activator.PrepareForFinalClose();
            }
        }
    }

    void UpdatePointsText()
    {
        if (pointsText != null)
        {
            pointsText.text = currentPoints + " / " + maxPoints;
        }
    }

    // --- НОВЫЙ МЕТОД ---
    // Метод для поиска и уничтожения всех линий
    private void CleanupLines()
    {
        // Находим все игровые объекты с тэгом "MatchLine"
        GameObject[] lines = GameObject.FindGameObjectsWithTag("MatchLine");

        // Проходим по каждому найденному объекту и уничтожаем его
        foreach (GameObject line in lines)
        {
            Destroy(line);
        }
    }
    // -------------------
}