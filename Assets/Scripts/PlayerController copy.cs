// using UnityEngine;

// [RequireComponent(typeof(SpermStats))]
// [RequireComponent(typeof(Damageable))]
// [RequireComponent(typeof(Rigidbody2D))]
// public class PlayerController : MonoBehaviour
// {
//     [SerializeField] SpermStatsData playerStats;
//     [SerializeField] float angleOffset = -90f;

//     [Header("Ram / Dash Settings")]
//     [SerializeField] float ramSpeed = 16f;        // Початкова швидкість ривка вперед
//     [SerializeField] float ramDrag = 35f;         // Як швидко гальмує ривок (опір рідини)
//     [SerializeField] float ramCooldown = 1.2f;    // Кулдаун між ударами
//     private float lastRamTime = -999f;
//     private Vector2 ramVelocity;

//     Camera mainCamera;
//     SpermStats spermStats;
//     Damageable playerDamageable;
//     Rigidbody2D rb;

//     Quaternion targetRotation;
//     float currentSpeed;

//     void Awake()
//     {
//         mainCamera = Camera.main;
//         targetRotation = transform.rotation;

//         playerStats = new SpermStatsData
//         {
//             MaxSpeed = 5f,
//             Acceleration = 8f,
//             Deceleration = 10f,
//             TurnSpeed = 360f
//         };
//     }

//     void Start()
//     {
//         spermStats = GetComponent<SpermStats>();
//         playerDamageable = GetComponent<Damageable>();
//         playerStats = spermStats.GetPlayerStats();
//         rb = GetComponent<Rigidbody2D>();

//         spermStats.onStatsChanged.AddListener(OnStatsChanged);
//     }

//     void OnStatsChanged(SpermStatsData newStats)
//     {
//         playerStats = newStats;
//         gameObject.transform.localScale = new Vector3(newStats.Size, newStats.Size, newStats.Size);
//         playerDamageable.MaxHealth = newStats.Health;

//         Debug.Log("Player stats changed.");
//     }

//     void OnDestroy()
//     {
//         if (spermStats != null)
//         {
//             spermStats.onStatsChanged.RemoveListener(OnStatsChanged);
//         }
//     }

//     void Update()
//     {
//         bool leftMouseButtonPressed = Input.GetMouseButton(0);
//         bool rightMouseButtonPress = Input.GetMouseButtonDown(1);

//         // 1. Активація удару / ривка по ПКМ
//         if (rightMouseButtonPress && Time.time >= lastRamTime + ramCooldown)
//         {
//             ExecuteRam();
//         }

//         // 2. Розрахунок кута повороту за мишкою
//         if (leftMouseButtonPressed)
//         {
//             Vector3 mouseScreenPos = Input.mousePosition;
//             mouseScreenPos.z = -mainCamera.transform.position.z;
//             Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
//             Vector2 difference = (Vector2)(mouseWorldPos - transform.position);

//             if (difference.sqrMagnitude >= 0.01f)
//             {
//                 float targetAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg + angleOffset;
//                 targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
//             }
//         }

//         // Плавний поворот (під час сильного ривка поворот можна сповільнювати для відчуття маси)
//         float currentTurnSpeed = (ramVelocity.sqrMagnitude > 1f) ? playerStats.TurnSpeed * 0.3f : playerStats.TurnSpeed;
//         transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, currentTurnSpeed * Time.deltaTime);

//         // 3. Розрахунок базової швидкості
//         float targetSpeed = leftMouseButtonPressed ? playerStats.MaxSpeed : 0f;
//         float rate = leftMouseButtonPressed ? playerStats.Acceleration : playerStats.Deceleration;
//         currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.deltaTime);

//         // 4. Сумарне переміщення: звичайний рух + імпульс ривка
//         Vector2 totalVelocity = ((Vector2)transform.up * currentSpeed) + ramVelocity;
//         if (totalVelocity.sqrMagnitude > 0.001f)
//         {
//             transform.position += (Vector3)(totalVelocity * Time.deltaTime);
//         }

//         // 5. Плавне згасання імпульсу ривка
//         ramVelocity = Vector2.MoveTowards(ramVelocity, Vector2.zero, ramDrag * Time.deltaTime);
//     }

//     public void UpdatePlayerStats(SpermStatsData newStats)
//     {
//         playerStats = newStats;
//     }

//     private void ExecuteRam()
//     {
//         lastRamTime = Time.time;
//         // Задаємо різкий поштовх точно у напрямку, куди зараз дивиться голова
//         ramVelocity = (Vector2)transform.up * ramSpeed;
//     }
// }
