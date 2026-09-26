using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains all modifications, that influence player behaviour
/// </summary>
[System.Serializable]
public struct SpermStatsData
{
    [SerializeField] public float MaxSpeed; // 5f
    [SerializeField] public float Acceleration; // 8f
    [SerializeField] public float Deceleration; // 10f
    [SerializeField] public float TurnSpeed; // 1f
    [SerializeField] public float Health;
    [SerializeField] public float Size;
    [SerializeField] public float AttackDamage;
    [SerializeField] public float DamageResistance;

    [Header("Healing")]
    [SerializeField] public float HealStartTime;
    [SerializeField] public float HealCooldown;
    [SerializeField] public float HealPerTick;

    [SerializeField] public bool HasSpikes;
    [SerializeField] public bool HasToxicTrail;
    [SerializeField] public bool HasRGB;
    [SerializeField] public bool HasGun;
}

public class SpermStats : MonoBehaviour
{
    [SerializeField] private List<MutationData> mutations;
    [SerializeField] private List<MicroImplantData> microImplants;

    [SerializeField] private SpermStatsData _currentStats;
    [SerializeField] private MicroImplantData defaultMicroImplant;

    [SerializeField] private GameObject spikes;
    [SerializeField] private GameObject acidTank;

    public UnityEvent<SpermStatsData> onStatsChanged;

    void Awake()
    {
        if (microImplants.Count == 0) microImplants.Add(defaultMicroImplant);
        UpdateStats();
    }

    void Start()
    {
        spikes.SetActive(false);
        acidTank.SetActive(false);
    }

    /// <summary>
    /// Add micro-implant to the list. From now it will influence player stats.
    /// </summary>
    /// <param name="microImplant">The micro-implant you want to add.</param>
    public void AddMicroImplant(MicroImplantData microImplant)
    {
        microImplants.Add(microImplant);

        if (microImplant.Name == "Cytoskeletal Framework") spikes.SetActive(true);
        if (microImplant.Name == "Acid Tank") acidTank.SetActive(true);

        UpdateStats();
    }

    /// <summary>
    /// Returns list of micro-implants, that are installed on the player.
    /// </summary>
    /// <returns>I already said what it returns.</returns>
    public List<MicroImplantData> GetMicroImplantsList()
    {
        return microImplants;
    }

    /// <summary>
    /// Removes micro-implant at given index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>Returns true if it was successfully removed.</returns>
    public bool RemoveMicroImplantAtIndex(int index)
    {
        // bovdur check
        if (index == 0 || index > microImplants.Count - 1) return false;
        microImplants.RemoveAt(index);

        UpdateStats();

        return true;
    }

    /// <summary>
    /// Removes specific micro-implant from the list.
    /// </summary>
    /// <param name="microImplant">Micro-implant you want to remove.</param>
    /// <returns>Returns true, if operation was successful.</returns>
    public bool RemoveMicroImplant(MicroImplantData microImplant)
    {
        bool res = microImplants.Remove(microImplant);

        if (res) UpdateStats();

        return res;
    }

    /// <summary>
    /// Returns current player modifiers.
    /// </summary>
    public SpermStatsData GetPlayerStats()
    {
        return _currentStats;
    }

    private void UpdateStats()
    {
        SpermStatsData stats = RecalculateMicroImplantEffects();
        _currentStats = stats;

        transform.localScale = new Vector3(stats.Size, stats.Size, stats.Size);

        onStatsChanged?.Invoke(stats);
    }

    /// <summary>
    /// Calculates player stats from the list, by multiplying all of them.
    /// </summary>
    /// <returns>Actual player stats.</returns>
    private SpermStatsData RecalculateMicroImplantEffects()
    {
        float max_speed = 1f;
        float accel = 1f;
        float decel = 1f;
        float turn_speed = 1f;
        float health = 1f;
        float size = 1f;
        float attack_d = 1f;
        float dam_res = 1f;

        float heal_start = 1f;
        float heal_cooldown = 1f;
        float heal_per_tick = 1f;

        bool spikes = false;
        bool toxic_trail = false;
        bool rgb = false;
        bool gun = false;

        foreach (MicroImplantData implant in microImplants)
        {
            max_speed *= implant.MaxSpeed;
            accel *= implant.Acceleration;
            decel *= implant.Deceleration;
            turn_speed *= implant.TurnSpeed;
            health *= implant.Health;
            size *= implant.Size;
            attack_d *= implant.AttackDamage;
            dam_res *= implant.DamageResistance;

            heal_start *= implant.HealStartTime;
            heal_cooldown *= implant.HealCooldown;
            heal_per_tick *= implant.HealPerTick;

            spikes = spikes || implant.Spikes;
            toxic_trail = toxic_trail || implant.ToxicTrail;
            rgb = rgb || implant.RGBLight;
            gun = gun || implant.Gun;
        }

        SpermStatsData stats = new SpermStatsData
        {
            MaxSpeed = max_speed,
            Acceleration = accel,
            Deceleration = decel,
            TurnSpeed = turn_speed,
            Health = health,
            Size = size,
            AttackDamage = attack_d,
            DamageResistance = dam_res,

            HealStartTime = heal_start,
            HealCooldown = heal_cooldown,
            HealPerTick = heal_per_tick,

            HasSpikes = spikes,
            HasToxicTrail = toxic_trail,
            HasRGB = rgb,
            HasGun = gun
        };

        return stats;
    }
}
