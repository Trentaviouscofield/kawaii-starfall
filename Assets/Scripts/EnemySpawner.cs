using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1.4f;
    [SerializeField] private float baseEnemySpeed = 0.9f;
    [SerializeField] private float speedIncreasePerWave = 0.25f;
    [SerializeField] private float xSpawnLimit = 8f;
    [SerializeField] private float ySpawnPosition = 6f;
    [SerializeField] private float bottomLimit = -6f;
    [SerializeField] private float waveDurationSeconds = 15f;

    private float nextSpawnTime;
    private float waveTimer;

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver || !GameManager.Instance.IsGameStarted)
        {
            return;
        }

        HandleWaveProgression();
        HandleSpawning();
    }

    private void HandleWaveProgression()
    {
        waveTimer += Time.deltaTime;
        if (waveTimer >= waveDurationSeconds)
        {
            waveTimer = 0f;
            GameManager.Instance.SetWave(GameManager.Instance.CurrentWave + 1);
        }
    }

    private void HandleSpawning()
    {
        if (enemyPrefab == null) return;
        if (Time.time < nextSpawnTime) return;

        nextSpawnTime = Time.time + spawnInterval;
        float spawnX = Random.Range(-xSpawnLimit, xSpawnLimit);
        Vector3 spawnPos = new Vector3(spawnX, ySpawnPosition, 0f);

        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        float speed = baseEnemySpeed + ((GameManager.Instance.CurrentWave - 1) * speedIncreasePerWave);
        enemy.Initialize(speed, bottomLimit);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(-xSpawnLimit, ySpawnPosition, 0f), new Vector3(xSpawnLimit, ySpawnPosition, 0f));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-xSpawnLimit, bottomLimit, 0f), new Vector3(xSpawnLimit, bottomLimit, 0f));
    }
}
