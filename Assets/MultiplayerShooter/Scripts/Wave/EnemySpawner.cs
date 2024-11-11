using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;
public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float minimalSpawnRadius;

    private float _spawnInterval;
    private List<GameObject> _enemyTypes;
    private Coroutine _spawnCoroutine;

    private List<Enemy> _enemies = new List<Enemy>();

    private Dictionary<PlayerRef, NetworkObject> _playerObjects;


    public void Initialize(Dictionary<PlayerRef, NetworkObject> playerObjects)
    {
        _playerObjects = playerObjects;
    }

    public void InitializeWave(float spawnInterval, List<GameObject> enemyTypes)
    {
        _spawnInterval = spawnInterval;
        _enemyTypes = enemyTypes;

        Debug.Log($"Initializing wave with interval: {_spawnInterval} and enemy types count: {_enemyTypes?.Count}");

        if (Object.HasStateAuthority && _spawnCoroutine == null)
        {
            _spawnCoroutine = StartCoroutine(SpawnEnemies());
        }
    }

    public void StopSpawning()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnInterval);
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {

        var playerTransform = GetRandomPlayerTransform();
        Vector2 spawnPosition = (Vector2)playerTransform.position + Random.insideUnitCircle * spawnRadius;

        int randomEnemyIndex = Random.Range(0, _enemyTypes.Count);
        GameObject enemyPrefab = _enemyTypes[randomEnemyIndex];

        if (Runner.IsServer)
        {
            NetworkObject enemyObject = Runner.Spawn(enemyPrefab, spawnPosition, Quaternion.identity);
            Enemy enemy = enemyObject.GetComponent<Enemy>();

            _enemies.Add(enemy);

        }
    }

    private Transform GetRandomPlayerTransform()
    {
        var player = _playerObjects.Values.ElementAt(Random.Range(0, _playerObjects.Count));
        return player.transform;
    }
}
