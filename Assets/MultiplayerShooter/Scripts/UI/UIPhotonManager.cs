using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UIPhotonManager : NetworkBehaviour
{
    [SerializeField] private GameObject _playerControllerUI;
    [SerializeField] private PlayerInfoUI _playerInfoUI;
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private BasicSpawner _spawner;


    public override void Spawned()
    {
        StartCoroutine(CheckPlayerSpawned());
    }

    private IEnumerator CheckPlayerSpawned()
    {
        while (!_playerSpawner.SpawnedCharacters.ContainsKey(Runner.LocalPlayer))
        {
            yield return new WaitForSeconds(0.1f);
        }

        SpawnedContext();
    }

    public void SpawnedContext()
    {
        InitializeLocalPlayerUI(Runner.LocalPlayer);
    }
    private void InitializeLocalPlayerUI(PlayerRef player)
    {
        if (_playerSpawner.SpawnedCharacters.ContainsKey(player))
        {
            NetworkObject localPlayer = _playerSpawner.SpawnedCharacters[player];
            PlayerModel playerModel = localPlayer.gameObject.GetComponent<PlayerModel>();

            _playerInfoUI.InitializePlayerUI(playerModel);
            _playerControllerUI.SetActive(true);
            _playerInfoUI.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Player not found in _spawnedCharacters");
        }
    }


}
