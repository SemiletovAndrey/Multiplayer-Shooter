using Fusion;
using UnityEngine;

public class NetworkCharacterController2D : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _visualObject;
    [SerializeField] private Transform _weaponTransform;
    private Rigidbody _rigidbody;

    [Networked, OnChangedRender(nameof(UpdateDirection))] private bool IsFacingLeft { get; set; }
    [Networked, OnChangedRender(nameof(UpdateWeaponRotation))] private float AimAngle { get; set; }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
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
            AimWeapon(data.aimDirection);
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
        Vector3 movementDirection = new Vector3(movement.x, movement.y, 0);
        _rigidbody.MovePosition(_rigidbody.position + movementDirection);
    }

    private void AimWeapon(Vector2 aimDirection)
    {
        if (aimDirection.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            AimAngle = angle;
        }
    }

    private void UpdateWeaponRotation()
    {
        _weaponTransform.rotation = Quaternion.Euler(0, 0, AimAngle);

        bool isAimingLeft = AimAngle > 90 || AimAngle < -90;

        _weaponTransform.localScale = new Vector3(1, isAimingLeft ? -1 : 1, 1);
    }
}
