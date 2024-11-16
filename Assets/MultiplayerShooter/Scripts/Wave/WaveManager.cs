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

    [SerializeField] private GameOverManager _deathUI;

    private float _timeWave;
    private string _waveStatus;
    private int currentWaveIndex = 0;
    private const float DelayWindowGameOver = 5f;

    public event Action<float> OnTimeChanged;
    public event Action<string> OnStatusChanged;

    [Networked, OnChangedRender(nameof(OnTimeChangedMethod)), HideInInspector]
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

    [Networked, OnChangedRender(nameof(OnStatusChangedMethod)), HideInInspector]
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
            currentWaveIndex++;
        }
        WaveStatus = "All waves completed!";

        yield return new WaitForSeconds(DelayWindowGameOver);
        RPC_ShowGameOverUI();
    }

    private void OnTimeChangedMethod()
    {
        OnTimeChanged?.Invoke(TimeWave);
    }

    private void OnStatusChangedMethod()
    {
        OnStatusChanged?.Invoke(WaveStatus);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowGameOverUI()
    {
        _deathUI.gameObject.SetActive(true);
    }
}
