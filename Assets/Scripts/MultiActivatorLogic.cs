// MultiActivatorLogic.cs
using UnityEngine;
using UnityEngine.Events;

public class MultiActivatorLogic : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Сколько активаторов нужно, чтобы сработать")]
    public int requiredActivations = 3;

    // УДАЛИМ поле для объекта, будем использовать только событие - это гибче
    // public GameObject objectToActivate;

    [Header("События")]
    [Tooltip("Это событие сработает, когда все активаторы будут включены")]
    public UnityEvent OnAllActivated;

    private int currentActivations = 0;
    private bool hasBeenActivated = false;

    public void IncrementActivationCount()
    {
        if (hasBeenActivated) return;

        currentActivations++;
        Debug.Log("Получен сигнал для моста! Всего: " + currentActivations + " / " + requiredActivations);

        if (currentActivations >= requiredActivations)
        {
            Activate();
        }
    }

    private void Activate()
    {
        hasBeenActivated = true;
        Debug.Log("Все " + requiredActivations + " активатора для моста сработали!");

        if (OnAllActivated != null)
        {
            OnAllActivated.Invoke();
        }
    }
}