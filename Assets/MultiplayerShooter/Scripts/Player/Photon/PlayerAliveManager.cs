using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAliveManager : NetworkBehaviour
{
    // TO DO
    [Networked] public NetworkDictionary<PlayerRef, NetworkObject> AliveCharacters => default;

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
            AliveCharacters.Remove(Object.InputAuthority);
            CheckAndFollowNextAlivePlayer();
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
