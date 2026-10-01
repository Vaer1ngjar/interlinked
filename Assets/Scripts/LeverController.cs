using UnityEngine;
using UnityEngine.Events;

public class LeverController : MonoBehaviour
{
    [Header("Ссылки и компоненты")]
    [Tooltip("Ссылка на Hinge Joint 2D, который контролирует вращение")]
    // БЫЛО: [SerializeField] private HingeJoint2D hingeJoint;
    [SerializeField] private HingeJoint2D _hingeJoint; // СТАЛО

    [Header("Настройки состояний")]
    [Tooltip("Угол, при достижении которого рычаг считается включенным")]
    [SerializeField] private float onAngle = 40f;
    [Tooltip("Угол, при достижении которого рычаг считается выключенным")]
    [SerializeField] private float offAngle = -40f;
    [Tooltip("Небольшой допуск, чтобы не требовать точного попадания в угол")]
    [SerializeField] private float angleThreshold = 5f;

    [Header("События")]
    [Tooltip("Событие, которое вызывается один раз при включении рычага")]
    public UnityEvent OnLeverActivated;
    
    [Tooltip("Событие, которое вызывается один раз при выключении рычага")]
    public UnityEvent OnLeverDeactivated;

    private bool isLeverOn = false;

    void Awake()
    {
        // БЫЛО: if (hingeJoint == null)
        if (_hingeJoint == null) // СТАЛО
        {
            Debug.LogError("Hinge Joint 2D не назначен в инспекторе!", this.gameObject);
            enabled = false;
        }
    }

    void Update()
    {
        // БЫЛО: float currentAngle = hingeJoint.jointAngle;
        float currentAngle = _hingeJoint.jointAngle; // СТАЛО

        if (!isLeverOn && currentAngle >= onAngle - angleThreshold)
        {
            ActivateLever();
        }
        else if (isLeverOn && currentAngle <= offAngle + angleThreshold)
        {
            DeactivateLever();
        }
    }

    private void ActivateLever()
    {
        isLeverOn = true;
        Debug.Log("Рычаг ВКЛЮЧЕН!");
        
        if (OnLeverActivated != null)
        {
            OnLeverActivated.Invoke();
        }
    }

    private void DeactivateLever()
    {
        isLeverOn = false;
        Debug.Log("Рычаг ВЫКЛЮЧЕН!");
        
        if (OnLeverDeactivated != null)
        {
            OnLeverDeactivated.Invoke();
        }
    }
}