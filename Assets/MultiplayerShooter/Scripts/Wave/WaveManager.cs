using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : NetworkBehaviour
{
    [SerializeField] private List<WaveConfig> waves;
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private ItemsSpawner _itemSpawner;

    [Networked, OnChangedRender(nameof(OnTimeChangedMethod))]
    public float TimeWave
    {
        get => _timeWave;
        set
        {
            if (_timeWave != value)
            {
                _timeWave = value;
                OnTimeChanged?.Invoke(_timeWave);
            }
        }
    }

    [Networked, OnChangedRender(nameof(OnStatusChangedMethod))]
    public string WaveStatus
    {
        get { return _waveStatus; }
        set
        {
            if (_waveStatus != value)
            {
                _waveStatus = value;
                OnStatusChanged?.Invoke(_waveStatus);
            }
        }
    }

    

    public event Action<float> OnTimeChanged;
    public event Action<string> OnStatusChanged;

    private float _timeWave;
    private string _waveStatus;
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
            _enemySpawner.InitializeWave(currentWave.SpawnIntervalEnemy, currentWave.EnemyTypes);
            _itemSpawner.InitializeWave(currentWave.MinIntervalSpawn,currentWave.MaxIntervalSpawn, currentWave.ItemType);
            TimeWave = currentWave.WaveDuration;


            float timeRemaining = TimeWave;
            WaveStatus = "Attack";
            while (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                TimeWave = Mathf.Max(0, timeRemaining);
                yield return null;
            }

            _enemySpawner.StopSpawning();
            WaveStatus = "Holiday";
            TimeWave = currentWave.TimeBetweenWaves;
            timeRemaining = TimeWave;
            while (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                TimeWave = Mathf.Max(0, timeRemaining);
                yield return null;
            }
            yield return new WaitForSeconds(currentWave.TimeBetweenWaves);

            currentWaveIndex++;
        }
        WaveStatus = " ";
        Debug.Log("All waves completed!");
    }

    private void OnTimeChangedMethod()
    {
        OnTimeChanged?.Invoke(TimeWave);
    }
    private void OnStatusChangedMethod()
    {
        OnStatusChanged?.Invoke(WaveStatus);
    }

}
