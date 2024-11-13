using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;
public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private float _spawnRadius = 10f;
    [SerializeField] private float _minimalSpawnRadius;
    [SerializeField] private PlayerSpawner _playerSpawner;

    private float _spawnInterval;
    private List<GameObject> _enemyTypes;
    private Coroutine _spawnCoroutine;

    private List<Enemy> _enemies = new List<Enemy>();


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
        Vector2 spawnPosition = (Vector2)playerTransform.position + Random.insideUnitCircle * _spawnRadius;

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
        var randomPlayer = _playerSpawner._spawnedCharacters.ElementAt(Random.Range(0, _playerSpawner._spawnedCharacters.Count)).Value;
        return randomPlayer.transform;
    }
}
