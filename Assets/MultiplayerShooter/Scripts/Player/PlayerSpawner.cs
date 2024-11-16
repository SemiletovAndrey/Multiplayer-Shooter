using Fusion;
using UnityEngine;
using Zenject;

public class PlayerSpawner : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private WeaponPhotonManager _weaponPhotonManager;
    [SerializeField] private PlayerAliveManager _playerAliveManager;
    [SerializeField] private PlayerDataConfigMD _playerDataConfigMD;
 
    [Networked] public NetworkDictionary<PlayerRef, NetworkObject> SpawnedCharacters => default;

    public void PlayerJoined(PlayerRef player)
    {
        if (HasStateAuthority)
        {
            SpawnPlayer(player);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (HasStateAuthority)
        {
            OnPlayerLeft(player);
        }
    }

    public void SpawnPlayer(PlayerRef player)
    {
        if (!SpawnedCharacters.ContainsKey(player))
        {
            Vector3 spawnPosition = new Vector3((player.RawEncoded % Runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            NetworkObject networkPlayer = Runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            SpawnedCharacters.Add(player, networkPlayer);
            _playerAliveManager.AddAliveCharacter(player, networkPlayer);
        }
    }

    public void OnPlayerLeft(PlayerRef player)
    {
        if (SpawnedCharacters.ContainsKey(player))
        {
            NetworkObject networkObject = SpawnedCharacters[player];
            Runner.Despawn(networkObject);
            SpawnedCharacters.Remove(player);
            _playerAliveManager.AliveCharacters.Remove(player);
            _weaponPhotonManager.PlayerLeft(player);
        }
    }
}
