// BatterySlot.cs
using UnityEngine;
using UnityEngine.Events; // Необходимо для использования UnityEvent

public class BatterySlot : MonoBehaviour
{
    [Tooltip("Точка, куда притянется батарейка. Если не указана, используется центр этого объекта.")]
    public Transform attachPoint;

    [Tooltip("Событие, которое вызывается один раз при установке батарейки.")]
    public UnityEvent OnBatteryInserted;
    
    private bool hasBattery = false; // Флаг, чтобы слот сработал только один раз

    void Start()
    {
        // Если точка не назначена, используем свой transform
        if (attachPoint == null)
        {
            attachPoint = this.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Если в слоте уже есть батарейка или объект не батарейка, выходим
        if (hasBattery || other.GetComponent<Battery>() == null)
        {
            return;
        }
        
        // Мы нашли батарейку!
        hasBattery = true;
        
        // Получаем компоненты батарейки, чтобы управлять ею
        Rigidbody2D batteryRigidbody = other.GetComponent<Rigidbody2D>();
        Collider2D batteryCollider = other.GetComponent<Collider2D>();

        // Отключаем физику и коллайдер, чтобы она больше ни с чем не взаимодействовала
        if (batteryRigidbody != null)
        {
            batteryRigidbody.simulated = false;
        }
        if (batteryCollider != null)
        {
            batteryCollider.enabled = false;
        }

        // Прикрепляем батарейку к слоту
        other.transform.SetParent(attachPoint);
        other.transform.localPosition = Vector3.zero;
        other.transform.localRotation = Quaternion.identity;

        Debug.Log("Батарейка установлена!");

        // Вызываем событие, чтобы активировать другие объекты
        if (OnBatteryInserted != null)
        {
            OnBatteryInserted.Invoke();
        }
    }
}