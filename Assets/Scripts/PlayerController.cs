using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("ОБЯЗАТЕЛЬНЫЕ ССЫЛКИ")]
    public Rigidbody2D headRigidbody;
    public Rigidbody2D torsoRigidbody;
    public DistanceJoint2D torsoJoint;
    public SpriteRenderer headSpriteRenderer;
    public SpriteRenderer torsoSpriteRenderer;
    
    [Header("Анимация")]
    public Animator headAnimator;
    // --- НОВОЕ ПОЛЕ ---
    [Tooltip("Перетащи сюда компонент Animator с объекта туловища")]
    public Animator torsoAnimator;
    // -------------------
    
    [Header("Визуализация связи")]
    public LineRenderer connectionLine;
    public Color normalColor = Color.cyan;
    public Color warningColor = Color.yellow;
    public Color maxDistanceColor = Color.red;
    [Range(0,1)]
    public float warningThreshold = 0.7f;
    
    [Header("Настройки Движения")]
    public float moveSpeed = 5f;
    public float followSpeed = 4f;
    public float maxDistance = 5f;

    [Header("Настройки Захвата (для Головы)")]
    public KeyCode grabKey = KeyCode.E;
    public float grabRadius = 1.5f;
    public string grabbableTag = "Grabbable";
    public float heavyHoldDuration = 0.5f;

    [Header("Настройки Плавного Захвата")]
    public float grabMoveDuration = 0.25f;
    public Vector2 grabOffset = new Vector2(0, -1.2f);
    private bool isGrabbing = false;

    [Header("Настройки Респауна")]
    public Transform respawnPoint;
    public float playerFallDuration = 0.7f;

    private Rigidbody2D grabbedObject = null;
    private FixedJoint2D grabJoint;
    private bool isControllingHead = true;
    private bool isTorsoLocked = false;
    private bool isPlayerFalling = false;
    private Coroutine heavyHoldCoroutine;
    private Rigidbody2D activeRigidbody;
    private Transform activeTransform;
    private Rigidbody2D followerRigidbody;
    private Transform followerTransform;

    void Start()
    {
        if (headRigidbody == null || torsoRigidbody == null || torsoJoint == null || headSpriteRenderer == null || torsoSpriteRenderer == null)
        {
            Debug.LogError("КРИТИЧЕСКАЯ ОШИБКА: Не все ссылки назначены в инспекторе!", this.gameObject);
            return;
        }
        if (respawnPoint == null)
        {
            Debug.LogWarning("Внимание: Точка респауна (Respawn Point) не назначена в PlayerController.", this.gameObject);
        }
        torsoJoint.connectedBody = headRigidbody;
        torsoJoint.distance = maxDistance;
        SwitchControlTarget();
        
        if (connectionLine != null)
        {
            connectionLine.positionCount = 2;
            connectionLine.startColor = normalColor;
            connectionLine.endColor = normalColor;
        }
    }

    void LateUpdate()
    {
        if (connectionLine != null)
        {
            UpdateConnectionLine();
        }
    }
    
    void UpdateConnectionLine()
    {
        connectionLine.SetPosition(0, headRigidbody.position);
        connectionLine.SetPosition(1, torsoRigidbody.position);
        
        float currentDistance = Vector2.Distance(headRigidbody.position, torsoRigidbody.position);
        float distanceFraction = currentDistance / maxDistance;
        Color currentColor;
        if (distanceFraction < warningThreshold)
        {
            float lerpFactor = distanceFraction / warningThreshold;
            currentColor = Color.Lerp(normalColor, warningColor, lerpFactor);
        }
        else
        {
            float lerpFactor = (distanceFraction - warningThreshold) / (1 - warningThreshold);
            currentColor = Color.Lerp(warningColor, maxDistanceColor, lerpFactor);
        }
        connectionLine.startColor = currentColor;
        connectionLine.endColor = currentColor;
    }

    void Update()
    {
        if (isPlayerFalling) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!isTorsoLocked) 
            {
                isControllingHead = !isControllingHead;
                SwitchControlTarget();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isTorsoLocked = !isTorsoLocked;
            if (isTorsoLocked)
            {
                torsoRigidbody.bodyType = RigidbodyType2D.Static;
                isControllingHead = true; 
                SwitchControlTarget();
            }
            else
            {
                torsoRigidbody.bodyType = RigidbodyType2D.Dynamic;
            }
        }
        
        if (Input.GetKeyDown(grabKey))
        {
            if (isControllingHead)
            {
                ToggleGrab();
            }
        }
    }
    
    void SwitchControlTarget()
    {
        if (isControllingHead)
        {
            activeRigidbody = headRigidbody;
            activeTransform = headRigidbody.transform;
            followerRigidbody = torsoRigidbody;
            followerTransform = torsoRigidbody.transform;
        }
        else
        {
            activeRigidbody = torsoRigidbody;
            activeTransform = torsoRigidbody.transform;
            followerRigidbody = headRigidbody;
            followerTransform = headRigidbody.transform;
        }
    }

    void FixedUpdate()
    {
        if (isPlayerFalling) return;

        if (activeRigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            Vector2 movement = new Vector2(moveX, moveY).normalized * moveSpeed;
            activeRigidbody.velocity = movement;
        }
        
        if (followerRigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            float distance = Vector2.Distance(followerRigidbody.position, activeRigidbody.position);
            if (distance > 1.0f)
            {
                Vector2 direction = (activeRigidbody.position - followerRigidbody.position).normalized;
                followerRigidbody.velocity = direction * followSpeed;
            }
            else
            {
                followerRigidbody.velocity = Vector2.zero;
            }
        }
        
        UpdateSpriteDirection(headRigidbody, headSpriteRenderer);
        UpdateSpriteDirection(torsoRigidbody, torsoSpriteRenderer);
        
        if (headAnimator != null)
        {
            bool isMoving = headRigidbody.velocity.magnitude > 0.1f;
            headAnimator.SetBool("isMoving", isMoving);
        }

        // --- НОВЫЙ БЛОК ДЛЯ АНИМАЦИИ ТУЛОВИЩА ---
        if (torsoAnimator != null)
        {
            bool isMoving = torsoRigidbody.velocity.magnitude > 0.1f;
            torsoAnimator.SetBool("isMoving", isMoving);
        }
        // ----------------------------------------
    }
    
    void UpdateSpriteDirection(Rigidbody2D rb, SpriteRenderer sr)
    {
        float moveX = rb.velocity.x;
        if (Mathf.Abs(moveX) > 0.1f)
        {
            sr.flipX = moveX < 0;
        }
    }
    
    void ToggleGrab()
    {
        if (isGrabbing) return;

        if (grabbedObject != null)
        {
            ForceDropObject();
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(headRigidbody.position, grabRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag(grabbableTag) && hit.GetComponent<Rigidbody2D>() != null)
            {
                bool isHeavy = hit.GetComponent<HeavyObject>() != null;
                StartCoroutine(GrabAndMove(hit.GetComponent<Rigidbody2D>(), isHeavy));
                break; 
            }
        }
    }

    IEnumerator GrabAndMove(Rigidbody2D objectToGrab, bool isHeavy)
    {
        isGrabbing = true; 
        grabbedObject = objectToGrab;

        float elapsedTime = 0f;
        Vector2 startPosition = objectToGrab.position;
        float originalGravityScale = objectToGrab.gravityScale;
        objectToGrab.gravityScale = 0;
        objectToGrab.velocity = Vector2.zero;

        while (elapsedTime < grabMoveDuration)
        {
            Vector2 targetPosition = headRigidbody.position + grabOffset;
            objectToGrab.MovePosition(Vector2.Lerp(startPosition, targetPosition, elapsedTime / grabMoveDuration));
            elapsedTime += Time.fixedDeltaTime; 
            yield return new WaitForFixedUpdate();
        }

        objectToGrab.position = headRigidbody.position + grabOffset;
        objectToGrab.gravityScale = originalGravityScale;

        grabJoint = headRigidbody.gameObject.AddComponent<FixedJoint2D>();
        grabJoint.connectedBody = grabbedObject;
        grabJoint.enableCollision = false;
        grabJoint.anchor = grabOffset; 
        grabJoint.connectedAnchor = Vector2.zero;

        if (headAnimator != null)
        {
            headAnimator.SetBool("isHolding", true);
        }
        
        isGrabbing = false;
        
        if (isHeavy)
        {
            heavyHoldCoroutine = StartCoroutine(HeavyObjectHoldTimer());
        }
    }
    
    private IEnumerator HeavyObjectHoldTimer()
    {
        yield return new WaitForSeconds(heavyHoldDuration);
        ForceDropObject();
        heavyHoldCoroutine = null;
    }
    
    private void ForceDropObject()
    {
        if (heavyHoldCoroutine != null)
        {
            StopCoroutine(heavyHoldCoroutine);
            heavyHoldCoroutine = null;
        }

        if (grabbedObject != null)
        {
            if (grabJoint != null)
            {
                Destroy(grabJoint);
            }
            grabbedObject = null;
            
            if (headAnimator != null)
            {
                headAnimator.SetBool("isHolding", false);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (headRigidbody != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(headRigidbody.position, grabRadius);
            Gizmos.color = Color.red;
            Vector2 targetPos = (Vector2)headRigidbody.transform.position + grabOffset;
            Gizmos.DrawSphere(targetPos, 0.1f);
        }
    }
    
    public Transform GetActiveTargetTransform()
    {
        return activeTransform;
    }

    public void HandlePlayerFall()
    {
        if (respawnPoint == null || isPlayerFalling) return;
        
        StartCoroutine(FallAndRespawnCoroutine());
    }
    
    private IEnumerator FallAndRespawnCoroutine()
    {
        isPlayerFalling = true;
        
        ForceDropObject();

        headRigidbody.simulated = false;
        torsoRigidbody.simulated = false;

        float timer = 0f;
        Vector3 headStartScale = headSpriteRenderer.transform.localScale;
        Vector3 torsoStartScale = torsoSpriteRenderer.transform.localScale;
        Color headStartColor = headSpriteRenderer.color;
        Color torsoStartColor = torsoSpriteRenderer.color;
        
        while (timer < playerFallDuration)
        {
            float progress = timer / playerFallDuration;
            
            headSpriteRenderer.transform.localScale = Vector3.Lerp(headStartScale, Vector3.zero, progress);
            headSpriteRenderer.color = Color.Lerp(headStartColor, Color.clear, progress);
            
            torsoSpriteRenderer.transform.localScale = Vector3.Lerp(torsoStartScale, Vector3.zero, progress);
            torsoSpriteRenderer.color = Color.Lerp(torsoStartColor, Color.clear, progress);
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        torsoRigidbody.transform.position = respawnPoint.position;
        headRigidbody.transform.position = respawnPoint.position;
        
        headSpriteRenderer.transform.localScale = headStartScale;
        torsoSpriteRenderer.transform.localScale = torsoStartScale;
        headSpriteRenderer.color = headStartColor;
        torsoSpriteRenderer.color = torsoStartColor;
        
        headRigidbody.simulated = true;
        torsoRigidbody.simulated = true;
        
        isPlayerFalling = false;
    }

    public bool IsCurrentlyGrabbing(Rigidbody2D objectToCheck)
    {
        return grabbedObject != null && grabbedObject == objectToCheck;
    }
}