using UnityEngine;

[RequireComponent(typeof(Damageable), typeof(SpermStats))]
public class RamAttacker : MonoBehaviour
{
    [Tooltip("Поріг кута атаки (0.5 = конус 60 градусів перед носом)")]
    [SerializeField] private float ramAngleThreshold = 0.5f;
    [SerializeField] private float attackCooldown = 0.8f;
    [SerializeField] private float kickbackForce = 5f;
    [SerializeField] private bool isPlayer = false;

    private SpermStats myStats;
    private Damageable myDamageable;
    private float lastAttackTime;

    void Awake()
    {
        myStats = GetComponent<SpermStats>();
        myDamageable = GetComponent<Damageable>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryExecuteRam(collision.gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        TryExecuteRam(collision.gameObject);
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        TryExecuteRam(collision.gameObject);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        TryExecuteRam(collision.gameObject);
    }

    private void TryExecuteRam(GameObject target)
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        if (!target.TryGetComponent(out Damageable targetDamageable)) return;

        Vector2 myForward = transform.up;
        Vector2 toTarget = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;

        // check if I hit forward
        float attackDot = Vector2.Dot(myForward, toTarget);
        if (attackDot < ramAngleThreshold) return;

        // check if target turned side to me
        Vector2 targetForward = target.transform.up;
        float targetDefenseDot = Vector2.Dot(targetForward, -toTarget);

        if (targetDefenseDot < ramAngleThreshold)
        {
            PerformHit(targetDamageable, target);
        }
        else
        {
            ApplyKickback(target, toTarget * 0.5f);
        }
    }

    void PerformHit(Damageable targetDamageable, GameObject targetObject)
    {
        lastAttackTime = Time.time;

        SpermStatsData currentStats = myStats.GetPlayerStats();
        float baseDamage = currentStats.AttackDamage;

        if (targetObject.TryGetComponent<SpermStats>(out var targetStats))
        {
            float resistance = targetStats.GetPlayerStats().DamageResistance;
            if (resistance > 0f)
            {
                baseDamage /= resistance;
            }
        }

        HealthModificationResult healthModificationResult =  targetDamageable.ModifyHealth(-baseDamage, isPlayer ? HealthModificationReason.PlayerAttack : HealthModificationReason.EnemyAttack);

        Vector2 pushDir = ((Vector2)targetObject.transform.position - (Vector2)transform.position).normalized;
        ApplyKickback(targetObject, pushDir);

        if (targetObject.TryGetComponent<SpermStats>(out var targetSt) && targetSt.GetPlayerStats().HasSpikes)
        {
            myDamageable.ModifyHealth(-baseDamage * 0.5f, isPlayer ? HealthModificationReason.PlayerAttack : HealthModificationReason.EnemyAttack);
        }

        // take micro-implants from died victim
        if (healthModificationResult.TargetDied)
        {
            foreach (MicroImplantData implant in healthModificationResult.RemovedMicroImplants)
            {
                if (implant.Name == "None") continue; // this is too overpowered if killed 2 or more, so skip it
                myStats.AddMicroImplant(implant);
            }
        }
    }

    void ApplyKickback(GameObject target, Vector2 direction)
    {
        if (target.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.AddForce(direction * kickbackForce, ForceMode2D.Force);
        }
    }
}