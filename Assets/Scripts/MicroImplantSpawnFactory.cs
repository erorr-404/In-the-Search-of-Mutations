using System.Collections.Generic;
using UnityEngine;


public class MicroImplantSpawnFactory : SpawnFactory
{
    [SerializeField] private PickupItem pickupPrefab;
    [SerializeField] private List<MicroImplantData> implantPool;

    /// <summary>
    /// Spawns random MicroImplant at given position.
    /// </summary>
    /// <param name="position">Vector2 where to spawn.</param>
    public override void Spawn(Vector2 position)
    {
        if (pickupPrefab == null) 
        {
            Debug.LogError("PickupPrefab is null"); 
            return;
        }

        MicroImplantData selected = GetWeightedRandomImplant();
        if (selected == null)
        {
            Debug.LogError("Can not select micro-implant.");
            return;
        }

        PickupItem pickup = Instantiate(pickupPrefab, position, Quaternion.identity);
        pickup.SetMicroImplant(selected);

        Debug.Log("Spawning MicroImplant " + selected.Name + " at X=" + position.x.ToString() + " Y=" + position.y.ToString());
    }

    /// <summary>
    /// Spawns specific micro-implant at specific position.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="implantData">type of MicroImplantData</param>
    public void Spawn(Vector2 position, MicroImplantData implantData)
    {
        if (pickupPrefab == null)
        {
            Debug.LogError("PickupPrefab is null");
            return;
        }

        if (implantData == null)
        {
            Debug.LogError("Micro-implant data is null.");
            return;
        }

        PickupItem pickup = Instantiate(pickupPrefab, position, Quaternion.identity);
        pickup.SetMicroImplant(implantData);
    }

    private MicroImplantData GetWeightedRandomImplant()
    {
        if (implantPool.Count == 0) return null;
        if (implantPool.Count == 1) return implantPool[1];

        float totalWeight = 0f;
        foreach (MicroImplantData implant in implantPool)
        {
            if (implant != null) totalWeight += implant.SpawnProbabilityWeight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float currentSum = 0f;

        foreach (MicroImplantData implant in implantPool)
        {
            if (implant == null) continue;
            currentSum += implant.SpawnProbabilityWeight;
            if (randomValue <= currentSum) return implant;
        }

        return null;
    }
}