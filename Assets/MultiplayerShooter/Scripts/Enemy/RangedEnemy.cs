using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] private GameObject ProjectilePrefab;
    [SerializeField] protected Transform ShootPoint;

    [SerializeField] protected float ProjectileLifeTime = 2.0f;
    [SerializeField] protected int Damage = 5;
    [SerializeField] protected int ProjectilePerShot = 1;

    protected override void Attack()
    {
        base.Attack();

        if (ProjectilePrefab != null)
        {
            Vector2 direction = (PlayerTransform.position - ShootPoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            SpawnBullet(angle);
        }
    }

    protected virtual void SpawnBullet(float angle)
    {
        Vector3 position = ShootPoint.position;
        if (Runner.IsServer)
        {
            NetworkObject projectile = Runner.Spawn(ProjectilePrefab, position, Quaternion.Euler(0, 0, angle));
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            projectileScript.Initialize(new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)), Damage, ProjectileLifeTime);
        }
    }
}
