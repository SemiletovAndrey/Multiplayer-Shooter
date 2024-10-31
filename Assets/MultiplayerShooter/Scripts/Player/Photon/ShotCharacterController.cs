using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotCharacterController : NetworkBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Transform _weaponTransform;

    [Networked, OnChangedRender(nameof(UpdateWeaponRotation))] private float AimAngle { get; set; }


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
        NetworkObject bullet = Runner.Spawn(_bulletPrefab, _shootPoint.position, Quaternion.identity);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Initialize(new Vector2(Mathf.Cos(AimAngle * Mathf.Deg2Rad), Mathf.Sin(AimAngle * Mathf.Deg2Rad)));
    }
}
