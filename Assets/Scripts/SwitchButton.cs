// SwitchButton.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SwitchButton : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Уникальный номер этого переключателя")]
    public int switchID;
    public Sprite switchOnSprite;
    public Sprite switchOffSprite;

    private Image buttonImage;
    private bool isInteractable = true;

    void Awake()
    {
        // Получаем ссылку на Image один раз, это более правильно
        buttonImage = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isInteractable) return;
        SwitchMinigameLogic.instance.SwitchPressed(switchID);
    }

    // Включает кнопку (делает неактивной)
    public void TurnOn()
    {
        buttonImage.sprite = switchOnSprite;
        isInteractable = false;
    }

    // Выключает кнопку (делает активной)
    public void TurnOff()
    {
        buttonImage.sprite = switchOffSprite;
        isInteractable = true;
    }
}