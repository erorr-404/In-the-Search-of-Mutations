using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public struct HealthChange
{
    public float NewHealth;
    public float Difference;
    public float MaxHealth;
    public HealthModificationReason Reason;
}

public enum HealthModificationReason
{
    Heal,
    PlayerAttack,
    EnemyAttack,
    Spawn
}

public enum DeathReason
{
    PlayerAttack,
    EnemyAttack
}

public struct HealthModificationResult
{
    public bool DamageAccepted;
    public bool TargetDied;
    public MicroImplantData[] RemovedMicroImplants;

}
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpermStats))]
public class Damageable : MonoBehaviour
{
    /// <summary>
    /// Current health of object.
    /// </summary>
    [field: SerializeField]
    public float Health { get; private set; }

    /// <summary>
    /// Maximum amount of health.
    /// </summary>
    public float MaxHealth;

    /// <summary>
    /// Object is invincible if it is false.
    /// </summary>
    public bool canTakeDamageAndHeal = true;

    /// <summary>
    /// If true, gives array of MicroImplants to its killer.
    /// </summary>
    public bool DropMicroImplantsIfDead = true;

    [Header("Healing")]

    /// <summary>
    /// If true, object will heal when enough time passed since last hit.
    /// </summary>
    public bool HealOverTime = true;

    /// <summary>
    /// Time needed to start healing in seconds.
    /// </summary>
    public float HealStartTime = float.MaxValue;

    /// <summary>
    /// Heal interval in seconds;
    /// </summary>
    public float HealCooldown = 1f;

    /// <summary>
    /// Heal amount per second
    /// </summary>
    public float HealPerTick = 1f; // TODO: add micro-implant to modify this param

    [Header("Hit Animation")]
    public bool playHitAnimation = true;
    public float hitHealAnimationDuration = .5f;


    public UnityEvent<HealthChange> onHealthChanged;
    public UnityEvent<float> onHeal;
    public UnityEvent<float> onDamage;
    public UnityEvent<DeathReason> onDeath;

    SpriteRenderer spriteRenderer;
    SpermStats spermStats;
    float lastTimeHit = 0f;
    float lastTimeHeal = 0f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spermStats = GetComponent<SpermStats>();
        
        Health = MaxHealth;
        onHealthChanged?.Invoke(new HealthChange() {NewHealth = Health, Difference = Health, MaxHealth = MaxHealth, Reason = HealthModificationReason.Spawn});
        
        onHealthChanged.AddListener(PlayAnimation);
        onHealthChanged.AddListener(ProcessSubEvents);

        spermStats.onStatsChanged.AddListener(OnStatsChanged);
    }

    void Update()
    {
        float currentTime = Time.time;

        // check cooldown and enable status
        if (HealOverTime && currentTime - lastTimeHit >= HealStartTime && currentTime - lastTimeHeal >= HealCooldown)
        {
            // avoid overhealing
            float healAmount = Mathf.Min(HealPerTick, MaxHealth - Health);
            ModifyHealth(healAmount, HealthModificationReason.Heal);
            
            lastTimeHeal = Time.time;
        }
    }

    public void OnStatsChanged(SpermStatsData newStats)
    {
        HealStartTime = newStats.HealStartTime;
        HealCooldown = newStats.HealCooldown;
        HealPerTick = newStats.HealPerTick;
    }

    /// <summary>
    /// Calls sub-events of onHealthChange
    /// </summary>
    void ProcessSubEvents(HealthChange healthChange)
    {
        if (healthChange.Difference > 0) onHeal?.Invoke(healthChange.Difference);
        if (healthChange.Difference < 0) onDamage?.Invoke(-healthChange.Difference);
    }

    /// <summary>
    /// Takes damage, and dies if health is less than 1.
    /// </summary>
    /// <param name="amount">Must be negative if you want to subtract health.</param>
    /// <returns>Returns true if changes applied accepted.</returns>
    public HealthModificationResult ModifyHealth(float amount, HealthModificationReason reason)
    {   
        // do not accept damage if it is disabled
        if (!canTakeDamageAndHeal) return new HealthModificationResult{DamageAccepted = false};

        // if damage is 0, than we count as it was not accepted
        if (amount == 0) return new HealthModificationResult{DamageAccepted = false};

        // update last time hit counter
        if (reason == HealthModificationReason.PlayerAttack || reason == HealthModificationReason.EnemyAttack) lastTimeHit = Time.time;

        // modify health and invoke event
        Health += amount;
        onHealthChanged?.Invoke(new HealthChange() {NewHealth = Health, Difference = amount, MaxHealth = MaxHealth, Reason = reason});
        
        if (Health <= 0)
        {
            if (reason == HealthModificationReason.PlayerAttack)
            {
                Die(DeathReason.PlayerAttack);
            }
            else
            {
                Die(DeathReason.EnemyAttack);
            }

            return new HealthModificationResult
            {
                DamageAccepted = true,
                TargetDied = true,
                RemovedMicroImplants = DropMicroImplantsIfDead ? GetComponent<SpermStats>().GetMicroImplantsList().ToArray() : Array.Empty<MicroImplantData>()
            };
        }
        
        return new HealthModificationResult
        {
            DamageAccepted = true,
            TargetDied = false
        };
    }

/// <summary>
/// Well, it just kills object.
/// </summary>
    public void Die(DeathReason reason)
    {
        onDeath?.Invoke(reason);
        Destroy(gameObject);
    }

    void PlayAnimation(HealthChange healthChange)
    {
        if (healthChange.Difference > 0)
        {
            StartCoroutine(HealAnimationCoroutine());
        }
        else
        {
            StartCoroutine(HitAnimationCoroutine());
        }
    }

    IEnumerator HitAnimationCoroutine()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitHealAnimationDuration);
        spriteRenderer.color = Color.white;
    }

    IEnumerator HealAnimationCoroutine()
    {
        spriteRenderer.color = Color.green;
        yield return new WaitForSeconds(hitHealAnimationDuration);
        spriteRenderer.color = Color.white;
    }
}
