using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;  // Enemy prefab to spawn
    public Transform player;        // Reference to the player
    public float spawnRadius = 15f; // Minimum distance from the player

    [Header("Spawn Timing")]
    public float initialSpawnRate = 2f;  // Initial spawn rate (time between spawns)
    public float minSpawnRate = 0.5f;    // Minimum spawn rate (time between spawns)
    public float spawnRateDecay = 0.01f; // How much spawn rate decreases per interval
    private float currentSpawnRate;

    [Header("Enemy Limits")]
    public int initialMaxEnemies = 25;   // Initial limit for active enemies
    public int maxMaxEnemies = 200;      // Maximum limit for enemies over time
    public int maxEnemiesIncrease = 1;   // Increase in max enemies per interval
    public float difficultyIncreaseInterval = 10f; // Time interval to increase difficulty
    private int currentMaxEnemies;

    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        // Initialize spawn rate and max enemies
        currentSpawnRate = initialSpawnRate;
        currentMaxEnemies = initialMaxEnemies;

        // Start spawning enemies and difficulty progression
        StartCoroutine(SpawnEnemies());
        StartCoroutine(IncreaseDifficulty());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (activeEnemies.Count < currentMaxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(currentSpawnRate);
        }
    }

    IEnumerator IncreaseDifficulty()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyIncreaseInterval);

            // Decrease spawn rate gradually but not below the minimum
            currentSpawnRate = Mathf.Max(minSpawnRate, currentSpawnRate - spawnRateDecay);

            // Increase the maximum number of enemies gradually
            if (currentMaxEnemies < maxMaxEnemies)
            {
                currentMaxEnemies += maxEnemiesIncrease;
            }

            Debug.Log($"Difficulty Increased: SpawnRate = {currentSpawnRate}, MaxEnemies = {currentMaxEnemies}");
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
        if (player != null)
        {
            Gizmos.DrawWireSphere(player.position, spawnRadius);
        }
    }
}

