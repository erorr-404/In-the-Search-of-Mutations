using System;
using UnityEngine;

public enum AIState
{
    Wander,
    SeekLoot,
    Attack,
    Flank,       // Новий стан: спроба зайти збоку
    Reposition,  // Новий стан: відступ після зіткнення
    Flee
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpermStats))]
[RequireComponent(typeof(Damageable))]
public class EnemyController : MonoBehaviour
{
    [Header("Vision & Behavior")]
    [SerializeField] public float visionRadius = 10f;
    [SerializeField] public float minimumHealthToAttack = 5f;
    [SerializeField] public float maximumHealthToFlee = 2.5f;
    [SerializeField] private LayerMask scanLayers;
    [SerializeField] private float angleOffset = -90f;

    [SerializeField] private AIState currentState = AIState.Wander;

    private SpermStats spermStats;
    private Damageable damageable;
    private Rigidbody2D rb;

    private Vector2 targetPosition;
    private float targetAngle;
    private bool hasTarget;

    private readonly Collider2D[] nearbyHits = new Collider2D[10];
    private float nextWanderTime;
    
    // Таймер для відступу
    private float repositionEndTime;

    private void Awake()
    {
        spermStats = GetComponent<SpermStats>();
        damageable = GetComponent<Damageable>();
        rb = GetComponent<Rigidbody2D>();

        targetPosition = transform.position;
        targetAngle = transform.eulerAngles.z;
    }

    private void Update()
    {
        SenseEnvironment();
        CalculateAimDirection();
    }

    private void FixedUpdate()
    {
        ApplyPhysicsMovement();
    }

    // HIT-AND-RUN: Якщо врізалися у когось, відступаємо
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Damageable>(out _))
        {
            ForceReposition(collision.transform.position);
        }
    }

    private void ForceReposition(Vector2 obstaclePos)
    {
        currentState = AIState.Reposition;
        
        // Вектор відштовхування від перешкоди
        Vector2 away = ((Vector2)transform.position - obstaclePos).normalized;
        
        // Додаємо випадковий зсув вбік, щоб не відступати по прямій
        Vector2 right = new Vector2(away.y, -away.x);
        float side = UnityEngine.Random.value > 0.5f ? 1f : -1f;
        
        // Цільова точка для відступу
        targetPosition = (Vector2)transform.position + away * 4f + right * side * 3f;
        
        // Бот ігноруватиме атаки і просто відпливатиме протягом 1-1.5 секунд
        repositionEndTime = Time.time + UnityEngine.Random.Range(1.0f, 1.5f);
    }

    private void SenseEnvironment()
    {
        // 1. Якщо ми зараз відступаємо, ігноруємо сенсори, поки не спливе таймер
        if (currentState == AIState.Reposition && Time.time < repositionEndTime)
        {
            return;
        }

        int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, visionRadius, nearbyHits, scanLayers);
        Transform closestEnemy = null;
        Transform closestLoot = null;
        float minEnemyDist = float.MaxValue;
        float minLootDist = float.MaxValue;

        // Перемішування, щоб не фокусувати лише одну ціль
        for (int i = hitCount - 1; i > 0; i--)
        {
            int swapIndex = UnityEngine.Random.Range(0, i + 1);
            Collider2D temporary = nearbyHits[i];
            nearbyHits[i] = nearbyHits[swapIndex];
            nearbyHits[swapIndex] = temporary;
        }

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = nearbyHits[i];
            if (hit.gameObject == gameObject) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);

            if (hit.GetComponent<PickupItem>() != null && dist < minLootDist)
            {
                minLootDist = dist;
                closestLoot = hit.transform;
            }

            if (hit.GetComponent<Damageable>() != null && dist < minEnemyDist)
            {
                minEnemyDist = dist;
                closestEnemy = hit.transform;
            }
        }

        // Пріоритети станів
        if (damageable.Health <= maximumHealthToFlee && closestEnemy != null)
        {
            currentState = AIState.Flee;
            targetPosition = (Vector2)transform.position + ((Vector2)transform.position - (Vector2)closestEnemy.position);
        }
        else if (closestLoot != null)
        {
            currentState = AIState.SeekLoot;
            targetPosition = closestLoot.position;
        }
        else if (closestEnemy != null)
        {
            Vector2 enemyPos = closestEnemy.position;
            Vector2 myPos = transform.position;
            Vector2 toEnemy = enemyPos - myPos;
            Vector2 enemyForward = closestEnemy.up;

            // Розраховуємо, чи дивиться ворог на нас. 
            // Якщо > 0.25, ми знаходимося в його передньому конусі зору (у зоні лобової атаки).
            float positionDot = Vector2.Dot(enemyForward, -toEnemy.normalized);

            if (positionDot > 0.25f)
            {
                // ВОРОГ ДИВИТЬСЯ НА НАС -> ОБХОДИМО ЗБОКУ (FLANK)
                currentState = AIState.Flank;
                
                // Вектор, перпендикулярний погляду ворога (його "боки")
                Vector2 right = new Vector2(enemyForward.y, -enemyForward.x);
                
                // Визначаємо, до якого боку ми ближче (лівого чи правого)
                float side = Mathf.Sign(Vector2.Dot(toEnemy, right)); 
                
                // Ставимо ціль: збоку від ворога і трохи позаду нього
                targetPosition = enemyPos + (right * side * 4f) - (enemyForward * 2.5f);
            }
            else
            {
                // МИ ЗБОКУ АБО ПОЗАДУ -> АТАКУЄМО НАПРЯМУ (DIVE BOMB)
                currentState = AIState.Attack;
                targetPosition = enemyPos;
            }
        }
        else if (Time.time >= nextWanderTime)
        {
            currentState = AIState.Wander;
            targetPosition = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * visionRadius;
            nextWanderTime = Time.time + UnityEngine.Random.Range(2f, 4f);
        }
    }

    private void CalculateAimDirection()
    {
        Vector2 diff = targetPosition - (Vector2)transform.position;

        if (diff.sqrMagnitude < 0.25f)
        {
            hasTarget = false;
            return;
        }

        hasTarget = true;
        targetAngle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg + angleOffset;
    }

    private void ApplyPhysicsMovement()
    {
        SpermStatsData stats = spermStats.GetPlayerStats();

        float targetMaxSpeed = stats.MaxSpeed;
        float targetAccel = stats.Acceleration;
        float targetTurn = stats.TurnSpeed;

        // Використовуємо твої множники СПРИНТУ, коли бот іде на вбивство або тікає!
        if (currentState == AIState.Attack || currentState == AIState.Flee)
        {
            targetMaxSpeed *= Mathf.Max(1f, stats.SprintSpeedMultiplier);
            targetAccel *= Mathf.Max(1f, stats.SprintAccelerationMultiplier);
            targetTurn *= Mathf.Max(1f, stats.SprintTurnMultiplier);
        }

        // 1. Плавний поворот
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, targetTurn * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

        // 2. Рух
        if (hasTarget)
        {
            float forwardSpeed = Vector2.Dot(rb.velocity, transform.up);
            if (forwardSpeed < targetMaxSpeed)
            {
                rb.AddForce(transform.up * (targetAccel * rb.mass), ForceMode2D.Force);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}