using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class PickupItem : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    #nullable enable
    public MutationData? mutation;
    public MicroImplantData? microImplant;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (microImplant != null)
        {
            if (!collision.gameObject.TryGetComponent<SpermStats>(out var spermStats)) return;
            spermStats.AddMicroImplant(microImplant);

            Debug.Log("Someone collected " + microImplant.Name + " micro-implant");
            Destroy(gameObject);
        }
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