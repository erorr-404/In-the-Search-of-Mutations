using UnityEngine;

[RequireComponent(typeof(SpermStats))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private SpermStatsData playerStats;
    [SerializeField] private float angleOffset = -90f;

    [Header("Ram / Dash Settings")]
    [SerializeField] private float ramImpulse = 15f;
    [SerializeField] private float ramCooldown = 1.0f;

    private Camera mainCamera;
    private SpermStats spermStats;
    private Damageable playerDamageable;
    private Rigidbody2D rb;

    private float targetAngle;
    private bool isAccelerating;
    private float lastRamTime = -999f;

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

        // 1. Зчитування цільового кута за курсором
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -mainCamera.transform.position.z;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        Vector2 difference = (Vector2)(mouseWorldPos - transform.position);

        if (difference.sqrMagnitude >= 0.01f)
        {
            targetAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg + angleOffset;
        }

        // 2. Ривок на ПКМ
        if (Input.GetMouseButtonDown(1) && Time.time >= lastRamTime + ramCooldown)
        {
            ExecuteRam();
        }
    }

    private void FixedUpdate()
    {
        // 1. Фізичний плавний поворот без тремтіння колізій
        float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, playerStats.TurnSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newAngle);

        // 2. Рух уперед через сили
        if (isAccelerating)
        {
            // Рахуємо проекцію поточної швидкості на вектор "переду"
            float forwardSpeed = Vector2.Dot(rb.velocity, transform.up);

            // Додаємо тягу, тільки якщо ще не перевищили максимальну швидкість
            if (forwardSpeed < playerStats.MaxSpeed)
            {
                rb.AddForce(transform.up * (playerStats.Acceleration * rb.mass), ForceMode2D.Force);
            }
        }
    }

    private void ExecuteRam()
    {
        lastRamTime = Time.time;
        // Миттєвий імпульс уперед, який фізично зіштовхнеться з жертвою
        rb.AddForce(transform.up * ramImpulse, ForceMode2D.Impulse);
    }
}
