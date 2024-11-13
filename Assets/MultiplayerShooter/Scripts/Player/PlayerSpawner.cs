using Fusion;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
 
    [Networked] public NetworkDictionary<PlayerRef, NetworkObject> _spawnedCharacters => default;

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
        if (!_spawnedCharacters.ContainsKey(player))
        {
            Vector3 spawnPosition = new Vector3((player.RawEncoded % Runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            NetworkObject networkPlayer = Runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            _spawnedCharacters.Add(player, networkPlayer);
        }
    }

    public void OnPlayerLeft(PlayerRef player)
    {
        if (_spawnedCharacters.ContainsKey(player))
        {
            NetworkObject networkObject = _spawnedCharacters[player];
            Runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }
}
