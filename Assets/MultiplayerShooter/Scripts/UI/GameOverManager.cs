using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject playerDeathUIPrefab;
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private PlayerAliveManager _playerAliveManager;

    private Dictionary<PlayerRef, NetworkObject> _allPlayer = new Dictionary<PlayerRef, NetworkObject>();

    private void OnEnable()
    {
        foreach (var kvp in _playerSpawner.SpawnedCharacters)
        {
            _allPlayer.Add(kvp.Key, kvp.Value);
        }
        OnGameOver(_allPlayer, _playerAliveManager);
    }

    public void ShowGameOverUI(Dictionary<PlayerRef, NetworkObject> allPlayers, PlayerAliveManager playerAliveManager)
    {
        gameOverPanel.SetActive(true);

        // ������� ������ UI ��������
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }

        // �������� �� ���� �������
        foreach (var playerEntry in allPlayers)
        {
            PlayerRef playerRef = playerEntry.Key;
            NetworkObject networkObject = playerEntry.Value;

            // �������� PlayerModel (��� ������ ������), ��������� � �������
            PlayerModel playerModel = networkObject.GetComponent<PlayerModel>();
            if (playerModel != null)
            {
                // ������� UI-��������� ��� ������ � ��������� ��� �������
                GameObject playerUIObject = Instantiate(playerDeathUIPrefab, playerListContainer);
                PlayerDeathUIContainer playerDeathUI = playerUIObject.GetComponent<PlayerDeathUIContainer>();
                playerDeathUI.InitializePlayerDeathUI(playerModel);
            }
        }
    }

    public void OnGameOver(Dictionary<PlayerRef, NetworkObject> allPlayers, PlayerAliveManager playerAliveManager)
    {
        bool allPlayersDead = false;
        if (playerAliveManager.AliveCharacters.Count == 0)
        {
            allPlayersDead = true;
        }

        if (allPlayersDead)
        {
            ShowGameOverUI(allPlayers, playerAliveManager);
        }
    }
}
