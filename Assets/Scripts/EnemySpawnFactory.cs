using UnityEngine;


public class EnemySpawnFactory : SpawnFactory
{
    [SerializeField] private GameObject enemyPrefab;

    public override void Spawn(Vector2 position)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyPrefab is null");
            return;
        }

        Instantiate(enemyPrefab, position, Quaternion.identity);
        Debug.Log("Spawned enemy at X=" + position.x.ToString() + " Y=" + position.y.ToString());
    }
}