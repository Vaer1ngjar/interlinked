// ActivatorLight.cs
using UnityEngine;
using UnityEngine.Rendering.Universal; // <-- ОБЯЗАТЕЛЬНО ДОБАВЬ ЭТУ СТРОКУ для работы с 2D светом

[RequireComponent(typeof(SpriteRenderer))]
public class ActivatorLight : MonoBehaviour
{
    [Header("Настройки Цвета")]
    public Color inactiveColor = Color.red;
    public Color activeColor = Color.green;

    // --- НОВОЕ ПОЛЕ ДЛЯ СВЕТА ---
    [Header("Ссылки (Опционально)")]
    [Tooltip("Перетащи сюда 2D свет, цвет которого нужно изменить. Если оставить пустым, будет меняться только цвет спрайта.")]
    public Light2D lightToControl;
    // ----------------------------

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Устанавливаем начальный цвет для спрайта
        spriteRenderer.color = inactiveColor;

        // --- НОВЫЙ БЛОК: Устанавливаем начальный цвет для света ---
        if (lightToControl != null)
        {
            lightToControl.color = inactiveColor;
        }
        // --------------------------------------------------------
    }

    /// <summary>
    /// Метод для активации света (меняет цвет на зеленый).
    /// </summary>
    public void Activate()
    {
        // Меняем цвет спрайта
        spriteRenderer.color = activeColor;

        // --- НОВЫЙ БЛОК: Меняем цвет света ---
        if (lightToControl != null)
        {
            lightToControl.color = activeColor;
        }
        // ------------------------------------
    }

    public void Deactivate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Устанавливаем начальный цвет для спрайта
        spriteRenderer.color = inactiveColor;

        // --- НОВЫЙ БЛОК: Устанавливаем начальный цвет для света ---
        if (lightToControl != null)
        {
            lightToControl.color = inactiveColor;
        }
        // --------------------------------------------------------
    }
}