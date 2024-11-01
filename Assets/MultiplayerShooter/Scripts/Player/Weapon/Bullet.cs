using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float _speed = 10f;
    private float _lifetime = 5f;
    private float _timeAlive = 0f;

    public void Initialize(Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * _speed;
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
}
