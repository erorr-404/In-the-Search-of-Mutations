using UnityEngine;

[CreateAssetMenu(fileName = "MicroImplant", menuName = "ScriptableObjects/MicroImplants", order = 1)]
public class MicroImplantData : ScriptableObject
{
    // visuals
    [Header("Visuals")]
    public string Name;
    [SerializeField] [TextArea] public string Description;
    public Sprite Icon;

    [Header("Spawn Settings")]
    public float SpawnProbabilityWeight = 10f;

    // modifiers
    [Header("Modifiers")]
    public float MaxSpeed = 1f;
    public float Acceleration = 1f;
    public float Deceleration = 1f;
    public float TurnSpeed = 1f;
    public float Health = 1f;
    public float Size = 1f;
    public float AttackDamage = 1f;
    public float DamageResistance = 1f;

    // flags
    [Header("Special Abilities")]
    public bool Spikes = false;
    public bool ToxicTrail = false;
    public bool RGBLight = false;
    public bool Gun = false;
}
