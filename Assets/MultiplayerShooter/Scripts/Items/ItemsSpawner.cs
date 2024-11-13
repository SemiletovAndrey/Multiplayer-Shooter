using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsSpawner : NetworkBehaviour
{
    [SerializeField] private float _spawnRadius = 10f;
    [SerializeField] private float _minimalSpawnRadius;
    [SerializeField] private PlayerSpawner _playerSpawner;

    private float _spawnIntervalMin;
    private float _spawnIntervalMax;
    private List<GameObject> _itemsType;
    private Coroutine _spawnCoroutine;

    private List<Item> _items = new List<Item>();


    public void InitializeWave(float spawnIntervalMin,float spawnIntervalMax, List<GameObject> itemsType)
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
        Vector2 spawnPosition = (Vector2)playerTransform.position + new Vector2(_minimalSpawnRadius, _minimalSpawnRadius) + Random.insideUnitCircle * _spawnRadius;

        int randomEnemyIndex = Random.Range(0, _itemsType.Count);
        GameObject enemyPrefab = _itemsType[randomEnemyIndex];

        if (Runner.IsServer)
        {
            NetworkObject enemyObject = Runner.Spawn(enemyPrefab, spawnPosition, Quaternion.identity);
            Item item = enemyObject.GetComponent<Item>();

            _items.Add(item);

        }
    }

    private Transform GetRandomPlayerTransform()
    {
        var randomPlayer = _playerSpawner.SpawnedCharacters.ElementAt(Random.Range(0, _playerSpawner.SpawnedCharacters.Count)).Value;
        return randomPlayer.transform;
    }
}
