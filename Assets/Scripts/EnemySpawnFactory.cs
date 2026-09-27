using UnityEngine;


public class EnemySpawnFactory : SpawnFactory
{
    [SerializeField] private GameObject enemyPrefab;
    
    private UIController uiController;

    void Start()
    {
        uiController = UIController.Instance;
    }

    public override GameObject Spawn(Vector2 position)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyPrefab is null");
            return null;
        }

        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        Damageable damageable = enemy.GetComponent<Damageable>();
        damageable.onDeath.AddListener(uiController.OnKill);

        Debug.Log("Spawned enemy at X=" + position.x.ToString() + " Y=" + position.y.ToString());
        return enemy;
    }
}