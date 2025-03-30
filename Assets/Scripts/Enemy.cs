using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Action OnEnemyDestroyed; // Event triggered when the enemy is destroyed

    public int maxHealth = 10;
    private int currentHealth;
    public int enemyDamage = 1;

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
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.08f);
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator ApplyKnockback(Vector2 knockbackSource)
    {
        isKnockedBack = true;
        Vector2 knockbackDirection = (rb.position - knockbackSource).normalized;
        rb.velocity = knockbackDirection * knockbackForce;

        yield return new WaitForSeconds(0.08f);

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
        StartCoroutine(DeathEffect());
    }

    private IEnumerator DeathEffect()
    {
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        isKnockedBack = true; // Prevent movement logic

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

            yield return null;

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Death"));

            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }

        else
        {
            yield return new WaitForSeconds(0.5f); // Fallback if no animator
        }

        Instantiate(expOrbPrefab, transform.position, Quaternion.identity);

        if (bloodEffectPrefab != null)
        {
            Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

}