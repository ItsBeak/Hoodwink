using UnityEngine;
using UnityEngine.UI;

public class H_HealthEffect : MonoBehaviour
{
    public Image hurtFlashImage;

    public Sprite hitSprite;
    public Sprite healSprite;

    public float decaySpeed;

    public AudioClip[] hurtClips;
    public AudioClip[] healClips;
    AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (hurtFlashImage.color.a > 0)
        {
            hurtFlashImage.color = new Color(hurtFlashImage.color.r, hurtFlashImage.color.g, hurtFlashImage.color.b, hurtFlashImage.color.a - decaySpeed * Time.deltaTime);
        }
    }

    public void Hit()
    {
        hurtFlashImage.color = Color.white;
        hurtFlashImage.sprite = hitSprite;
        source.PlayOneShot(hurtClips[Random.Range(0, hurtClips.Length)]);
    }

    public void Heal()
    {
        hurtFlashImage.color = Color.white;
        hurtFlashImage.sprite = healSprite;
        source.PlayOneShot(healClips[Random.Range(0, healClips.Length)]);
    }
}
