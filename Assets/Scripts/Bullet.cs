using UnityEngine;
using UnityEngine.SceneManagement;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

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

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy"))
        {
            // Destroy the enemy
            Destroy(hitInfo.gameObject);

            // Destroy the bullet
            Destroy(gameObject);
        }

    }


    }
