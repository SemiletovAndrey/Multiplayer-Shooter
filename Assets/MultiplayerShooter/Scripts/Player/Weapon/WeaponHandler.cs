using Fusion;
using UnityEngine;

public class WeaponHandler : NetworkBehaviour
{
    [SerializeField] private GameObject[] _weapons;

    public int WeaponCount => _weapons.Length;

    public void SetWeaponOnIndex(int index)
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            _weapons[i].SetActive(i == index);
        }
    }

    public Weapon GetWeaponOnIndex(int index)
    {
        return _weapons[index].GetComponent<Weapon>();
    }
}
