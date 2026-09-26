using UnityEngine;
using UnityEngine.Events;


public struct HealthChange
{
    public float NewHealth;
    public float Difference;
}

public enum HealthModificationReason
{
    Heal,
    RegularAttack
}

public class Damageable : MonoBehaviour
{
    /// <summary>
    /// Current health of object.
    /// </summary>
    public float Health { get; private set; }

    /// <summary>
    /// Maximum amount of health.
    /// </summary>
    public float MaxHealth;

    /// <summary>
    /// Object is invincible if it is false;
    /// </summary>
    public bool canTakeDamageAndHeal = true;


    public UnityEvent<HealthChange> onHealthChanged;
    public UnityEvent onDeath;

    private void Start()
    {
        Health = MaxHealth;
        onHealthChanged?.Invoke(new HealthChange() {NewHealth = Health, Difference = Health});
    }

    /// <summary>
    /// Takes damage, and dies if health is less than 1.
    /// </summary>
    /// <param name="amount">Must be negative if you want to subtract health.</param>
    /// <returns>Returns true if changes applied accepted.</returns>
    public bool ModifyHealth(float amount, HealthModificationReason reason)
    {
        if (!canTakeDamageAndHeal) return false;

        Health += amount;
        onHealthChanged?.Invoke(new HealthChange() {NewHealth = Health, Difference = -amount});
        
        if (Health <= 0)
        {
            Die();
        }
        
        return true;
    }

/// <summary>
/// Well, it just kills object.
/// </summary>
    public void Die()
    {
        onDeath?.Invoke();
        Destroy(gameObject);
    }
}
