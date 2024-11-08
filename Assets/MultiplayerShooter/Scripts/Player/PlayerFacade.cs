using Fusion;
using UnityEngine;

public class PlayerFacade : NetworkBehaviour
{
    [SerializeField] private WeaponHandler _weaponHandler;
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private ShotCharacterController _shotCharacterController;

    [Networked, OnChangedRender(nameof(OnWeaponIndexChanged))]
    private int currentWeaponIndex { get; set; }

    public void SetWeapon(int indexWeapon)
    {
        if (Object.HasStateAuthority)
        {
            currentWeaponIndex = indexWeapon;
        }
    }

    private void OnWeaponIndexChanged()
    {
        UpdateWeapon();
    }

    private void UpdateWeapon()
    {
        if (_weaponHandler == null || _playerData == null || _shotCharacterController == null)
        {
            Debug.LogError("NULL");
            return;
        }

        _weaponHandler.SetWeaponOnIndex(currentWeaponIndex);
        _playerData.ActiveWeapon = _weaponHandler.GetWeaponOnIndex(currentWeaponIndex);
        _shotCharacterController.SetCurrentWeapon(_playerData.ActiveWeapon);
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            SetWeapon(UnityEngine.Random.Range(0, _weaponHandler.WeaponCount));
        }
        else
        {
            UpdateWeapon();
        }
    }
}
