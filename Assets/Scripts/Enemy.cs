using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Action OnEnemyDestroyed; // Event triggered when the enemy is destroyed

    public int maxHealth = 3;        // Maximum health of the enemy
    private int currentHealth;       // Current health of the enemy

    public float moveSpeed = 2f;     // Speed at which the enemy moves
    private Transform player;        // Reference to the player's transform

    private Rigidbody2D rb;          // Rigidbody2D for physics-based movement
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component

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
        // Move towards the player
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void TakeDamage(int damage)
    {
        // Reduce the enemy's health by the damage amount
        currentHealth -= damage;
        Debug.Log($"Enemy {gameObject.name} took {damage} damage. Remaining health: {currentHealth}");

        // Trigger the damage flash effect
        StartCoroutine(HitEffect());

        // Check if health is depleted
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitEffect()
    {
        if (spriteRenderer != null)
        {
            // Save the original color
            Color originalColor = spriteRenderer.color;

            //Hit effects
            spriteRenderer.color = Color.white;

            yield return new WaitForSeconds(0.07f);

            // Revert to the original color
            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        OnEnemyDestroyed?.Invoke();

        PlayDestructionEffects();

        Destroy(gameObject);
    }

    private void PlayDestructionEffects()
    {
        // Placeholder for particle effects, sound effects, etc.
        Debug.Log($"Enemy {gameObject.name} destroyed!");
    }
}
