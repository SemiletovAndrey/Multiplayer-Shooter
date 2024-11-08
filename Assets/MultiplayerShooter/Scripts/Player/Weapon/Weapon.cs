using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public abstract class Weapon : NetworkBehaviour
{
    [SerializeField] protected NetworkPrefabRef BulletPrefab;
    [SerializeField] protected Transform ShootPoint;

    [SerializeField] protected float bulletLifetime = 2.0f;
    [SerializeField] protected int damage = 10;
    [SerializeField] protected int bulletsPerShot = 1;
    [SerializeField] protected int maxAmmo = 30;
    [SerializeField] protected float shootCooldown = 1f;

    private float _lastShootTime = 0f;

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

        if (CanShoot())
        {
            _currentAmmo -= BulletsPerShot;

            for (int i = 0; i < bulletsPerShot; i++)
            {
                float spreadAngle = aimAngle + GetSpreadAngle(i);
                SpawnBullet(spreadAngle);
            }
            _lastShootTime = Time.time;
        }
    }

    private bool CanShoot()
    {
        // Проверяем, прошло ли время перезарядки
        return Time.time >= _lastShootTime + shootCooldown;
    }

    protected virtual float GetSpreadAngle(int bulletIndex)
    {
        return 0f;
    }

    protected virtual void SpawnBullet(float angle)
    {
        Vector3 position = ShootPoint.position;
        if (Runner.IsServer)
        {
            NetworkObject bullet = Runner.Spawn(BulletPrefab, position, Quaternion.Euler(0, 0, angle));
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.Initialize(new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)), Damage, BulletLifetime);
        }
    }
}
