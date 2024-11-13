using Fusion;
using UnityEngine;

public class NetworkCharacterController2D : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _visualObject;
    [SerializeField] private PlayerAnimation _playerAnimation;
    private Rigidbody2D _rigidbody2D;

    [Networked, OnChangedRender(nameof(UpdateDirection))] private bool IsFacingLeft { get; set; }

    [Networked, OnChangedRender(nameof(OnIsRunningChanged))] private bool IsRunning { get; set; }

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void UpdateDirection()
    {
        _visualObject.localScale = new Vector3(IsFacingLeft ? -1 : 1, 1, 1);
    }

    private void OnIsRunningChanged()
    {
        _playerAnimation.SetRunning(IsRunning);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            MovementPlayer(data);
            DirectionMovementPlayer(data);
            IsRunning = data.direction.magnitude > 0.01f;
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
