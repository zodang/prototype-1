using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform player;

    [Header("Spawn Range")]
    [SerializeField] private float minRadius = 3f;
    [SerializeField] private float maxRadius = 7f;

    [Header("Spawn Time")]
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 3f;

    private float _timer;
    private float _nextSpawnTime;

    private void Start()
    {
        SetNextSpawnTime();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _nextSpawnTime)
        {
            SpawnEnemy();
            _timer = 0f;
            SetNextSpawnTime();
        }
    }

    private void SetNextSpawnTime()
    {
        _nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }

    private void SpawnEnemy()
    {
        Vector2 spawnPos = GetRandomPositionAroundPlayer();
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    private Vector2 GetRandomPositionAroundPlayer()
    {
        float angle = Random.Range(0f, 360f);
        float distance = Random.Range(minRadius, maxRadius);

        Vector2 dir = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        return (Vector2)player.position + dir * distance;
    }
}