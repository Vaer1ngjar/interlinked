// SwitchMinigameLogic.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SwitchMinigameLogic : MonoBehaviour
{
    public static SwitchMinigameLogic instance;

    [Header("Связи на сцене")]
    public MiniGameActivator activator;
    public SwitchButton[] switches;
    public Image[] indicatorLights;
    public Sprite lightOnSprite;
    public Sprite lightOffSprite;
    public GameObject victoryMessageObject;

    [Header("Звуки")]
    // --- НОВОЕ ПОЛЕ ДЛЯ ЗВУКА УСПЕХА ---
    [Tooltip("Звук правильного переключения тумблера")]
    public AudioClip successSound;
    // ------------------------------------
    [Tooltip("Звук, который проигрывается при нажатии на неправильный тумблер")]
    public AudioClip failureSound;

    [Header("Настройки головоломки")]
    public int[] correctSequence = { 3, 1, 6, 4, 2, 5};
    
    private int currentStep = 0;
    private AudioSource audioSource;

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        ResetGame();
    }
    
    // --- ИЗМЕНЕННЫЙ МЕТОД ---
    public void SwitchPressed(int switchID)
    {
        if (switchID == correctSequence[currentStep])
        {
            // Правильно!
            // --- НОВЫЙ БЛОК: Проигрываем звук успеха ---
            if (audioSource != null && successSound != null)
            {
                audioSource.PlayOneShot(successSound);
            }
            // -----------------------------------------

            indicatorLights[currentStep].sprite = lightOnSprite;
            FindSwitchByID(switchID).TurnOn();
            currentStep++;

            if (currentStep >= correctSequence.Length)
            {
                WinGame();
            }
        }
        else
        {
            // Ошибка!
            if (audioSource != null && failureSound != null)
            {
                audioSource.PlayOneShot(failureSound);
            }
            ResetGame();
        }
    }
    // -------------------------

    private void WinGame()
    {
        if (victoryMessageObject != null)
        {
            victoryMessageObject.SetActive(true);
        }
        if (activator != null)
        {
            activator.PrepareForFinalClose();
        }
    }

    private void ResetGame()
    {
        currentStep = 0;

        foreach (var light in indicatorLights)
        {
            light.sprite = lightOffSprite;
        }

        foreach (var sw in switches)
        {
            sw.TurnOff();
        }

        if (victoryMessageObject != null)
        {
            victoryMessageObject.SetActive(false);
        }
    }

    private SwitchButton FindSwitchByID(int id)
    {
        foreach (var sw in switches)
        {
            if (sw.switchID == id)
            {
                return sw;
            }
        }
        return null;
    }
}