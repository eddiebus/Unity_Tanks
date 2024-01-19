using System;
using Palmmedia.ReportGenerator.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;


public enum PlayerConState
{
    KeyboardMouse,
    Gamepad,
}

public struct PlayerConSetting{
    public float MouseSensitivity;
    public float GamepadSensitivity;
    public float GamepadDeadZone;
    public float TriggerDeadZone;
}

// New Player Controller with Static Methods
public class PlayerCon
{
    private static PlayerConSetting settings = new PlayerConSetting();

    private static PlayerCon[] _Instances;
    private int _ControllerIndex = -1;
    private PlayerConState _ControllerState = PlayerConState.KeyboardMouse;
    public Action<PlayerCon> SomeAction = new Action<PlayerCon>((PlayerCon theCon) => { });
    public Vector2 MoveVector => _MoveVector;

    public Vector2 AimPoint => _AimPoint;
    public Vector2 AimVector => _AimVector;
    public float Fire => _Fire;
    public float AimDownScope => _ADS;

    private Vector2 _MoveVector;
    private Vector2 _AimVector;
    private Vector2 _AimPoint = new Vector2(0.5f,0.5f);
    private float _Fire;
    private float _ADS;

    public static PlayerCon GetController(int Index)
    {
        if (_Instances == null)
        {
            _InitController();
        }
        _Instances[Index]._UpdateValues();

        return _Instances[Index];
    }

    public static void SetSettings(PlayerConSetting newSettings){
        settings = newSettings;
    }

    private static void _InitController()
    {
        var someCon = new PlayerCon(0);
        _Instances = new PlayerCon[]{
            new PlayerCon(0),
            new PlayerCon(1),
            new PlayerCon(2),
            new PlayerCon(3)
        };
    }

    private PlayerCon(int PlayerIndex)
    {
        _UpdateValues();
        _ControllerIndex = PlayerIndex;

        InputSystem.onEvent += (eventP, device) =>
        {
            this._CheckState(eventP, device);
        };
    }

    private void _UpdateValues()
    {
        // Lock Cursor When Not Paused
        if (!GameManager.Paused && Application.isFocused)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        _MoveVector = Vector2.zero;
        _AimVector = Vector2.zero;


        switch (_ControllerState)
        {
            case PlayerConState.KeyboardMouse:
                {
                    // Get Input based on Active (most recent) Device
                    Keyboard _keyboard = Keyboard.current;
                    Mouse _mouse = Mouse.current;

                    var keyMoveVector = Vector2.zero;
                    var up = _keyboard.wKey.isPressed;
                    var down = _keyboard.sKey.isPressed;
                    var left = _keyboard.aKey.isPressed;
                    var right = _keyboard.dKey.isPressed;

                    if (up && !down)
                    {
                        keyMoveVector += Vector2.up;
                    }
                    else if (down && !up)
                    {
                        keyMoveVector += Vector2.down;
                    }

                    if (left && !right)
                    {
                        keyMoveVector += Vector2.left;
                    }
                    else if (right && !left)
                    {
                        keyMoveVector += Vector2.right;
                    }

                    _MoveVector = keyMoveVector;
                    if (_mouse.leftButton.isPressed)
                    {
                        _Fire = 1.0f;
                    }
                    else
                    {
                        _Fire = 0.0f;
                    }

                    if (_mouse.rightButton.isPressed)
                    {
                        _ADS = 1.0f;
                    }
                    else
                    {
                        _ADS = 0.0f;
                    }

                    _AimVector = (_mouse.delta.ReadValue() * 1.0f) / 20;
                    break;
                }
            case PlayerConState.Gamepad:
                {
                    Gamepad _gamepad = GetGamepad();
                    var leftStick = _gamepad.leftStick.ReadValue();
                    var rightStick = _gamepad.rightStick.ReadValue();
                    var rightTrigger = _gamepad.rightTrigger.ReadValue();
                    var leftTrigger = _gamepad.leftTrigger.ReadValue();

                    _AimPoint += rightStick * Time.deltaTime;
                    if (leftStick.magnitude > 0.1f)
                    {
                        _MoveVector = leftStick;
                    }
                    if (rightStick.magnitude > 0.1f)
                    {
                        _AimVector = rightStick * 1.0f * 2;
                    }
                    else
                    {
                        _AimVector = Vector2.zero;
                    }

                    float triggerDeadZone = 0.1f;
                    if (rightTrigger >= triggerDeadZone)
                    {
                        _Fire = rightTrigger;
                    }
                    else
                    {
                        _Fire = 0.0f;
                    }

                    if (leftTrigger >= triggerDeadZone)
                    {
                        _ADS = leftTrigger;
                    }
                    else
                    {
                        _ADS = 0.0f;
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }

        // Clamp AimPoint
        float Limit = 1.0f;
        if (_AimPoint.x  > Limit){
            _AimPoint.x = Limit;
        }
        else if (_AimPoint.x < - Limit){
            _AimPoint.x = -Limit;
        }


        if (_AimPoint.y  > Limit){
            _AimPoint.y = Limit;
        }
        else if (_AimPoint.y < - Limit){
            _AimPoint.y = -Limit;
        }
        
    }

    public Gamepad GetGamepad()
    {
        if (_ControllerIndex < Gamepad.all.Count)
        {
            return Gamepad.all[_ControllerIndex];
        }
        else
        {
            return null;
        }
    }

    private void _CheckState(InputEventPtr @event, InputDevice inputDevice)
    {
        if (inputDevice is Keyboard)
        {
            _ControllerState = PlayerConState.KeyboardMouse;
        }
        else if (inputDevice is Gamepad)
        {
            if ((Gamepad)inputDevice == GetGamepad())
                _ControllerState = PlayerConState.Gamepad;
        }
    }

}
