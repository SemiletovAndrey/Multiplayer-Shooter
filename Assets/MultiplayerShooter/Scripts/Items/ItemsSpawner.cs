using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsSpawner : NetworkBehaviour
{
    [SerializeField] private float _spawnRadius = 10f;
    [SerializeField] private float _minimalSpawnRadius;
    [SerializeField] private PlayerAliveManager _playerAliveManager;

    private float _spawnIntervalMin;
    private float _spawnIntervalMax;
    private List<Item> _itemsType;
    private Coroutine _spawnCoroutine;

    private List<Item> _items = new List<Item>();


    public void InitializeWave(float spawnIntervalMin,float spawnIntervalMax, List<Item> itemsType)
    {
        _spawnIntervalMin = spawnIntervalMin;
        _spawnIntervalMax = spawnIntervalMax;
        _itemsType = itemsType;


        if (Object.HasStateAuthority && _spawnCoroutine == null)
        {
            _spawnCoroutine = StartCoroutine(Spawntems());
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

    private IEnumerator Spawntems()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_spawnIntervalMin, _spawnIntervalMax));
            SpawnItem();
        }
    }

    private void SpawnItem()
    {
        var playerTransform = GetRandomPlayerTransform();
        if (playerTransform != null)
        {
            Vector2 spawnPosition = GetRandomPositionAroundPlayer(playerTransform.position, _minimalSpawnRadius, _spawnRadius);
        //Vector2 spawnPosition = (Vector2)playerTransform.position + new Vector2(_minimalSpawnRadius, _minimalSpawnRadius) + Random.insideUnitCircle * _spawnRadius;

        int randomEnemyIndex = Random.Range(0, _itemsType.Count);
        GameObject enemyPrefab = _itemsType[randomEnemyIndex].gameObject;

            if (Runner.IsServer)
            {
                NetworkObject enemyObject = Runner.Spawn(enemyPrefab, spawnPosition, Quaternion.identity);
                Item item = enemyObject.GetComponent<Item>();

                _items.Add(item);

            }
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
        if (_playerAliveManager.AliveCharacters.Count == 0) return null;

        var randomPlayer = _playerAliveManager.AliveCharacters.ElementAt(Random.Range(0, _playerAliveManager.AliveCharacters.Count)).Value;
        return randomPlayer.transform;
    }
}
