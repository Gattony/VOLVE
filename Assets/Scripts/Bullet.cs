using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;        // Speed of the bullet
    public float lifetime = 2f;     // Time before the bullet is destroyed
    public int baseDamage = 1;      // Base damage of the bullet
    public float damage;           // Actual damage after applying multipliers

    private void Start()
    {
        // Calculate damage based on player's damage multiplier
        damage = baseDamage * PlayerCharacter.Instance.damageMultiplier;

        // Destroy the bullet after a set time
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the bullet forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy"))
        {
            // Get the Enemy component and apply damage
            Enemy enemy = hitInfo.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)damage, transform.position);
            }

            // Destroy the bullet after hitting the enemy
            Destroy(gameObject);
        }
    }
}
