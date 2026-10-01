using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Цель для слежения")]
    public PlayerController playerController; 

    [Header("Настройки камеры")]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    // --- НОВЫЙ РАЗДЕЛ: Настройки приближения ---
    [Header("Настройки приближения (зум)")]
    public float zoomSpeed = 4f; // Чувствительность колесика мыши
    public float minZoomSize = 2f; // Минимальный размер (максимальное приближение)
    public float maxZoomSize = 10f; // Максимальный размер (максимальное отдаление)
    // ---------------------------------------------

    // --- НОВЫЕ ПЕРЕМЕННЫЕ ---
    private Camera mainCamera; // Ссылка на компонент камеры
    private float targetOrthographicSize; // Целевой размер, к которому мы будем плавно стремиться
    // -------------------------

    // Awake вызывается раньше Start, идеально для получения ссылок на компоненты
    void Awake()
    {
        // НОВОЕ: Получаем компонент камеры с этого же объекта
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("Ошибка: На объекте с CameraController нет компонента Camera!", this.gameObject);
        }
        else
        {
            // НОВОЕ: Изначально устанавливаем целевой размер равным текущему размеру камеры
            targetOrthographicSize = mainCamera.orthographicSize;
        }
    }

    // Update хорошо подходит для обработки ввода, так как проверяется каждый кадр
    void Update()
    {
        // НОВОЕ: Обрабатываем ввод с колесика мыши
        HandleZoom();
    }

    // LateUpdate, как и раньше, используется для движения камеры
    void LateUpdate()
    {
        // --- Логика слежения за персонажем (остается без изменений) ---
        if (playerController == null) return;
        
        Transform target = playerController.GetActiveTargetTransform();
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // --- НОВОЕ: Применяем плавное изменение размера камеры ---
        if (mainCamera != null)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetOrthographicSize, smoothSpeed * Time.deltaTime);
        }
        // --------------------------------------------------------
    }

    // --- НОВЫЙ МЕТОД: Логика обработки зума ---
    void HandleZoom()
    {
        // Получаем значение прокрутки колесика. 
        // Положительное - вверх (приблизить), отрицательное - вниз (отдалить)
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f) // Если колесико вообще крутилось
        {
            // Изменяем целевой размер. Вычитаем, так как прокрутка вверх (положительное значение)
            // должна уменьшать размер камеры для приближения.
            targetOrthographicSize -= scrollInput * zoomSpeed;

            // Ограничиваем целевой размер, чтобы не уйти в слишком сильное приближение или отдаление
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minZoomSize, maxZoomSize);
        }
    }
    // -------------------------------------------
}