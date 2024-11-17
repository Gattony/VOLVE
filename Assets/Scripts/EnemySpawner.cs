using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Enemy prefab to spawn
    public Transform player;       // Reference to the player
    public float spawnRadius = 15f; // Minimum distance from the player
    public float spawnRate = 2f;    // Enemies spawn every 2 seconds
    private int maxEnemies = 150;     // Limit for active enemies

    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        // Start spawning enemies at regular intervals
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Spawn enemies at intervals ( Reduce Lag )
            if (activeEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    void SpawnEnemy()
    {
        // Randomize spawn position around the player
        Vector2 spawnDirection = Random.insideUnitCircle.normalized;
        Vector2 spawnPosition = (Vector2)player.position + spawnDirection * spawnRadius;

        // Instantiate the enemy and add it to the active list
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy);

        // Remove the enemy from the list when it is destroyed
        newEnemy.GetComponent<Enemy>().OnEnemyDestroyed += () =>
        {
            activeEnemies.Remove(newEnemy);
        };
    }

    void OnDrawGizmosSelected()
    {
        // Draw spawn radius in the Scene view for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, spawnRadius);
    }
}
