using Fusion;
using UnityEngine;

public class ShotCharacterController : NetworkBehaviour
{
    [SerializeField] private Weapon _currentWeapon;
    [SerializeField] private Transform _weaponTransform;

    private PlayerData _playerData;

    [Networked, OnChangedRender(nameof(UpdateWeaponRotation))] private float AimAngle { get; set; }

    public void SetCurrentWeapon(Weapon currentWeapon)
    {
        _currentWeapon = currentWeapon;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            AimWeapon(data.aimDirection);

            if (data.isShooting)
            {
                Shoot();
            }
        }
    }

    private void AimWeapon(Vector2 aimDirection)
    {
        if (aimDirection.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            AimAngle = angle;
        }
    }

    private void UpdateWeaponRotation()
    {
        _weaponTransform.rotation = Quaternion.Euler(0, 0, AimAngle);

        bool isAimingLeft = AimAngle > 90 || AimAngle < -90;

        _weaponTransform.localScale = new Vector3(1, isAimingLeft ? -1 : 1, 1);
    }

    private void Shoot()
    {
        if (HasInputAuthority)
        {
            RPC_ShootRequest(AimAngle);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_ShootRequest(float aimAngle)
    {
        _currentWeapon.Shoot(aimAngle);
    }
}   
