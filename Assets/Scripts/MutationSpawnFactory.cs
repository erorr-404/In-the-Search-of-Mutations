using System.Collections.Generic;
using UnityEngine;

// TODO: Create separate class for weighted random choose

public class MutationSpawnFactory : SpawnFactory
{
    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private List<MutationData> mutationPool;

    /// <summary>
    /// Spawns random Mutation at given position.
    /// </summary>
    /// <param name="position">Vector2 where to spawn.</param>
    public override GameObject Spawn(Vector2 position)
    {
        if (pickupPrefab == null) 
        {
            Debug.LogError("PickupPrefab is null"); 
            return null;
        }

        MutationData selected = GetWeightedMutationData();
        if (selected == null)
        {
            Debug.LogError("Can not select mutation to spawn.");
            return null;
        }

        GameObject gm = Instantiate(pickupPrefab, position, Quaternion.identity);
        gm.GetComponent<PickupItem>().SetMutation(selected);

        Debug.Log("Spawning mutation " + selected.Name + " at X=" + position.x.ToString() + " Y=" + position.y.ToString());
        return gm;
    }

    private MutationData GetWeightedMutationData()
    {
        if (mutationPool.Count == 0) return null;
        if (mutationPool.Count == 1) return mutationPool[1];

        float totalWeight = 0f;
        foreach (MutationData mutation in mutationPool)
        {
            if (mutation != null) totalWeight += mutation.SpawnProbabilityWeight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float currentSum = 0f;

        foreach (MutationData mutation in mutationPool)
        {
            if (mutation == null) continue;
            currentSum += mutation.SpawnProbabilityWeight;
            if (randomValue <= currentSum) return mutation;
        }

        return null;
    } 
}