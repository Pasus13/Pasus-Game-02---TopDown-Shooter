using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;          // Player to spawn around
    [SerializeField] private GameObject enemyPrefab;    // Enemy prefab to spawn
    [SerializeField] private BoxCollider2D roomBounds;  // Same bounds used by the camera

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 3f;      // Time between spawns
    [SerializeField] private float spawnDistance = 8f;      // Fixed distance from player
    [SerializeField] private int maxEnemiesAlive = 20;      // Limit total enemies

    [Header("Collision Check (optional)")]
    [SerializeField] private LayerMask obstacleMask;        // Walls/obstacles to avoid
    [SerializeField] private float spawnCheckRadius = 0.5f; // Radius to check for obstacles
    [SerializeField] private int maxSpawnAttempts = 10;     // To avoid infinite loops

    private float _timer;

    // Tracks alive enemies spawned by this spawner
    private readonly List<EnemyHealth> _aliveEnemies = new();

    private void Update()
    {
        if (player == null || enemyPrefab == null || roomBounds == null)
            return;

        _timer += Time.deltaTime;

        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            TrySpawnEnemy();
        }
    }

    private void TrySpawnEnemy()
    {
        // Clean up null entries (enemies destroyed)
        CleanupDeadReferences();

        // Limit number of enemies
        if (_aliveEnemies.Count >= maxEnemiesAlive)
            return;

        // Try multiple random positions around the player
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 spawnPos = GetRandomPositionAroundPlayer();

            // 1) Check if spawnPos is inside the room bounds
            if (!roomBounds.bounds.Contains(spawnPos))
                continue;

            // 2) Optional: check for obstacles at that position
            if (Physics2D.OverlapCircle(spawnPos, spawnCheckRadius, obstacleMask) != null)
                continue;

            // All checks passed → spawn enemy
            GameObject enemyGO = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            // Subscribe to EnemyHealth death event
            EnemyHealth health = enemyGO.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.OnEnemyDied += HandleEnemyDied;
                _aliveEnemies.Add(health);
            }
            else
            {
                Debug.LogWarning("[EnemySpawner] Spawned enemy has no EnemyHealth component in children.");
            }

            return;
        }

        // If we reach here, no valid position was found
        Debug.LogWarning("[EnemySpawner] Could not find valid spawn position.");
    }

    private Vector2 GetRandomPositionAroundPlayer()
    {
        // Random angle in radians
        float angle = Random.Range(0f, Mathf.PI * 2f);

        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 spawnPos = (Vector2)player.position + dir * spawnDistance;

        return spawnPos;
    }

    private void HandleEnemyDied(EnemyHealth deadEnemy)
    {
        // Unsubscribe to avoid dangling references
        if (deadEnemy != null)
            deadEnemy.OnEnemyDied -= HandleEnemyDied;

        // Remove from alive list
        _aliveEnemies.Remove(deadEnemy);
    }

    private void CleanupDeadReferences()
    {
        // Remove nulls (destroyed enemies)
        for (int i = _aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (_aliveEnemies[i] == null)
                _aliveEnemies.RemoveAt(i);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from all alive enemies
        for (int i = 0; i < _aliveEnemies.Count; i++)
        {
            if (_aliveEnemies[i] != null)
                _aliveEnemies[i].OnEnemyDied -= HandleEnemyDied;
        }

        _aliveEnemies.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null)
            return;

        Gizmos.color = Color.red;
        // Draw a circle (approx) of spawn distance around player
        const int segments = 32;
        float step = Mathf.PI * 2f / segments;
        Vector3 prevPoint = player.position + new Vector3(spawnDistance, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * step;
            Vector3 nextPoint = player.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spawnDistance;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}

