using UnityEngine;
using UnityEngine.InputSystem;


public class MobileInputService : IInputService
{
    PlayerJoystick _playerJoystick = new PlayerJoystick();

    private InputAction _joystickInputMovement;
    private InputAction _joystickInputAim;
    public Vector2 InputVectorDirectionMovement { get {return _joystickInputMovement.ReadValue<Vector2>(); }}

    public Vector2 InputVectorDirectionAim { get { return _joystickInputAim.ReadValue<Vector2>(); } }

    public MobileInputService()
    {
        _playerJoystick = new PlayerJoystick();
        _joystickInputMovement = _playerJoystick.Movement.InputMovement;
        _joystickInputAim = _playerJoystick.Movement.InputAim;

        _playerJoystick.Enable();
        _joystickInputMovement.Enable();
    }

}
