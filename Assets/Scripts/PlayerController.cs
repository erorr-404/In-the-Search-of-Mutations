using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpermStats))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private SpermStatsData playerStats;
    [SerializeField] private float angleOffset = -90f;

    public UnityEvent<float> onStaminaChange;

    private Camera mainCamera;
    private SpermStats spermStats;
    private Damageable playerDamageable;
    private Rigidbody2D rb;

    private float targetAngle;
    private bool isAccelerating;
    public bool isSprinting {get; private set ;}
    private float currentStamina;

    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        spermStats = GetComponent<SpermStats>();
        playerDamageable = GetComponent<Damageable>();

        targetAngle = transform.eulerAngles.z;

        playerStats = new SpermStatsData
        {
            MaxSpeed = 7f,
            Acceleration = 25f,
            TurnSpeed = 720f
        };
    }

    private void Start()
    {
        playerStats = spermStats.GetPlayerStats();
        spermStats.onStatsChanged.AddListener(OnStatsChanged);
        currentStamina = playerStats.MaxStamina;

        float staminaPercentFill = playerStats.MaxStamina == 0 ? 0 : currentStamina / playerStats.MaxStamina;
        onStaminaChange?.Invoke(staminaPercentFill);
    }

    private void OnStatsChanged(SpermStatsData newStats)
    {
        playerStats = newStats;
        playerDamageable.MaxHealth = newStats.Health;
    }

    private void OnDestroy()
    {
        if (spermStats != null)
        {
            spermStats.onStatsChanged.RemoveListener(OnStatsChanged);
        }
    }

    private void Update()
    {
        isAccelerating = Input.GetMouseButton(0);
        bool sprintInput = Input.GetMouseButton(1);

        // sprint can be activated only if enough stamina available
        isSprinting = sprintInput && currentStamina > 0;

        // stamina consumption
        if (sprintInput)
        {
            currentStamina -= playerStats.StaminaConsumption * Time.deltaTime;
            currentStamina = Mathf.Max(0f, currentStamina);
        }

        // stamina regeneration
        else
        {
            currentStamina += playerStats.StaminaRegenerationPerSecond * Time.deltaTime;
            currentStamina = Mathf.Min(playerStats.MaxStamina, currentStamina);
        }

        float staminaPercentFill = playerStats.MaxStamina == 0 ? 0 : currentStamina / playerStats.MaxStamina;
        onStaminaChange?.Invoke(staminaPercentFill);

        // 1. Зчитування цільового кута за курсором
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -mainCamera.transform.position.z;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        Vector2 difference = (Vector2)(mouseWorldPos - transform.position);

        if (difference.sqrMagnitude >= 0.01f)
        {
            targetAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg + angleOffset;
        }
    }

    private void FixedUpdate()
    {
        // 1. Поворот (запобігаємо діленню на 0 або блокуванню повороту)
        float turnMult = isSprinting ? Mathf.Max(0.1f, playerStats.SprintTurnMultiplier) : 1f;
        float currentTurnSpeed = playerStats.TurnSpeed * turnMult;
        
        float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, currentTurnSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

        // 2. Рух: якщо затиснутий ЛКМ АБО гравець спринтує на ПКМ
        if (isAccelerating || isSprinting)
        {
            float speedMult = isSprinting ? Mathf.Max(1f, playerStats.SprintSpeedMultiplier) : 1f;
            float accelMult = isSprinting ? Mathf.Max(1f, playerStats.SprintAccelerationMultiplier) : 1f;

            float targetSpeed = playerStats.MaxSpeed * speedMult;
            float targetAcceleration = playerStats.Acceleration * accelMult;

            float forwardSpeed = Vector2.Dot(rb.velocity, transform.up);

            if (forwardSpeed < targetSpeed)
            {
                rb.AddForce(transform.up * (targetAcceleration * rb.mass), ForceMode2D.Force);
            }
        }
    }
}
