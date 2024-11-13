using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : NetworkBehaviour
{
    [SerializeField] private List<GameObject> _weapons;


    public void SetWeaponOnIndex(int index)
    {
        for (int i = 0; i < _weapons.Count; i++)
        {
            if (i == index)
            {
                _weapons[i].SetActive(true);
            }
            else
            {
                _weapons[i].SetActive(false);
            }
        }
    }

    public Weapon GetWeaponOnIndex(int index)
    {
        Weapon weapon = _weapons[index].GetComponent<Weapon>();
        if (weapon == null)
        {
            Debug.LogError($"Weapon at index {index} does not have a Weapon component attached!");
        }
        return weapon;
    }
}
