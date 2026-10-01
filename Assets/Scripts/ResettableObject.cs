// ResettableObject.cs
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ResettableObject : MonoBehaviour
{
    // --- Настройки эффекта падения ---
    [Tooltip("Время, за которое объект уменьшится и исчезнет (в секундах).")]
    public float fallDuration = 0.5f;

    // --- Переменные для сброса ---
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private Color initialColor;
    
    // --- Компоненты ---
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    // --- Состояние ---
    private bool isFalling = false; // Флаг, чтобы не запустить падение дважды

    void Start()
    {
        // Получаем компоненты
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Запоминаем начальное состояние
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
        if (spriteRenderer != null)
        {
            initialColor = spriteRenderer.color;
        }
    }

    /// <summary>
    /// Запускает визуальный эффект падения.
    /// </summary>
    public void StartFallEffect()
    {
        // Если эффект уже запущен, ничего не делаем
        if (isFalling) return;

        StartCoroutine(FallAndResetCoroutine());
    }

    private IEnumerator FallAndResetCoroutine()
    {
        isFalling = true;

        // Отключаем физику, чтобы объект не "протолкнулся" дальше
        rb.simulated = false;
        col.enabled = false;

        float timer = 0f;
        Vector3 startScale = transform.localScale;
        Color startColor = spriteRenderer.color;

        while (timer < fallDuration)
        {
            // Плавно уменьшаем масштаб до нуля
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, timer / fallDuration);
            
            // Плавно делаем спрайт прозрачным
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(startColor, Color.clear, timer / fallDuration);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Вызываем сброс после завершения анимации
        ResetPosition();
    }

    /// <summary>
    /// Возвращает объект в исходное состояние.
    /// </summary>
    public void ResetPosition()
    {
        // Возвращаем позицию и масштаб
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;

        // Возвращаем цвет
        if (spriteRenderer != null)
        {
            spriteRenderer.color = initialColor;
        }

        // Включаем физику обратно
        rb.simulated = true;
        col.enabled = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        isFalling = false;
    }
}