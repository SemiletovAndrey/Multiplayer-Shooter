using Fusion;
using UnityEngine;

public class NetworkCharacterController2D : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _visualObject;
    private Rigidbody2D _rigidbody2D;

    [Networked, OnChangedRender(nameof(UpdateDirection))] private bool IsFacingLeft { get; set; }

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void UpdateDirection()
    {
        _visualObject.localScale = new Vector3(IsFacingLeft ? -1 : 1, 1, 1);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            MovementPlayer(data);
            DirectionMovementPlayer(data);
        }
    }

    private void DirectionMovementPlayer(NetworkInputData data)
    {
        if (data.direction.x < 0 && !IsFacingLeft)
        {
            IsFacingLeft = true;
        }
        else if (data.direction.x > 0 && IsFacingLeft)
        {
            IsFacingLeft = false;
        }
    }

    private void MovementPlayer(NetworkInputData data)
    {
        Vector2 movement = data.direction * _moveSpeed * Runner.DeltaTime;
        _rigidbody2D.MovePosition(_rigidbody2D.position + movement);
    }
}
