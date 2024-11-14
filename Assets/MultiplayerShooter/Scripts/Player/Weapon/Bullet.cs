using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float _speed = 10f;
    private float _lifetime = 5f;
    private float _timeAlive = 0f;
    private int _damage;
    private PlayerModel _playerModel;

    public void Initialize(Vector2 direction, int damage, float lifeTime, PlayerModel playerModel)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * _speed;
        _lifetime = lifeTime;
        _damage = damage;
        _playerModel = playerModel;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (collision.TryGetComponent<Enemy>(out Enemy enemy))
            {
                if (!enemy.IsDead)
                {
                    enemy.TakeDamage(_damage, _playerModel);
                    if (_playerModel != null)
                    {
                        _playerModel.AddAllDamage(_damage);
                    }
                    Runner.Despawn(Object);
                }
            }
        }
    }
}
