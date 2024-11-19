using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;
public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private float _spawnRadius = 10f;
    [SerializeField] private float _minimalSpawnRadius;
    [SerializeField] private PlayerAliveManager _playerManagerAlive;

    private float _spawnInterval;
    private List<Enemy> _enemyTypes;
    private Coroutine _spawnCoroutine;

    private List<NetworkObject> _enemies = new List<NetworkObject>();


    public void InitializeWave(float spawnInterval, List<Enemy> enemyTypes)
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
        if (playerTransform != null)
        {
            Vector2 spawnPosition = GetRandomPositionAroundPlayer(playerTransform.position, _minimalSpawnRadius, _spawnRadius);
            //Vector2 spawnPosition = (Vector2)playerTransform.position + new Vector2(_minimalSpawnRadius, _minimalSpawnRadius) + Random.insideUnitCircle * _spawnRadius;

            int randomEnemyIndex = Random.Range(0, _enemyTypes.Count);
            GameObject enemyPrefab = _enemyTypes[randomEnemyIndex].gameObject;

            if (Runner.IsServer)
            {
                NetworkObject enemyObject = Runner.Spawn(enemyPrefab, spawnPosition, Quaternion.identity);
                Enemy enemy = enemyObject.GetComponent<Enemy>();

                _enemies.Add(enemyObject);

            }
        }
    }

    public void DeleteAllEnemies()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            Runner.Despawn(_enemies[i]);
            _enemies.RemoveAt(i);
        }
    }

    private Vector2 GetRandomPositionAroundPlayer(Vector2 center, float minRadius, float maxRadius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2); // Случайный угол в радианах
        float radius = Random.Range(minRadius, maxRadius); // Случайное расстояние в пределах заданного диапазона

        // Вычисляем смещение по оси X и Y
        float offsetX = Mathf.Cos(angle) * radius;
        float offsetY = Mathf.Sin(angle) * radius;

        return center + new Vector2(offsetX, offsetY);
    }

    private Transform GetRandomPlayerTransform()
    {
        if (_playerManagerAlive.AliveCharacters.Count == 0) return null;

        var randomPlayer = _playerManagerAlive.AliveCharacters.ElementAt(Random.Range(0, _playerManagerAlive.AliveCharacters.Count)).Value;
        return randomPlayer.transform;
    }
}
