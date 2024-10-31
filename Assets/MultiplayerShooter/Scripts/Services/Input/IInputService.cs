using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputService
{
    public Vector2 InputVectorDirectionMovement { get; }
    public Vector2 InputVectorDirectionAim { get; }
    bool IsShooting { get; }
}
