using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponPhotonManager : NetworkBehaviour, IPlayerLeft
{
    [SerializeField] private PlayerSpawner _playerSpawner;

    private readonly int[] weaponIndices = { 0, 1, 2 };

    [Networked] public NetworkDictionary<PlayerRef, int> playerWeaponIndices => default;

    public void PlayerLeft(PlayerRef player)
    {
        playerWeaponIndices.Remove(player);
    }

    public int AssignUniqueWeaponIndex(PlayerRef player)
    {
        if (!Object.HasStateAuthority) return -1;

        List<int> availableIndices = new List<int>(weaponIndices);

        foreach (var kvp in playerWeaponIndices)
        {
            availableIndices.Remove(kvp.Value);
        }

        if (availableIndices.Count > 0)
        {
            int assignedIndex = availableIndices[Random.Range(0, availableIndices.Count)];
            playerWeaponIndices.Add(player, assignedIndex);
            return assignedIndex;
        }
        else
        {
            Debug.LogWarning("No available weapon indices. Increase the weapon pool if needed.");
            return -1;
        }
    }

    public void ReleaseWeaponIndex(PlayerRef player)
    {
        
            playerWeaponIndices.Remove(player);
        
    }
}
