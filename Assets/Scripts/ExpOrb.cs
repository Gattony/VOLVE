using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    public float baseSpeed = 1f;      
    public float maxSpeed = 5f;      
    public float detectionRange = 5f; // Distance within which the orb moves toward the player
    public int expAmount = 10;       

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Move toward the player only if within detection range
            if (distanceToPlayer <= detectionRange)
            {
                // Interpolate speed based on distance, dynamic based exp orb movement
                float dynamicSpeed = Mathf.Lerp(maxSpeed, baseSpeed, distanceToPlayer / detectionRange);

                Vector3 direction = (player.position - transform.position).normalized;

                transform.position += direction * dynamicSpeed * Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Adding EXP to the character
            PlayerCharacter.Instance.AddExp(expAmount);

            Destroy(gameObject);
        }
    }
}
