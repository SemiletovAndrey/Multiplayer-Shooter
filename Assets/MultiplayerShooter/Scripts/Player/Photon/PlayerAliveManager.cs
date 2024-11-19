using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAliveManager : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnChangeAlivePlayer))] public NetworkDictionary<PlayerRef, NetworkObject> AliveCharacters => default;

    [SerializeField] private GameObject _playerDeathUI;

    public void OnChangeAlivePlayer()
    {
        if (AliveCharacters.Count == 0)
        {
            Camera.main.gameObject.GetComponent<CameraFollower>().enabled = false;
            _playerDeathUI.SetActive(true);
        }
    }

    public void AddAliveCharacter(PlayerRef player, NetworkObject networkObject)
    {
        AliveCharacters.Add(player, networkObject);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DiePlayerRpc()
    {
        DiePlayer();
    }

    public void DiePlayer()
    {
        if (Object.HasStateAuthority)
        {
            Debug.Log("Die player");
            OnPlayerDeath(Object.InputAuthority);
            CheckAndFollowNextAlivePlayer();
        }
    }

    public void OnPlayerDeath(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            RemovePlayerFromDictionary(playerRef);
        }
        else
        {
            Debug.LogWarning("Только сервер может удалять игроков из словаря.");
        }
    }

    public void RemovePlayerFromDictionary(PlayerRef playerRef)
    {
        if (AliveCharacters.ContainsKey(playerRef))
        {
            AliveCharacters.Remove(playerRef);
            Debug.Log($"Игрок {playerRef} был удален из словаря.");
        }
        else
        {
            Debug.LogWarning($"Игрок {playerRef} не найден в словаре.");
        }
    }

    private void CheckAndFollowNextAlivePlayer()
    {
        if (AliveCharacters.Count == 0)
        {
            Debug.Log("Все игроки мертвы.");
            return;
        }

        var randomPlayerKey = AliveCharacters.ElementAt(UnityEngine.Random.Range(0, AliveCharacters.Count)).Key;

        if (randomPlayerKey != null && Runner != null)
        {
            var playerObject = Runner.GetPlayerObject(randomPlayerKey);
            if (playerObject != null)
            {
                var playerTransform = playerObject.transform;

                var mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    var cameraFollower = mainCamera.GetComponent<CameraFollower>();
                    if (cameraFollower != null)
                    {
                        cameraFollower.CameraFollow(playerTransform);
                    }   
                }       
            }
        }
    }
}
