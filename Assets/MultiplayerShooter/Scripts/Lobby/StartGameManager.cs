using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameManager : NetworkBehaviour
{
    [SerializeField] private GameObject _hostStartMenuUI;
    [SerializeField] private GameObject _clientStartMenuUI;

    [SerializeField] private WaveManager _waveManager;
    [SerializeField] private ItemsSpawner _itemsSpawner;
    [SerializeField] private EnemySpawner _enemySpawner;

    public override void Spawned()
    {
        base.Spawned();
        if (Runner.IsClient)
        {
            _clientStartMenuUI.SetActive(true);
            _hostStartMenuUI.SetActive(false);
        }
        else
        {
            _clientStartMenuUI.SetActive(false);
            _hostStartMenuUI.SetActive(true);
        }
    }

    public void StartGame()
    {
        if (Runner.IsServer)
        {
            RPC_DisableUI();
        }
        
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_DisableUI()
    {
        _hostStartMenuUI.SetActive(false);
        _clientStartMenuUI.SetActive(false);

        _itemsSpawner.gameObject.SetActive(true);
        _enemySpawner.gameObject.SetActive(true);
        _waveManager.gameObject.SetActive(true);
    }
}
