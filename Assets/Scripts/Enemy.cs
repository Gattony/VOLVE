using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Action OnEnemyDestroyed; // Event triggered when the enemy is destroyed

    public int maxHealth = 10;        // Maximum health of the enemy
    private int currentHealth;       // Current health of the enemy
    public int enemyDamage = 1;

    public float moveSpeed = 2f;     // Speed at which the enemy moves
    private Transform player;        // Reference to the player's transform

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float knockbackForce = 10f; // Strength of the knockback effect
    private bool isKnockedBack = false; 

    public GameObject expOrbPrefab; 

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Find the player in the scene
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Get the Rigidbody2D and SpriteRenderer components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found! Make sure the enemy has a SpriteRenderer component.");
        }
    }

    void FixedUpdate()
    {
        // Move towards the player only if not being knocked back
        if (player != null && !isKnockedBack)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackSource)
    {
        // Reduce the enemy's health by the damage amount
        currentHealth -= damage;

        // Trigger the damage flash effect
        StartCoroutine(FlashWhite());

        // Apply knockback
        StartCoroutine(ApplyKnockback(knockbackSource));

        // Check if health is depleted
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashWhite()
    {
        if (spriteRenderer != null)
        {
            // Save the original color
            Color originalColor = spriteRenderer.color;

            spriteRenderer.color = Color.white;

            yield return new WaitForSeconds(0.07f);

            // Revert to the original color
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator ApplyKnockback(Vector2 knockbackSource)
    {
        isKnockedBack = true;

        // Calculate the knockback direction
        Vector2 knockbackDirection = (rb.position - knockbackSource).normalized; 

        rb.velocity = knockbackDirection * knockbackForce;

        yield return new WaitForSeconds(0.07f);

        // Stop the knockback
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(enemyDamage);
        }
    }

    private void Die()
    {

        OnEnemyDestroyed?.Invoke();

        // Play destruction effects, particles, sound, etc.
        PlayDestructionEffects();

        // Spawn EXP orb
        Instantiate(expOrbPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void PlayDestructionEffects()
    {
        // Placeholder for particle effects, sound effects, etc.
        
    }
}
