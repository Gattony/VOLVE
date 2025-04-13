using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Action OnEnemyDestroyed; // Event triggered when the enemy is destroyed

    public int maxHealth = 10;
    private int currentHealth;
    public int enemyDamage = 1;
    public int scoreDrop = 10;

    public float moveSpeed = 2f;
    private Transform player;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float knockbackForce = 10f;
    private bool isKnockedBack = false;

    public GameObject expOrbPrefab;
    public GameObject bloodEffectPrefab;

    void Start()
    {

        currentHealth = maxHealth;

        // Get components BEFORE modifying animator
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found! Make sure the enemy has a SpriteRenderer component.");
        }

        if (animator == null)
        {
            Debug.LogError("Animator not found! Make sure the enemy has an Animator component.");
        }
    }

    void FixedUpdate()
    {
        if (player != null && !isKnockedBack)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;

            // Flip sprite based on movement direction
            if (direction.x > 0)
                spriteRenderer.flipX = false; 
            else if (direction.x < 0)
                spriteRenderer.flipX = true;
        }
    }
    public void TakeDamage(int damage, Vector2 knockbackSource)
    {
        currentHealth -= damage;
        StartCoroutine(FlashWhite());
        StartCoroutine(ApplyKnockback(knockbackSource));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashWhite()
    {
        if (spriteRenderer != null)
        {
            Material material = spriteRenderer.material; 

            if (material.HasProperty("_FlashAmount")) // Check if shader supports flash
            {
                material.SetFloat("_FlashAmount", 1f);
                yield return new WaitForSeconds(0.08f);
                material.SetFloat("_FlashAmount", 0f); // Reset flash
            }
        }
    }

    private IEnumerator ApplyKnockback(Vector2 knockbackSource)
    {
        isKnockedBack = true;
        Vector2 knockbackDirection = (rb.position - knockbackSource).normalized;
        rb.velocity = knockbackDirection * knockbackForce;

        yield return new WaitForSeconds(0.09f);

        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(enemyDamage);
            Die();
        }
    }

    private void Die()
    {
        OnEnemyDestroyed?.Invoke();

        //Scoring Logic
        ScoreManager.Instance.AddScore(scoreDrop);
        ScoreManager.Instance.OnEnemyKilled();

        StartCoroutine(DeathEffect());
    }

    private IEnumerator DeathEffect()
    {
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        isKnockedBack = true; // Prevent movement logic

        // Quick pop effect
        float timer = 0f;
        Vector3 originalScale = transform.localScale;
        while (timer < 0.06f)
        {
            timer += Time.deltaTime;
            transform.localScale = originalScale * (1f + timer * 2f);
            yield return null;
        }

        if (animator != null)
        {
            animator.SetTrigger("Death");

            // Wait until the Death animation starts playing
            yield return new WaitUntil(() =>
                animator.GetCurrentAnimatorStateInfo(0).IsName("Death") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("DeathZ")
            );

            // Then wait for its actual duration
            float animDuration = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animDuration);

        }

        else
        {
            // Fallback wait if there's no animator
            yield return new WaitForSeconds(0.5f);
        }

        if (expOrbPrefab != null)
            Instantiate(expOrbPrefab, transform.position, Quaternion.identity);

        if (bloodEffectPrefab != null)
            Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

}