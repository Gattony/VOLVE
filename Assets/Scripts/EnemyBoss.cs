using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemyBoss : MonoBehaviour, IDamageable
{
    enum SlamPattern
    {
        Radial,
        Plus,
        Diagonal,
        Spiral
    }

    private List<GameObject> warningArrows = new List<GameObject>();

    [Header("Phase 2 Pulse")]
    public Color phase1Color = Color.white;
    public Color phase2Color = new Color(1f, 0.25f, 0.25f);
    public float pulseSpeed = 3f;

    private bool isPhase2 = false;
    private SpriteRenderer sr;

    [Header("Warning Beep")]
    public float warningBeepInterval = 0.15f;

    [Header("Spiral Slam")]
    public float spiralStep = 20f;
    private float spiralOffset = 0f;

    public float spiralSpawnDelay = 0.1f;
    public float spiralWidth = 2f;
    public float spiralLength = 8f;
    public int spiralLineCount = 16;

    [Header("Boss Slam Attack")]
    public GameObject warningLinePrefab;
    public GameObject slamLinePrefab;

    [Header("Slam Visual")]
    public Color slamColor = new Color(1f, 0.4f, 0.1f);

    [Header("Telegraph Flash")]
    public float flashDuration = 0.5f;
    public int flashCount = 5;
    public Color warningColor = Color.yellow;
    public Color flashColor = Color.red;

    private float[] slamAngles;
    public int slamLineCount = 8;
    public float warningDuration = 1.25f;
    public float slamWidth = 7f;
    public float slamLength = 7f;
    public float attackCooldown = 4f;

    private bool phase2Started = false;
    private bool isTransitioningPhase = false;
    private float attackTimer;
    private bool isAttacking;
    private bool introFinished;

    public Action OnEnemyDestroyed;

    [Header("Boss Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip roarSound;
    public AudioClip warningSound;
    public AudioClip slamSound;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    public int enemyDamage = 1;
    public int scoreDrop = 1500;

    [Header("Movement")]
    public float moveSpeed = 2f;
    private Transform player;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float knockbackForce = 10f;
    private bool isKnockedBack = false;

    public Transform arrowPivot;

    [Header("Drops")]
    public GameObject expOrbPrefab;
    public GameObject bloodEffectPrefab;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        StartCoroutine(FindPlayer());

        StartCoroutine(BossIntro());

    }

    IEnumerator FindPlayer()
    {
        while (player == null)
        {
            if (PlayerCharacter.Instance != null)
            {
                player = PlayerCharacter.Instance.transform;
                Debug.Log("Boss found player!");
                yield break;
            }

            yield return null; // check again next frame
        }
    }

    void FixedUpdate()
    {
        if (!introFinished || player == null || isTransitioningPhase)
            return;

        HandleMovement();
        HandleAttacks();

        if (isPhase2 && sr != null)
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            sr.color = Color.Lerp(phase1Color, phase2Color, pulse);
        }
    }

    void HandleMovement()
    {
        if (player == null || isKnockedBack)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        if (direction.x > 0)
            spriteRenderer.flipX = true;
        else if (direction.x < 0)
            spriteRenderer.flipX = false;
    }

    void HandleAttacks()
    {
        if (isAttacking) return;

        attackTimer += Time.fixedDeltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0;
            StartCoroutine(SlamAttack());
        }
    }
    IEnumerator WarningBeepLoop()
    {
        float timer = 0f;

        while (timer < warningDuration)
        {
            if (audioSource != null && warningSound != null)
            {
                audioSource.PlayOneShot(warningSound);
            }

            yield return new WaitForSeconds(warningBeepInterval);
            timer += warningBeepInterval;
        }
    }

    IEnumerator BossIntro()
    {
        if (audioSource != null && roarSound != null)
            audioSource.PlayOneShot(roarSound);

        rb.velocity = Vector2.zero;

        if (animator != null)
            animator.SetTrigger("Roar");

        // Wait until roar animation actually finishes
        yield return new WaitForSeconds(2f);

        introFinished = true;
    }

    IEnumerator FlashSingleArrow(SpriteRenderer sr)
    {
        for (int i = 0; i < flashCount; i++)
        {
            if (sr == null) yield break;

            sr.color = flashColor;
            yield return new WaitForSeconds(flashDuration / (flashCount * 2));

            sr.color = warningColor;
            yield return new WaitForSeconds(flashDuration / (flashCount * 2));
        }
    }

    IEnumerator SpiralAttack()
    {
        float angle = spiralOffset;

        for (int i = 0; i < spiralLineCount; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            GameObject arrow = Instantiate(
                warningLinePrefab,
                arrowPivot.position,
                rotation
            );

            SpriteRenderer sr = arrow.GetComponent<SpriteRenderer>();
            Collider2D col = arrow.GetComponent<Collider2D>();

            arrow.transform.localScale = new Vector3(spiralWidth, spiralLength, 1f);

            if (sr != null)
                sr.color = warningColor;

            // WARNING SOUND
            if (audioSource != null && warningSound != null)
                audioSource.PlayOneShot(warningSound);

            // Handle slam timing separately
            StartCoroutine(HandleSpiralLine(sr, col));

            angle += spiralStep;

            // THIS controls spiral speed
            yield return new WaitForSeconds(spiralSpawnDelay);
        }

        spiralOffset += spiralStep;
    }

    IEnumerator HandleSpiralLine(SpriteRenderer sr, Collider2D col)
    {
        yield return new WaitForSeconds(warningDuration - flashDuration);

        if (sr != null)
            yield return StartCoroutine(FlashSingleArrow(sr));

        // SLAM SOUND
        if (audioSource != null && slamSound != null)
            audioSource.PlayOneShot(slamSound);

        SpriteRenderer slamSprite = slamLinePrefab.GetComponent<SpriteRenderer>();

        if (sr != null && slamSprite != null)
        {
            sr.sprite = slamSprite.sprite;
            sr.color = slamColor;
        }

        if (col != null)
        {
            col.enabled = true;
            StartCoroutine(DisableHitboxAfterDelay(col));
        }

        StartCoroutine(FadeOutArrow(sr));
    }

    SlamPattern GetRandomPattern()
    {
        // Phase 2 attacks
        if (phase2Started)
        {
            int roll = UnityEngine.Random.Range(0, 2);

            return roll == 0 ? SlamPattern.Radial : SlamPattern.Spiral;
        }

        // Phase 1 attacks
        else
        {
            int roll = UnityEngine.Random.Range(0, 2);

            return roll == 0 ? SlamPattern.Plus : SlamPattern.Diagonal;
        }
    }
    float[] GetPatternAngles(SlamPattern pattern)
    {
        switch (pattern)
        {
            case SlamPattern.Plus:
                return new float[] { 0, 90, 180, 270 };

            case SlamPattern.Diagonal:
                return new float[] { 45, 135, 225, 315 };

            case SlamPattern.Spiral:
            case SlamPattern.Radial:

                float[] angles = new float[slamLineCount];
                float step = 360f / slamLineCount;

                for (int i = 0; i < slamLineCount; i++)
                {
                    angles[i] = i * step + spiralOffset;
                }

                if (pattern == SlamPattern.Spiral)
                    spiralOffset += spiralStep;

                return angles;
        }

        return new float[] { 0, 90, 180, 270 };
    }

    IEnumerator SlamAttack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        if (animator != null)
            animator.SetTrigger("Prepare");

        SlamPattern pattern = GetRandomPattern();

        if (pattern == SlamPattern.Spiral)
        {
            yield return StartCoroutine(SpiralAttack());

            if (animator != null)
                animator.SetBool("IsMoving", true);

            isAttacking = false;
            yield break;
        }

        SpawnWarningLines(pattern);

        yield return new WaitForSeconds(warningDuration - flashDuration);

        yield return StartCoroutine(FlashTelegraph());

        if (animator != null)
            animator.SetTrigger("Slam");

        yield return StartCoroutine(FlashTelegraph());

        SpawnSlamLines();

        yield return new WaitForSeconds(0.4f);

        isAttacking = false;
    }

    IEnumerator ExpandArrow(Transform arrow)
    {
        float timer = 0f;

        Vector3 startScale = new Vector3(1f, 0f, 1f);
        Vector3 endScale = new Vector3(slamWidth, slamLength, 1f);

        if (arrow == null) yield break;

        arrow.localScale = startScale;

        while (timer < warningDuration)
        {
            if (arrow == null) yield break;   // SAFETY CHECK

            timer += Time.deltaTime;
            float t = timer / warningDuration;

            arrow.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }
    }

    IEnumerator FadeOutArrow(SpriteRenderer sr)
    {
        float t = 0;

        Color start = sr.color;
        
        while (t < 0.85f)
        {
            if (sr == null) yield break;
            t += Time.deltaTime;
            float alpha = 1 - (t / 0.85f);

            sr.color = new Color(start.r, start.g, start.b, alpha);

            yield return null;
        }

        Destroy(sr.gameObject);
    }

    void SpawnWarningLines(SlamPattern pattern)
    {
        StartCoroutine(WarningBeepLoop());

        warningArrows.Clear();

        float[] angles = GetPatternAngles(pattern);
        slamAngles = angles;

        foreach (float angle in angles)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            GameObject arrow = Instantiate(
                warningLinePrefab,
                arrowPivot.position,
                rotation
            );

            SpriteRenderer sr = arrow.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = warningColor;

            warningArrows.Add(arrow);

            StartCoroutine(ExpandArrow(arrow.transform));
        }
    }

    void SpawnSlamLines()
    {
        StopCoroutine(nameof(WarningBeepLoop));

        if (audioSource != null && slamSound != null)
            audioSource.PlayOneShot(slamSound);

        foreach (GameObject arrow in warningArrows)
        {
            if (arrow == null) continue;

            SpriteRenderer sr = arrow.GetComponent<SpriteRenderer>();
            SpriteRenderer slamSprite = slamLinePrefab.GetComponent<SpriteRenderer>();
            Collider2D col = arrow.GetComponent<Collider2D>();

            if (sr != null && slamSprite != null)
            {
                sr.sprite = slamSprite.sprite;
                sr.color = slamColor;

                if (col != null)
                {
                    col.enabled = true;
                    StartCoroutine(DisableHitboxAfterDelay(col));
                }

                StartCoroutine(FadeOutArrow(sr));
            }
        }

        warningArrows.Clear();
    }

    public void TakeDamage(int damage, Vector2 knockbackSource)
    {
        currentHealth -= damage;

        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);

        StartCoroutine(FlashWhite());
        StartCoroutine(ApplyKnockback(knockbackSource));

        if (currentHealth <= 0)
        {
            Die();
        }

        CheckPhase2();
    }

    IEnumerator Phase2Transition()
    {
        if (audioSource != null && roarSound != null)
            audioSource.PlayOneShot(roarSound);

        // wait until current attack finishes
        while (isAttacking)
            yield return null;

        rb.velocity = Vector2.zero;

        // Remove current attack warnings
        DestroyAllLines();

        // Play roar animation
        if (animator != null)
            animator.SetTrigger("Roar");

        if (sr != null)
        {
            sr.color = phase2Color;
        }
        // Optional roar sound
        if (audioSource != null)
            audioSource.Play();

        // Wait for roar animation
        yield return new WaitForSeconds(2f);

        // Apply Phase 2 buffs
        flashDuration = 0.25f;
        attackCooldown = 2.5f;
        moveSpeed = 6f;
        warningDuration = 0.55f;

        // Resume fighting
        isAttacking = false;
    }

    void CheckPhase2()
    {
        if (!phase2Started && currentHealth <= maxHealth * 0.65f)
        {
            phase2Started = true;
            StartCoroutine(Phase2Transition());
        }
    }

    IEnumerator DisableHitboxAfterDelay(Collider2D col)
    {
        yield return new WaitForSeconds(0.12f); // how long the hitbox exists
        if (col != null)
            col.enabled = false;
    }

    IEnumerator FlashTelegraph()
    {
        for (int i = 0; i < flashCount; i++)
        {
            foreach (GameObject arrow in warningArrows)
            {
                if (arrow == null) continue;

                SpriteRenderer sr = arrow.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.color = flashColor;
            }

            yield return new WaitForSeconds(flashDuration / (flashCount * 2));

            foreach (GameObject arrow in warningArrows)
            {
                if (arrow == null) continue;

                SpriteRenderer sr = arrow.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.color = warningColor;
            }

            yield return new WaitForSeconds(flashDuration / (flashCount * 2));
        }
    }

    private IEnumerator FlashWhite()
    {
        if (spriteRenderer != null)
        {
            Material material = spriteRenderer.material;

            if (material.HasProperty("_FlashAmount"))
            {
                material.SetFloat("_FlashAmount", 1f);
                yield return new WaitForSeconds(0.08f);
                material.SetFloat("_FlashAmount", 0f);
            }
        }
    }

    IEnumerator ApplyKnockback(Vector2 knockbackSource)
    {
        isKnockedBack = true;

        Vector2 knockbackDirection = (rb.position - knockbackSource).normalized;
        rb.velocity = knockbackDirection * knockbackForce;

        yield return new WaitForSeconds(0.09f);

        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(enemyDamage);
        }
    }

    void DestroyAllLines()
    {
        foreach (GameObject arrow in warningArrows)
        {
            if (arrow != null)
                Destroy(arrow);
        }

        warningArrows.Clear();
    }

    void Die()
    {
        StopAllCoroutines();
        DestroyAllLines();

        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        OnEnemyDestroyed?.Invoke();

        ScoreManager.Instance.AddScore(scoreDrop);
        ScoreManager.Instance.OnEnemyKilled();

        StartCoroutine(DeathEffect());
    }

    IEnumerator DeathEffect()
    {
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        isKnockedBack = true;

        if (animator != null)
            animator.SetTrigger("Death");

        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(1f);

        if (expOrbPrefab != null)
            Instantiate(expOrbPrefab, transform.position, Quaternion.identity);

        if (bloodEffectPrefab != null)
            Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}