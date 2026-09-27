using System;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public enum VisualMutationSlot
{
    Head,
    Eyes,
    Ears,
    Mouth,
    Torso,
    Back,
    Penis,
    Arms,
    Legs,
    Skin,
    Additional,
    None // does not affect visual
}

/// <summary>
/// Contains info for HumanGenerator about mutation.
/// </summary>
[Serializable]
public struct MutationVisuals
{
    /// <summary>
    /// Part of the body, that will change.
    /// </summary>
    public VisualMutationSlot VisualSlot;

    /// <summary>
    /// Anchor for proper alignment.
    /// </summary>
    public Vector2 VisualAnchor;

    /// <summary>
    /// Rotation with centre in anchor.
    /// </summary>
    public float Rotation;

    /// <summary>
    /// Actual image.
    /// </summary>
    public Sprite Sprite;
}

/// <summary>
/// Contains full data about mutation
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mutation", order = 1)]
public class MutationData : ScriptableObject
{
    [Header("Info")]
    public string Name;
    public string Description;
    public Sprite Icon;
    public float SpawnProbabilityWeight = 10f;

    [Header("Visuals")]
    [field: SerializeField]
    public MutationVisuals Visuals;

    [Header("Modifiers")]
    public float Iq = 1f;
    public float Attractiveness = 1f;
    public float AbilityToReproduce = 1f;
    public float Strength = 1f;
    public float Hearing = 1f;
    public float Eyesight = 1f;
    public float Speed = 1f;
    public float Stamina = 1f;
    public float Power = 1f;
    public float Immunity = 1f;
    public float Charisma = 1f;

    [Header("Additional flags")]
    public bool CanBreathUnderWater;
    public bool CanFly;
}
