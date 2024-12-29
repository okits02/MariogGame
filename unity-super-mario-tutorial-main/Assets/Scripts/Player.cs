using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CapsuleCollider2D capsuleCollider { get; private set; }
    public PlayerMovement movement { get; private set; }
    public DeathAnimation deathAnimation { get; private set; }

    public PlayerSpriteRenderer smallRenderer;
    public PlayerSpriteRenderer bigRenderer;
    public PlayerSpriteRenderer fireRenderer;
    private PlayerSpriteRenderer activeRenderer;

    public bool big => bigRenderer.enabled;
    public bool dead => deathAnimation.enabled;

    public bool fire => fireRenderer.enabled;
    public bool starpower { get; private set; }
    

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        movement = GetComponent<PlayerMovement>();
        deathAnimation = GetComponent<DeathAnimation>();
        activeRenderer = smallRenderer;
    }

    public void Hit()
    {
        if (!dead && !starpower)
        {
            if (fire)
            {
                bigRenderer.enabled = true;
                fireRenderer.enabled = false;
                activeRenderer = bigRenderer;
                StartCoroutine(ScaleAnimation());
            }
            else if (big)
            {
                Shrink();
            }
            else
            {
                Death();
            }
        }
    }

    public void Death()
    {
        smallRenderer.enabled = false;
        bigRenderer.enabled = false;
        fireRenderer.enabled = false;
        deathAnimation.enabled = true;

        GameManager.Instance.ResetLevel(3f);
    }

    public void Grow()
    {
        smallRenderer.enabled = false;
        bigRenderer.enabled = true;
        fireRenderer.enabled = false;
        activeRenderer = bigRenderer;

        capsuleCollider.size = new Vector2(1f, 2f);
        capsuleCollider.offset = new Vector2(0f, 0.5f);

        StartCoroutine(ScaleAnimation());
    }

    public void Shrink()
    {
        smallRenderer.enabled = true;
        bigRenderer.enabled = false;
        fireRenderer.enabled = false;
        activeRenderer = smallRenderer;

        capsuleCollider.size = new Vector2(1f, 1f);
        capsuleCollider.offset = new Vector2(0f, 0f);

        StartCoroutine(ScaleAnimation());
    }

    public void Fire()
    {
        smallRenderer.enabled = false;
        bigRenderer.enabled = false;
        fireRenderer.enabled = true;
        activeRenderer = fireRenderer;

        capsuleCollider.size = new Vector2(1f, 2f);
        capsuleCollider.offset = new Vector2(0f, 0.5f);

        StartCoroutine(FireTransformationAnimation());
    }

    private IEnumerator FireTransformationAnimation()
    {

        if (activeRenderer == smallRenderer)
        {
            float elapsed = 0f;
            float duration = 0.5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fireRenderer.enabled = !fireRenderer.enabled;
                yield return new WaitForSeconds(0.1f);
            }
        }

        float fireDuration = 1f;
        float blinkInterval = 0.1f;
        float fireElapsed = 0f;

        while (fireElapsed < fireDuration)
        {
            fireRenderer.enabled = !fireRenderer.enabled;
            fireElapsed += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        smallRenderer.enabled = false;
        bigRenderer.enabled = false;
        fireRenderer.enabled = true;
    }



    private IEnumerator ScaleAnimation()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            activeRenderer.enabled = !activeRenderer.enabled;

            yield return new WaitForSeconds(0.1f); 
        }

        smallRenderer.enabled = false;
        bigRenderer.enabled = false;
        activeRenderer.enabled = true;
    }

    public bool IsFireState()
    {
        return fire;
    }

    public void Starpower()
    {
        StartCoroutine(StarpowerAnimation());
    }

    private IEnumerator StarpowerAnimation()
    {
        starpower = true;

        float elapsed = 0f;
        float duration = 10f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (Time.frameCount % 4 == 0) {
                activeRenderer.spriteRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }

            yield return null;
        }

        activeRenderer.spriteRenderer.color = Color.white;
        starpower = false;
    }

}
