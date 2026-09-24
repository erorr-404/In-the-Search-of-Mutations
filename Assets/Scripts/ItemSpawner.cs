using UnityEngine;


// TODO: next step is to make random spawn of micro-implants
public class ItemSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private int initialCount = 30;

    [Header("Spawn Bounds (Size & Position)")]
    [SerializeField] private Vector2 areaCenter = Vector2.zero;
    [SerializeField] private Vector2 areaSize = new Vector2(40f, 25f);

    [Header("Collision Check")]
    [SerializeField] private float itemRadius = 0.4f;
    [SerializeField] private LayerMask obstacleLayers; // Шар стін / перешкод
    [SerializeField] private int maxAttempts = 15;      // Кількість спроб знайти вільне місце

    private void Start()
    {
        SpawnAll();
    }

    public void SpawnAll()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0) return;

        for (int i = 0; i < initialCount; i++)
        {
            SpawnSingleItem();
        }
    }

    public void SpawnSingleItem()
    {
        if (TryGetRandomFreePosition(out Vector2 spawnPosition))
        {
            GameObject randomPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            Instantiate(randomPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private bool TryGetRandomFreePosition(out Vector2 result)
    {
        float minX = areaCenter.x - areaSize.x / 2f;
        float maxX = areaCenter.x + areaSize.x / 2f;
        float minY = areaCenter.y - areaSize.y / 2f;
        float maxY = areaCenter.y + areaSize.y / 2f;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPoint = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            // Перевіряємо, чи немає в цій точці стіни або перешкоди
            Collider2D overlap = Physics2D.OverlapCircle(randomPoint, itemRadius, obstacleLayers);
            if (overlap == null)
            {
                result = randomPoint;
                return true;
            }
        }

        result = Vector2.zero;
        return false;
    }

    // Малює зелену рамку зони спавну прямо у вікні Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}