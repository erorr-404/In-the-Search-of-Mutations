using UnityEngine;

public class AreaSpawner : MonoBehaviour
{
    [Header("Bounds & Collision")]
    [SerializeField] private Vector2 areaCenter = Vector2.zero;
    [SerializeField] private Vector2 areaSize = new Vector2(40f, 25f);
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private int maxAttempts = 15;

    public static AreaSpawner Instance;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool TryGetRandomPosition(out Vector2 spawnPosition)
    {
        float minX = areaCenter.x - areaSize.x / 2f;
        float maxX = areaCenter.x + areaSize.x / 2f;
        float minY = areaCenter.y - areaSize.y / 2f;
        float maxY = areaCenter.y + areaSize.y / 2f;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 point = new (Random.Range(minX, maxX), Random.Range(minY, maxY));

            if (Physics2D.OverlapCircle(point, checkRadius, obstacleLayers) == null)
            {
                spawnPosition = point;
                return true;
            }
        }

        spawnPosition = Vector2.zero;
        return false;
    }

    public void SpawnUsingFactory(SpawnFactory factory)
    {
        if (factory != null && TryGetRandomPosition(out Vector2 pos))
        {
            factory.Spawn(pos);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}