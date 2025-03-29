using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;  
    public float lifetime = 2f;    
    public float damage;
    private TrailRenderer bulletTrail;

    private void Start()
    {

        Destroy(gameObject, lifetime);

        bulletTrail = GetComponent<TrailRenderer>();

        if (bulletTrail != null)
        {
            bulletTrail.enabled = false; // Disable trail initially
        }
    }

    private void Update()
    {
        // Move the bullet forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (bulletTrail != null && !bulletTrail.enabled)
        {
            bulletTrail.enabled = true; // Enable trail only when moving
        }
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
            Destroy(bulletTrail);
        }
    }
}
