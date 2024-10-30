using Fusion;
using UnityEngine;
using Zenject;

public class NetworkCharacterController2D : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D _rigidbody2D;
    private Transform _playerTransform;

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerTransform = GetComponent<Transform>();
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
        if (data.direction.x < 0 && _playerTransform.localScale.x > 0)
        {
            RPC_SetDirection(true);
        }
        else if (data.direction.x > 0 && _playerTransform.localScale.x < 0)
        {
            RPC_SetDirection(false);
        }
    }


    private void MovementPlayer(NetworkInputData data)
    {
        Vector2 movement = data.direction * moveSpeed * Runner.DeltaTime;
        _rigidbody2D.MovePosition(_rigidbody2D.position + movement);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_SetDirection(bool isFacingLeft)
    {
        _playerTransform.localScale = new Vector3(isFacingLeft ? -1 : 1, 1, 1);
    }
}
