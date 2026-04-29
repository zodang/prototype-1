using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int spawnCount = 5;
    [SerializeField] private float spawnRadius = 6f;
    [SerializeField] private bool spawnOnStart = true;

    private Coroutine _spawnCoroutine;

    private void OnValidate()
    {
        spawnInterval = Mathf.Max(0.01f, spawnInterval);
        spawnCount = Mathf.Max(1, spawnCount);
        spawnRadius = Mathf.Max(0f, spawnRadius);
    }

    private void Start()
    {
        if (player == null)
        {
            PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
            if (playerMovement != null)
            {
                player = playerMovement.transform;
            }
        }

        if (spawnOnStart)
        {
            SpawnEnemiesAroundPlayer();
        }

        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        if (_spawnCoroutine == null) return;

        StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = null;
    }

    private IEnumerator SpawnRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        while (true)
        {
            yield return wait;
            SpawnEnemiesAroundPlayer();
        }
    }

    private void SpawnEnemiesAroundPlayer()
    {
        if (enemyPrefab == null || player == null) return;

        float angleStep = 360f / spawnCount;
        float startAngle = Random.Range(0f, 360f);

        for (int i = 0; i < spawnCount; i++)
        {
            float angle = (startAngle + angleStep * i) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector3 spawnPosition = player.position + (Vector3)(direction * spawnRadius);

            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
