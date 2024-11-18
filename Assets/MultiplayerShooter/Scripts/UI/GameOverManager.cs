using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class GameOverManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerDeathUIPrefab;
    [SerializeField] private Transform playerListContainer;

    [SerializeField] private NetworkRunner _runner;
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private PlayerAliveManager _playerAliveManager;
    [SerializeField] private WaveManager _waveManager;

    private Dictionary<PlayerRef, NetworkObject> _allPlayer = new Dictionary<PlayerRef, NetworkObject>();

    private void OnEnable()
    {
        foreach (var kvp in _playerSpawner.SpawnedCharacters)
        {
            _allPlayer.Add(kvp.Key, kvp.Value);
            Debug.Log("Add");
        }
        OnGameOver(_allPlayer, _playerAliveManager);
    }

    public void ShowGameOverUI(Dictionary<PlayerRef, NetworkObject> allPlayers)
    {
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var playerEntry in allPlayers)
        {
            PlayerRef playerRef = playerEntry.Key;
            NetworkObject networkObject = playerEntry.Value;

            PlayerModel playerModel = networkObject.GetComponent<PlayerModel>();
            if (playerModel != null)
            {
                GameObject playerUIObject = Instantiate(playerDeathUIPrefab, playerListContainer);
                PlayerDeathUIContainer playerDeathUI = playerUIObject.GetComponent<PlayerDeathUIContainer>();
                bool isLocalPlayer = networkObject.HasInputAuthority;

                playerDeathUI.InitializePlayerDeathUI(playerModel, isLocalPlayer);
            }
        }
    }

    public void OnGameOver(Dictionary<PlayerRef, NetworkObject> allPlayers, PlayerAliveManager playerAliveManager)
    {
        Runner.SessionInfo.IsOpen = false;
        ShowGameOverUI(allPlayers);
        _waveManager.gameObject.SetActive(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_ExitInMainMenuHostAndClients()
    {
        Debug.Log("Host: Loading Lobby for everyone...");
        SceneManager.LoadSceneAsync("Lobby");
        Runner.Shutdown();
    }

    public void ExitInMainMenu()
    {
        if (Runner.IsClient)
        {
            SceneManager.LoadSceneAsync("Lobby");
            Runner.Shutdown();
        }
        else
        {
            Rpc_ExitInMainMenuHostAndClients();
        }
    }
}
