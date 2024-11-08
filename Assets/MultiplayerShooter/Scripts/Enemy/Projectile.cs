using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float _speed = 10f;
    private float _lifetime = 5f;
    private float _timeAlive = 0f;
    private float _damage;

    public void Initialize(Vector2 direction, float damage, float lifeTime)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * _speed;
        _lifetime = lifeTime;
        _damage = damage;
    }

    public override void FixedUpdateNetwork()
    {
        _timeAlive += Runner.DeltaTime;

        if (_timeAlive >= _lifetime)
        {
            if (Object.HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
            return;
        }

        if (Object.HasStateAuthority)
        {
            transform.position += transform.right * _speed * Runner.DeltaTime;
        }
    }

    //TO DO Logic Damage
}
