using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] int numberOfMicroImplantsToSpawn = 10;
    [SerializeField] int numberOfEnemiesToSpawn = 10;
    [SerializeField] int numberOfMutationsToSpawn = 10;

    [SerializeField] MicroImplantSpawnFactory microImplantSpawnFactory;
    [SerializeField] EnemySpawnFactory enemySpawnFactory;
    [SerializeField] MutationSpawnFactory mutationSpawnFactory;
    [SerializeField] AreaSpawner areaSpawner;
    [SerializeField] List<GameObject> enemiesList;

    public UnityEvent OnPlayerWin;
    public bool EnemiesSpawned = false;

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
        SpawnMutations();
        SpawnEnemies();
    }

    void Update()
    {
        // count all non null enemies
        int validEnemies = 0;
        foreach (GameObject enemy in enemiesList) if (enemy != null) validEnemies += 1;

        // player wins if all enemies dead
        if (EnemiesSpawned && validEnemies == 0) OnPlayerWin?.Invoke();
    }

    void SpawnEnemies()
    {
        Debug.Log("Spawning enemies.");
        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            GameObject enemy = areaSpawner.SpawnUsingFactory(enemySpawnFactory);
            if (enemy == null) continue;
            enemiesList.Add(enemy);
        }

        EnemiesSpawned = true;
    }

    void SpawnMicroImplants()
    {
        Debug.Log("Spawning MicroImplants");
        for (int i = 0; i < numberOfMicroImplantsToSpawn; i++)
        {
            areaSpawner.SpawnUsingFactory(microImplantSpawnFactory);
        }
    }

    void SpawnMutations()
    {
        Debug.Log("Spawning Mutations.");
        for (int i = 0; i < numberOfMutationsToSpawn; i++)
        {
            areaSpawner.SpawnUsingFactory(mutationSpawnFactory);
        }
    }
}