using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] int numberOfMicroImplantsToSpawn = 10;
    [SerializeField] int numberOfEnemiesToSpawn = 10;

    [SerializeField] MicroImplantSpawnFactory microImplantSpawnFactory;
    [SerializeField] EnemySpawnFactory enemySpawnFactory;
    [SerializeField] AreaSpawner areaSpawner;

    void Awake()
    {
        // singleton implementation
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SpawnMicroImplants();
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        Debug.Log("Spawning enemies.");
        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            areaSpawner.SpawnUsingFactory(enemySpawnFactory);
        }
    }

    void SpawnMicroImplants()
    {
        Debug.Log("Spawning MicroImplants");
        for (int i = 0; i < numberOfMicroImplantsToSpawn; i++)
        {
            areaSpawner.SpawnUsingFactory(microImplantSpawnFactory);
        }
    }
}