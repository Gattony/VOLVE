using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    public float baseSpeed = 1f;
    public float maxSpeed = 5f;
    public float detectionRange = 5f;

    [Header("Experience Settings")]
    public int minExpAmount = 5;   // Minimum experience amount
    public int maxExpAmount = 15;  // Maximum experience amount
    private int expAmount;         // Randomized experience amount

    private Transform player;

    private void Start()
    {
        // Randomize the experience amount within the specified range
        expAmount = Random.Range(minExpAmount, maxExpAmount + 1);

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        float expDetectionMultiplier = PlayerStats.Instance.expDetectionMultipler;

        if (player != null)
        {
            // Calculate the distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            float adjustedDetectionRange = detectionRange * expDetectionMultiplier;

            // Move toward the player only if within the adjusted detection range
            if (distanceToPlayer <= adjustedDetectionRange)
            {
                // Calculate speed dynamically based on distance
                float dynamicSpeed = Mathf.Lerp(maxSpeed, baseSpeed, distanceToPlayer / adjustedDetectionRange);

                // Move the orb toward the player
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * dynamicSpeed * Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCharacter.Instance.AddExp(expAmount);
            Destroy(gameObject);
        }
    }
}
