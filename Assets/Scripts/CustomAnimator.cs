using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CustomAnimator : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] public float baseFramesPerSecond;
    [SerializeField] public float velocityImpactOnAnimation = 1f;

    [SerializeField] public bool IsPlaying = true;

    [field: SerializeField]
    public float FPS { get; private set; }
    private int currentFrame = 0;
    private float frameStartTime;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (TryGetComponent(out SpriteRenderer sp))
        {
            spriteRenderer = sp;
        } 
        else
        {
            Debug.LogError("Can not play animation without SpriteRendered");
        }

        FPS = baseFramesPerSecond * velocityImpactOnAnimation;
    }

    void Update()
    {
        if (!IsPlaying) return;

        float currentTime = Time.time;
        if (currentTime - frameStartTime > 1 / FPS)
        {
            currentFrame = currentFrame + 1 > sprites.Length - 1 ? 0 : currentFrame + 1;
            spriteRenderer.sprite = sprites[currentFrame];
            frameStartTime = currentTime;
        }
    }

    public void ChangeAnimationSpeedBasedOnVelocity(SpermStatsData spermStatsData)
    {
        FPS = baseFramesPerSecond * velocityImpactOnAnimation * spermStatsData.MaxSpeed;
    }

}