using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Invader : MonoBehaviour
{
    public Image image { get; private set; }
    public Sprite[] animationSprites = new Sprite[0];
    public float animationTime = 1f;
    public int animationFrame { get; private set; }
    public int score = 10;
    public System.Action<Invader> killed;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = animationSprites[0];
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), animationTime, animationTime);
    }

    private void AnimateSprite()
    {
        animationFrame++;

        // Loop back to the start if the animation frame exceeds the length
        if (animationFrame >= animationSprites.Length)
        {
            animationFrame = 0;
        }

        image.sprite = animationSprites[animationFrame];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            killed?.Invoke(this);
        }
    }
}
