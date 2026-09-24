using System.Collections.Generic;
using UnityEngine;




[RequireComponent(typeof(SpriteRenderer))]
public class PickupItem : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    #nullable enable
    public MutationData? mutation;
    public MicroImplantData? microImplant;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Sets this pickup to represent a mutation and updates its sprite. Removes micro implant data.
    /// </summary>
    /// <param name="mut">The mutation data to assign to this pickup.</param>
    public void SetMutation(MutationData mut)
    {
        mutation = mut;
        microImplant = null;
        UpdateSprite(mut.Icon);
    }

    /// <summary>
    /// Sets this pickup to represent a micro implant and updates its sprite. Removes mutation data.
    /// </summary>
    /// <param name="implant">Micro implant data to assign.</param>
    public void SetMicroImplant(MicroImplantData implant)
    {
        microImplant = implant;
        mutation = null;
        UpdateSprite(implant.Icon);
    }
    
    private void UpdateSprite(Sprite sp)
    {
        spriteRenderer.sprite = sp;
    }
}