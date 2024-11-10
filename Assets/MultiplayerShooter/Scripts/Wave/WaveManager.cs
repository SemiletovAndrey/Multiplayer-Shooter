using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : NetworkBehaviour
{
    [SerializeField] private List<WaveConfig> waves;
    [SerializeField] private EnemySpawner _enemySpawner;

    private int currentWaveIndex = 0;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            StartCoroutine(StartWaves());
        }
    }


    private IEnumerator StartWaves()
    {
        while (currentWaveIndex < waves.Count)
        {
            WaveConfig currentWave = waves[currentWaveIndex];
            _enemySpawner.InitializeWave(currentWave.SpawnInterval, currentWave.EnemyTypes);

            yield return new WaitForSeconds(currentWave.WaveDuration);

            _enemySpawner.StopSpawning();
            yield return new WaitForSeconds(currentWave.TimeBetweenWaves);

            currentWaveIndex++;
        }

        Debug.Log("All waves completed!");
    }
}
