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

    private void OnEnable()
    {
        PlayerData.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        PlayerData.OnPlayerDeath -= HandlePlayerDeath;
    }

    public void StopSpawning()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private void HandlePlayerDeath(PlayerRef playerRef)
    {
        if (_playerObjects.ContainsKey(playerRef))
        {
            _playerObjects.Remove(playerRef);

            UpdateEnemyTargets();
        }
    }

    private void UpdateEnemyTargets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            enemy.UpdateTarget(GetRandomPlayerTransform());
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
        if (_playerObjects == null || _playerObjects.Count == 0) return;

        var playerTransform = GetRandomPlayerTransform();
        Vector2 spawnPosition = (Vector2)playerTransform.position + Random.insideUnitCircle * spawnRadius;

        int randomEnemyIndex = Random.Range(0, _enemyTypes.Count);
        GameObject enemyPrefab = _enemyTypes[randomEnemyIndex];

        if (Runner.IsServer)
        {
            NetworkObject enemyObject = Runner.Spawn(enemyPrefab, spawnPosition, Quaternion.identity);
            enemyObject.GetComponent<Enemy>().Initialize(playerTransform);
        }
    }

    private Transform GetRandomPlayerTransform()
    {
        var player = _playerObjects.Values.ElementAt(Random.Range(0, _playerObjects.Count));
        return player.transform;
    }
}
