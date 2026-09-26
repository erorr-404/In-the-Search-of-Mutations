// using UnityEngine;


// public enum AIState
// {
//     Wander,
//     SeekLoot,
//     Attack,
//     Flee
// }

// public class EnemyController : MonoBehaviour
// {
//     [SerializeField] public float visionRadius = 10f;
//     [SerializeField] public float minimumHealthToAttack = 5f;
//     [SerializeField] public float maximumHealthToFlee = 2.5f;
//     [SerializeField] private LayerMask scanLayers;

//     [SerializeField] private AIState currentState = AIState.Wander;
//     private SpermStats spermStats;
//     private Damageable damageable;
//     private Vector2 targetPosition;
//     private readonly Collider2D[] nearbyHits = new Collider2D[10];
//     private float nextWanderTime;

//     void Awake()
//     {
//         spermStats = GetComponent<SpermStats>();
//         damageable = GetComponent<Damageable>();
//     }

//     void Update()
//     {
//         SenseEnvironment();
//         MoveTowardsTarget();
//     }

//     void SenseEnvironment()
//     {
//         int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, visionRadius, nearbyHits, scanLayers);
//         Transform closestEnemy = null;
//         Transform closestLoot = null;
//         float minEnemyDist = float.MaxValue;
//         float minLootDist = float.MaxValue;

//         for (int i = 0; i < hitCount; i++)
//         {
//             Collider2D hit = nearbyHits[i];
//             if (hit.gameObject == gameObject) continue;

//             float dist = Vector2.Distance(transform.position, hit.transform.position);

//             if (hit.GetComponent<PickupItem>() != null && dist < minLootDist)
//             {
//                 minLootDist = dist;
//                 closestLoot = hit.transform;
//             }

//             if (hit.GetComponent<Damageable>() != null && dist < minEnemyDist)
//             {
//                 minEnemyDist = dist;
//                 closestEnemy = hit.transform;
//             }
//         }

//         if (damageable.Health <= maximumHealthToFlee && closestEnemy != null)
//         {
//             currentState = AIState.Flee;
//             targetPosition = (Vector2)transform.position + ((Vector2)transform.position - (Vector2)closestEnemy.position);
//         }
//         else if (closestLoot != null)
//         {
//             currentState = AIState.SeekLoot;
//             targetPosition = closestLoot.position;
//         }
//         else if (closestEnemy != null)
//         {
//             currentState = AIState.Attack;
//             targetPosition = closestEnemy.position;
//         }
//         else if (Time.time >= nextWanderTime)
//         {
//             currentState = AIState.Wander;
//             targetPosition = (Vector2)transform.position + Random.insideUnitCircle * visionRadius;
//             nextWanderTime = Time.time + Random.Range(2f, 4f);
//         }
//     }

//     void MoveTowardsTarget()
//     {
//         Vector2 diff = targetPosition - (Vector2)transform.position;
//         if (diff.sqrMagnitude < 0.1f) return;

//         float targetAngle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90f;
//         Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

//         SpermStatsData stats = spermStats.GetPlayerStats();
//         transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, stats.TurnSpeed * Time.deltaTime);
//         transform.position += transform.up * (stats.MaxSpeed * Time.deltaTime);
//     }

//     void OnDrawGizmos()
//     {
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(transform.position, visionRadius);
//         Gizmos.DrawLine(transform.position, (Vector3)targetPosition);
//     }
// }
