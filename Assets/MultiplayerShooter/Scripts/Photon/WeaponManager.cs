using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private List<Weapon> weaponPrefabs; 

    private HashSet<int> assignedWeapons = new HashSet<int>(); 

    public Weapon GetUniqueWeapon()
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, weaponPrefabs.Count);
        } while (assignedWeapons.Contains(randomIndex));

        assignedWeapons.Add(randomIndex);
        return weaponPrefabs[randomIndex]; 
    }

    public void SpawnWeaponOnPlayer(NetworkObject player, Weapon weaponPrefab)
    {
        if (player.HasStateAuthority)
        {
            Vector3 spawnPosition = player.transform.position;  
            Quaternion spawnRotation = player.transform.rotation;

            if (weaponPrefab.gameObject != null)
            {
                // Make sure the weaponPrefab has a NetworkObject component
                NetworkObject weaponNetworkObject = weaponPrefab.GetComponent<NetworkObject>();
                if (weaponNetworkObject == null)
                {
                    Debug.LogError("Weapon prefab does not have a NetworkObject component attached.");
                    return;
                }

                // Proceed with spawning the weapon
                NetworkObject weapon = Runner.Spawn(weaponPrefab.gameObject, spawnPosition, spawnRotation);
                weapon.transform.SetParent(player.transform);
            }
            else
            {
                Debug.LogError("Weapon prefab or its gameObject is null!");
            }
        }
    }
}
