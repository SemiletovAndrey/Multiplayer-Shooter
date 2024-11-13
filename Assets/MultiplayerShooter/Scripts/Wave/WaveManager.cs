using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : NetworkBehaviour
{
    [SerializeField] private List<WaveConfig> waves;
    [SerializeField] private EnemySpawner _enemySpawner;

    [Networked, OnChangedRender(nameof(OnTimeChangedMethod))]
    public float TimeWave
    {
        get => _timeWave;
        set
        {
            if (_timeWave != value)
            {
                _timeWave = value;
                OnTimeChanged?.Invoke(_waveStatus, _timeWave);
            }
        }
    }

    [Networked]
    public string WaveStatus
    {
        get { return _waveStatus; }
        set { _waveStatus = value; }
    }

    public event Action<string, float> OnTimeChanged;

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
            _enemySpawner.InitializeWave(currentWave.SpawnInterval, currentWave.EnemyTypes);
            TimeWave = currentWave.WaveDuration;


            float timeRemaining = TimeWave;
            _waveStatus = "Attack";
            while (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                TimeWave = Mathf.Max(0, timeRemaining);
                yield return null;
            }

            _enemySpawner.StopSpawning();
            _waveStatus = "Holiday";
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
        Debug.Log("All waves completed!");
    }

    private void OnTimeChangedMethod()
    {
        OnTimeChanged?.Invoke(_waveStatus, TimeWave);
    }
}
