// PitTriggerController.cs
using UnityEngine;

public class PitTriggerController : MonoBehaviour
{
    private PlayerController playerController;

    void Start()
    {
        // БЫЛО (Устаревший метод):
        // playerController = FindObjectOfType<PlayerController>();

        // СТАЛО (Новый, рекомендуемый метод):
        playerController = FindFirstObjectByType<PlayerController>();
        // Или более быстрый вариант, который тоже отлично подходит:
        // playerController = FindAnyObjectByType<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("Ошибка: PlayerController не найден на сцене!", this.gameObject);
        }
    }

    // Сработает один раз, когда объект входит в яму
    private void OnTriggerEnter2D(Collider2D other)
    {
        ProcessObject(other);
    }

    // Будет срабатывать каждый кадр, пока объект находится внутри ямы
    private void OnTriggerStay2D(Collider2D other)
    {
        ProcessObject(other);
    }
    
    /// <summary>
    /// Общая логика обработки объекта, попавшего в яму.
    /// </summary>
    private void ProcessObject(Collider2D objCollider)
    {
        if (playerController == null) return;

        // 1. Обработка падения ТУЛОВИЩА
        if (objCollider.attachedRigidbody == playerController.torsoRigidbody)
        {
            playerController.HandlePlayerFall();
            return;
        }

        // 2. Обработка падения сбрасываемых объектов
        ResettableObject resettable = objCollider.GetComponent<ResettableObject>();
        if (resettable != null)
        {
            // Если голова НЕ ДЕРЖИТ этот объект, запускаем эффект падения
            if (!playerController.IsCurrentlyGrabbing(objCollider.attachedRigidbody))
            {
                resettable.StartFallEffect();
            }
        }
    }
}