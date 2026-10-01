// PressurePlate.cs
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{

    public UnityEvent OnPlatePressed;
    public UnityEvent PlateUnpressed;

    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;

        HeavyObject heavyObject = other.GetComponent<HeavyObject>();

        if (heavyObject != null)
        {
            isActivated = true;
            Debug.Log("Кнопка нажата правильным объектом!");
            OnPlatePressed.Invoke();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        // --- ГЛАВНОЕ ИЗМЕНЕНИЕ ---
        // Прежде чем что-то делать, ПРОВЕРЯЕМ, кто именно ушел с плиты.
        HeavyObject heavyObject = other.GetComponent<HeavyObject>();

        // Если с плиты ушел правильный объект (куб) И плита была активирована
        if (heavyObject != null && isActivated)
        {
            isActivated = false;
            Debug.Log("Кнопка ОТЖАТА правильным объектом: " + other.gameObject.name);
            PlateUnpressed.Invoke();
        }
    }
}