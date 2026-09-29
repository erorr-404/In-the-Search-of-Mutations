using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RGBAnimation : MonoBehaviour
{
    [SerializeField] float speed;
    public bool Active;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!Active) return;
        float hue = Mathf.Repeat(Time.time * speed, 1f);
        spriteRenderer.color = Color.HSVToRGB(hue, 1f, 1f);
    }
}