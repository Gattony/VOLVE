using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;        // Speed of the bullet
    public float lifetime = 2f;     // Time before the bullet is destroyed
    public float damage;           // Damage value passed from the weapon script

    private void Start()
    {
        // Destroy the bullet after a set time
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the bullet forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    // Method to set the damage value when the bullet is instantiated
    public void Initialize(float damageValue)
    {
        damage = damageValue;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy"))
        {
            // Apply damage to the enemy
            Enemy enemy = hitInfo.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)damage, transform.position);
            }

            // Destroy the bullet upon collision with the enemy
            Destroy(gameObject);
        }
    }
}
