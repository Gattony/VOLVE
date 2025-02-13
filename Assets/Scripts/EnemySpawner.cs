using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnRadius = 15f;
    public float maxEnemyDistance = 30f; // Maximum distance before relocating

    [Header("Spawn Timing")]
    public float initialSpawnRate = 2f;
    public float minSpawnRate = 0.5f;
    public float spawnRateDecay = 0.01f;
    private float currentSpawnRate;

    [Header("Enemy Limits")]
    public int initialMaxEnemies = 25;
    public int maxMaxEnemies = 200;
    public int maxEnemiesIncrease = 10;
    public float difficultyIncreaseInterval = 10f;
    private int currentMaxEnemies;

    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        currentSpawnRate = initialSpawnRate;
        currentMaxEnemies = initialMaxEnemies;

        StartCoroutine(SpawnEnemies());
        StartCoroutine(IncreaseDifficulty());
        StartCoroutine(RelocateFarEnemies());
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
            currentSpawnRate = Mathf.Max(minSpawnRate, currentSpawnRate - spawnRateDecay);
            if (currentMaxEnemies < maxMaxEnemies)
            {
                currentMaxEnemies += maxEnemiesIncrease;
            }
            Debug.Log($"Difficulty Increased: SpawnRate = {currentSpawnRate}, MaxEnemies = {currentMaxEnemies}");
        }
    }

    IEnumerator RelocateFarEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f); // Check every 2 seconds

            foreach (GameObject enemy in activeEnemies)
            {
                if (enemy == null) continue;

                float distance = Vector2.Distance(enemy.transform.position, player.position);
                if (distance > maxEnemyDistance)
                {
                    RelocateEnemy(enemy);
                }
            }
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnDirection = Random.insideUnitCircle.normalized;
        Vector2 spawnPosition = (Vector2)player.position + spawnDirection * spawnRadius;

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy);

        newEnemy.GetComponent<Enemy>().OnEnemyDestroyed += () =>
        {
            activeEnemies.Remove(newEnemy);
        };
    }

    void RelocateEnemy(GameObject enemy)
    {
        Vector2 spawnDirection = Random.insideUnitCircle.normalized;
        Vector2 spawnPosition = (Vector2)player.position + spawnDirection * spawnRadius;
        enemy.transform.position = spawnPosition;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (player != null)
        {
            Gizmos.DrawWireSphere(player.position, spawnRadius);
        }
    }
}
