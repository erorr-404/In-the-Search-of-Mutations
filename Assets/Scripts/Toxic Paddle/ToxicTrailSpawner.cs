using UnityEngine;

public class ToxicTrailSpawner : MonoBehaviour
{
    [Header("Trail Settings")]
    [SerializeField] private GameObject toxicPuddlePrefab;
    [SerializeField] private float spawnInterval = 0.15f;
    [SerializeField] private float damageMultiplier = 0.3f; 

    private SpermStats parentStats;
    private Rigidbody2D parentRb;
    
    public bool isSpawning { get; private set; }
    private float lastSpawnTime;
    private bool isPlayer;
    private PlayerController playerController;
    private EnemyController enemyController;

    private void Awake()
    {
        parentStats = GetComponentInParent<SpermStats>();
        parentRb = GetComponentInParent<Rigidbody2D>();        
        playerController = GetComponentInParent<PlayerController>();
        isPlayer = playerController != null;
        enemyController = isPlayer ? null : GetComponentInParent<EnemyController>();
    }

    private void Update()
    {
        if (parentStats == null || parentRb == null) return;

        isSpawning = isPlayer ? playerController.isSprinting : enemyController.isSprinting;
        SpermStatsData currentStats = parentStats.GetPlayerStats();

        if (isSpawning)
        {
            if (Time.time >= lastSpawnTime + spawnInterval)
            {
                SpawnPuddle(currentStats.AttackDamage);
                lastSpawnTime = Time.time;
            }
        }
    }

    private void SpawnPuddle(float parentBaseDamage)
    {
        if (toxicPuddlePrefab == null) return;

        Vector3 spawnPos = transform.position - (parentStats.transform.up * 0.5f);
        
        GameObject puddle = Instantiate(toxicPuddlePrefab, spawnPos, Quaternion.identity);
        
        if (puddle.TryGetComponent<ToxicPuddle>(out var puddleScript))
        {
            puddleScript.owner = parentStats.gameObject;
            puddleScript.isPlayerOwner = isPlayer;
            puddleScript.tickDamage = parentBaseDamage * damageMultiplier; 
        }
    }
}