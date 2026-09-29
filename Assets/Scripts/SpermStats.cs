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

    [Header("Sprint and Stamina")]
    [SerializeField] public float SprintSpeedMultiplier;
    [SerializeField] public float SprintTurnMultiplier;
    [SerializeField] public float SprintAccelerationMultiplier;
    [SerializeField] public float MaxStamina;
    [SerializeField] public float StaminaConsumption;
    [SerializeField] public float StaminaRegenerationPerSecond;

    [Header("Healing")]
    [SerializeField] public float HealStartTime;
    [SerializeField] public float HealCooldown;
    [SerializeField] public float HealPerTick;

    [SerializeField] public bool HasSpikes;
    [SerializeField] public bool HasToxicTrail;
    [SerializeField] public bool HasRGB;
    [SerializeField] public bool HasGun;
}

[RequireComponent(typeof(RGBAnimation))]
public class SpermStats : MonoBehaviour
{
    [SerializeField] private List<MutationData> mutations;
    [SerializeField] private List<MicroImplantData> microImplants;

    [SerializeField] private SpermStatsData _currentStats;
    [SerializeField] private MicroImplantData defaultMicroImplant;

    [SerializeField] private GameObject spikes;
    [SerializeField] private GameObject acidTank;
    [SerializeField] private RGBAnimation rgbAnimation;

    public UnityEvent<SpermStatsData> onStatsChanged;
    public UnityEvent<List<MutationData>> onMutationsChange;
    public UnityEvent<List<MicroImplantData>> onMicroImplantsChange;

    void Awake()
    {
        if (microImplants.Count == 0) microImplants.Add(defaultMicroImplant);
        UpdateStats();
    }

    void Start()
    {
        rgbAnimation = GetComponent<RGBAnimation>();

        spikes.SetActive(false);
        acidTank.SetActive(false);
        rgbAnimation.Active = false;
    }

    /// <summary>
    /// Add micro-implant to the list. From now it will influence player stats.
    /// </summary>
    /// <param name="microImplant">The micro-implant you want to add.</param>
    public void AddMicroImplant(MicroImplantData microImplant)
    {
        if (microImplant == null) return;
        microImplants.Add(microImplant);

        UpdateStats();
        onMicroImplantsChange?.Invoke(microImplants);
    }

    /// <summary>
    /// Add mutation to the list.
    /// </summary>
    /// <param name="mutation">Mutation to add.</param>
    public void AddMutation(MutationData mutation)
    {
        if (mutation == null) return;
        mutations.Add(mutation);
        onMutationsChange?.Invoke(mutations);
    }

    /// <summary>
    /// Returns list of micro-implants, that are installed on the player.
    /// </summary>
    /// <returns>I already said what it returns.</returns>
    public List<MicroImplantData> GetMicroImplantsList()
    {
        return microImplants;
    }

    public List<MutationData> GetMutationsList()
    {
        return mutations;
    }

    /// <summary>
    /// Removes micro-implant at given index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>Returns true if it was successfully removed.</returns>
    public bool RemoveMicroImplantAtIndex(int index)
    {
        // do not remove default implant
        if (index == 0 || index > microImplants.Count - 1) return false;
        microImplants.RemoveAt(index);

        UpdateStats();
        onMicroImplantsChange?.Invoke(microImplants);

        return true;
    }

    /// <summary>
    /// Removes mutation at given position.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool RemoveMutationAtIndex(int index)
    {
        if (index == 0 || index > mutations.Count - 1) return false;
        mutations.RemoveAt(index);
        onMutationsChange?.Invoke(mutations);
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

        if (res) 
        {
            UpdateStats();         
            onMicroImplantsChange?.Invoke(microImplants);
        }

        return res;
    }

    /// <summary>
    /// Removes specific mutation.
    /// </summary>
    /// <param name="mutationData"></param>
    /// <returns></returns>
    public bool RemoveMutation(MutationData mutationData)
    {
        bool res = mutations.Remove(mutationData);
        if (res) onMutationsChange?.Invoke(mutations);
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

        spikes.SetActive(stats.HasSpikes);
        acidTank.SetActive(stats.HasToxicTrail);
        rgbAnimation.Active = stats.HasRGB;
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

        float sprint_speed_mult = 1f;
        float sprint_turn_mult = 1f;
        float sprint_accel_mult = 1f;
        float max_stamina = 1f;
        float stamina_consumption = 1f;
        float stamina_regen = 1f;

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

            sprint_speed_mult *= implant.SprintSpeedMultiplier;
            sprint_turn_mult *= implant.SprintTurnMultiplier;
            sprint_accel_mult *= implant.SprintAccelerationMultiplier;
            max_stamina *= implant.MaxStamina;
            stamina_consumption *= implant.StaminaConsumption;
            stamina_regen *= implant.StaminaRegenerationPerSecond;

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

            // Обов'язково заповнюємо ці поля:
            SprintSpeedMultiplier = sprint_speed_mult,
            SprintTurnMultiplier = sprint_turn_mult,
            SprintAccelerationMultiplier = sprint_accel_mult,
            MaxStamina = max_stamina,
            StaminaConsumption = stamina_consumption,
            StaminaRegenerationPerSecond = stamina_regen,

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
