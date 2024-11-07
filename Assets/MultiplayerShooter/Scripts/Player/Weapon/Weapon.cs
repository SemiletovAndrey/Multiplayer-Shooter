using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public abstract class Weapon : NetworkBehaviour
{
    [SerializeField] protected NetworkPrefabRef _bulletPrefab;
    [SerializeField] protected Transform _shootPoint;

    [SerializeField] protected float bulletLifetime = 2.0f;
    [SerializeField] protected int damage = 10;
    [SerializeField] protected int bulletsPerShot = 1;
    [SerializeField] protected int maxAmmo = 30;

    [Networked] protected int _currentAmmo { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            _currentAmmo = maxAmmo;
        }
    }

    public float BulletLifetime => bulletLifetime;
    public int Damage => damage;
    public int BulletsPerShot => bulletsPerShot;
    public int Ammo => _currentAmmo;
    public int MaxAmmo => maxAmmo;

    public virtual void Shoot(float aimAngle)
    {
        if (!Object.HasStateAuthority) return;

        if (_currentAmmo <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        _currentAmmo -= BulletsPerShot;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float spreadAngle = aimAngle + GetSpreadAngle(i);
            SpawnBullet(spreadAngle);
        }
    }

    protected virtual float GetSpreadAngle(int bulletIndex)
    {
        return 0f;
    }

    protected virtual void SpawnBullet(float angle)
    {
        Vector3 position = _shootPoint.position;
        if (Runner.IsServer)
        {
            NetworkObject bullet = Runner.Spawn(_bulletPrefab, position, Quaternion.Euler(0, 0, angle));
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.Initialize(new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)), Damage, BulletLifetime);
        }
    }
}
