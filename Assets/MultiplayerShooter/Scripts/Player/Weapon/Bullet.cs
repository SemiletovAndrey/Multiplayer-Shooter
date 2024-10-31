using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float _speed = 10f;
    private float _lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, _lifetime);
    }

    public void Initialize(Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * _speed;
    }

    public override void FixedUpdateNetwork()
    {
        transform.position += transform.forward * _speed * Runner.DeltaTime;
    }
}
