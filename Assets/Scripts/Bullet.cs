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
            bulletTrail.enabled = false;
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
        // Check if object implements IDamageable
        IDamageable damageable = hitInfo.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            // Apply damage
            damageable.TakeDamage((int)damage, transform.position);

            // Spawn damage number
            if (damageNumber != null)
            {
                GameObject damageText = Instantiate(damageNumber, transform.position, Quaternion.identity);
                DamageNumber dmgNumber = damageText.GetComponent<DamageNumber>();

                if (dmgNumber != null)
                {
                    dmgNumber.Initialize(damage);
                }
            }

            // Spawn impact effect
            if (impactEffectPrefab != null)
            {
                GameObject impactEffect = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
                Destroy(impactEffect, 0.18f);
            }

            Destroy(gameObject);
        }
    }
}