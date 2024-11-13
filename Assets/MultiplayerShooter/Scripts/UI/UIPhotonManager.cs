using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPhotonManager : NetworkBehaviour
{
    [SerializeField] private GameObject _playerControllerUI;
    [SerializeField] private PlayerInfoUI _playerInfoUI;
    [SerializeField] private PlayerSpawner _playerSpawner;

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

    [ContextMenu("Spawned check")]
    public void SpawnedContext()
    {
        InitializeLocalPlayerUI(Runner.LocalPlayer);
    }
    private void InitializeLocalPlayerUI(PlayerRef player)
    {
        if (_playerSpawner.SpawnedCharacters.ContainsKey(player))
        {
            NetworkObject localPlayer = _playerSpawner.SpawnedCharacters[player];
            PlayerData playerData = localPlayer.gameObject.GetComponent<PlayerData>();

            _playerInfoUI.InitializePlayerUI(playerData);
            _playerControllerUI.SetActive(true);
            _playerInfoUI.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Player not found in _spawnedCharacters");
        }
    }
}
