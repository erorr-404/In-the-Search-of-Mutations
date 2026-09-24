using UnityEngine;

public enum MutationSlots
{
    Head,
    Eyes,
    Mouth,
    Torso,
    Arms,
    Legs,
    Skin,
    Personality
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mutation", order = 1)]
public class MutationData : ScriptableObject
{
    // visuals
    public string Name;
    public string Description;
    public Sprite Icon;

    // mutation slot
    public MutationSlots Slot;
    public Sprite Sprite;

    // modifiers
    public float Iq;
    public float Attractiveness;
    public float Strength;
}
