using TMPro;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public float damage;
    private TrailRenderer bulletTrail;

    public GameObject damageNumber;
    public GameObject impactEffectPrefab;

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
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (bulletTrail != null && !bulletTrail.enabled)
        {
            bulletTrail.enabled = true;
        }
    }

    public void Initialize(float damageValue)
    {
        damage = damageValue;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy"))
        {
            Enemy enemy = hitInfo.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)damage, transform.position);
            }

            // Play the impact effect
            if (impactEffectPrefab != null)
            {
                GameObject impactEffect = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
                Destroy(impactEffect, 0.18f); // Destroy effect after a short duration
            }

            // Destroy the bullet and its trail
            Destroy(gameObject);
            if (bulletTrail != null)
            {
                Destroy(bulletTrail.gameObject);
            }

            if (damageNumber != null)
            {
                GameObject damageText = Instantiate(damageNumber, transform.position, Quaternion.identity);
                damageText.GetComponent<TextMeshPro>().text = damage.ToString(); // Set damage text
            }
        }
    }
}
