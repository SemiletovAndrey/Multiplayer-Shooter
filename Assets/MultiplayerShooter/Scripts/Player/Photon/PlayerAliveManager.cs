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
            DiePlayerRpc();
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
            Debug.LogWarning("“олько сервер может удал€ть игроков из словар€.");
        }
    }

    private void RemovePlayerFromDictionary(PlayerRef playerRef)
    {
        if (AliveCharacters.ContainsKey(playerRef))
        {
            AliveCharacters.Remove(playerRef);
            Debug.Log($"»грок {playerRef} был удален из словар€.");
        }
        else
        {
            Debug.LogWarning($"»грок {playerRef} не найден в словаре.");
        }
    }

    private void CheckAndFollowNextAlivePlayer()
    {
        if (AliveCharacters.Count == 0)
        {
            Debug.Log("¬се игроки мертвы.");
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
                    else
                    {
                        Debug.LogWarning("CameraFollower не найден на основной камере.");
                    }
                }
                else
                {
                    Debug.LogWarning("ќсновна€ камера (Camera.main) не найдена.");
                }
            }
            else
            {
                Debug.LogWarning("PlayerObject дл€ случайного живого игрока не найден.");
            }
        }
        else
        {
            Debug.Log("Ќет доступных игроков дл€ переключени€ камеры.");
        }
    }
}
