// MatchItem.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MatchItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Имя предмета для сопоставления (у парных должно быть одинаковым)")]
    public string itemName;
    [Tooltip("Перетащи сюда ПРЕФАБ с компонентом UI Image для линии")]
    public GameObject linePrefab;
    [Tooltip("Перетащи сюда панель мини-игры, чтобы линии становились ее дочерними объектами")]
    public Transform lineParent;

    private static MatchItem draggedItem;
    private static MatchItem hoveredItem;

    private GameObject currentLineObject;
    private RectTransform lineTransform;
    private Canvas rootCanvas; // Ссылка на главный канвас

    void Start()
    {
        // Находим канвас один раз для расчетов
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Создаем линию как дочерний объект панели
        currentLineObject = Instantiate(linePrefab, lineParent);
        lineTransform = currentLineObject.GetComponent<RectTransform>();

        // Линия не должна мешать кликам
        Image lineImage = currentLineObject.GetComponent<Image>();
        if (lineImage != null)
        {
            lineImage.raycastTarget = false;
        }
        
        draggedItem = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentLineObject != null)
        {
            UpdateLine(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (hoveredItem != null && hoveredItem != draggedItem && draggedItem.itemName == hoveredItem.itemName)
        {
            // Успешное совпадение
            UpdateLine(eventData, hoveredItem.transform); // Примагничиваем к центру второго объекта
            MatchLogic.instance.AddPoint();

            draggedItem.enabled = false;
            hoveredItem.enabled = false;
        }
        else
        {
            // Неудачное совпадение
            if (currentLineObject != null)
            {
                Destroy(currentLineObject);
            }
        }
        
        draggedItem = null;
        hoveredItem = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            hoveredItem = this;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoveredItem == this)
        {
            hoveredItem = null;
        }
    }

    // Обновленный метод для рисования
    void UpdateLine(PointerEventData eventData, Transform endTarget = null)
    {
        // Получаем позиции начала и конца в координатах канваса
        Vector2 startPos = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, transform.position);
        Vector2 endPos = (endTarget != null) ? RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, endTarget.position) : eventData.position;

        // Конвертируем их в локальные координаты панели
        RectTransformUtility.ScreenPointToLocalPointInRectangle(lineParent as RectTransform, startPos, eventData.pressEventCamera, out Vector2 localStart);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(lineParent as RectTransform, endPos, eventData.pressEventCamera, out Vector2 localEnd);

        // Расчеты для поворота и размера
        Vector2 direction = localEnd - localStart;
        lineTransform.anchoredPosition = localStart + direction / 2;
        lineTransform.sizeDelta = new Vector2(direction.magnitude / rootCanvas.scaleFactor, lineTransform.sizeDelta.y);
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineTransform.localEulerAngles = new Vector3(0, 0, angle);
    }
}