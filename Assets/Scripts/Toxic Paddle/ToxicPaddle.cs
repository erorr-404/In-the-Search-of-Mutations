using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ToxicPuddle : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 3f;      
    public float damageTickRate = 0.5f;
    
    [HideInInspector] public GameObject owner;
    [HideInInspector] public float tickDamage = 1f;
    [HideInInspector] public bool isPlayerOwner;

    // dictionary remembers last hit time for every object
    private Dictionary<GameObject, float> lastDamageTimes = new Dictionary<GameObject, float>();

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // ignore owner
        if (other.gameObject == owner) return;

        if (other.TryGetComponent<Damageable>(out var targetDamageable))
        {
            // check cooldown
            if (!lastDamageTimes.ContainsKey(other.gameObject) || Time.time >= lastDamageTimes[other.gameObject] + damageTickRate)
            {
                ApplyDamage(other.gameObject, targetDamageable);
                lastDamageTimes[other.gameObject] = Time.time;
            }
        }
    }

    private void ApplyDamage(GameObject targetObj, Damageable targetDamageable)
    {
        float actualDamage = tickDamage;

        // calculate damage based on sperm resistance stats
        if (targetObj.TryGetComponent<SpermStats>(out var targetStats))
        {
            float resistance = targetStats.GetPlayerStats().DamageResistance;
            if (resistance > 0f)
            {
                actualDamage /= resistance;
            }
        }

        HealthModificationResult result = targetDamageable.ModifyHealth(-actualDamage, HealthModificationReason.ToxicPuddle);

        // give received micro-implants to owner
        if (result.TargetDied)
        {
            SpermStats ownerStats = owner.GetComponent<SpermStats>();
            foreach (MicroImplantData microImplant in result.RemovedMicroImplants)
            {
                if (microImplant.Name == "None") continue;
                ownerStats.AddMicroImplant(microImplant);
            }
        }
    }
}