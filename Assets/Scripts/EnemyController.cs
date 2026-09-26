using UnityEngine;

public enum AIState
{
    Wander,
    SeekLoot,
    Attack,
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

    private void SenseEnvironment()
    {
        int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, visionRadius, nearbyHits, scanLayers);
        Transform closestEnemy = null;
        Transform closestLoot = null;
        float minEnemyDist = float.MaxValue;
        float minLootDist = float.MaxValue;

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
            currentState = AIState.Attack;
            targetPosition = closestEnemy.position;
        }
        else if (Time.time >= nextWanderTime)
        {
            currentState = AIState.Wander;
            targetPosition = (Vector2)transform.position + Random.insideUnitCircle * visionRadius;
            nextWanderTime = Time.time + Random.Range(2f, 4f);
        }
    }

    private void CalculateAimDirection()
    {
        Vector2 diff = targetPosition - (Vector2)transform.position;

        // Якщо точка надто близько, перестаємо тиснути на тягу
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

        // 1. Плавний поворот (працює стабільно навіть при Freeze Rotation Z)
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, stats.TurnSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

        // 2. Фізичне додавання тяги вперед
        if (hasTarget)
        {
            float forwardSpeed = Vector2.Dot(rb.velocity, transform.up);

            if (forwardSpeed < stats.MaxSpeed)
            {
                rb.AddForce(transform.up * (stats.Acceleration * rb.mass), ForceMode2D.Force);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.DrawLine(transform.position, (Vector3)targetPosition);
    }
}
