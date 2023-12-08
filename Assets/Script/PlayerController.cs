using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;


public enum ControlMode {
    KeyboardMouse,
    Gamepad
}

public class PlayerController : MonoBehaviour
{
    public uint PlayerIndex = 0;
    [Range(0.01f,0.5f)]public float GamepadAnalogueDeadZone = 0.1f;

    
    public ControlMode ControlMode => _controlMode;
    private ControlMode _controlMode;
    public Vector2 MoveVector => _MoveVector;
    public Vector2 AimVector => _AimVector;
    public float Fire => _Fire;

    private Vector2 _MoveVector;
    private Vector2 _AimVector;
    private float _Fire;

    private Camera _PlayerCamera;


    public Gamepad GetGamepad(){
        if (Gamepad.all.Count >= PlayerIndex + 1){
            return Gamepad.all[(int)PlayerIndex];
        }
        else{
            return null;
        }
    }
    public void SetCamera(Camera camera){
        _PlayerCamera = camera;
    }
    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onEvent += (eventP,device) => {
            this.HandleInputEvent(eventP);
        };
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard _keyboard = Keyboard.current;
        Mouse _mouse = Mouse.current;
        Gamepad _gamepad = null;

        if (Gamepad.all.Count >= PlayerIndex + 1){
            _gamepad = Gamepad.all.ElementAt<Gamepad>((int)PlayerIndex);
        }

        _MoveVector = Vector2.zero;
        _AimVector = Vector2.zero;
        if (PlayerIndex > 0){
            _controlMode = ControlMode.Gamepad;
        }

        if (PlayerIndex == 0){
            var keyMoveVector = Vector2.zero;

            var up = _keyboard.wKey.isPressed;
            var down = _keyboard.sKey.isPressed;
            var left = _keyboard.aKey.isPressed;
            var right = _keyboard.dKey.isPressed; 

            if (up && !down){
                keyMoveVector += Vector2.up;
            }
            else if (down && !up){
                keyMoveVector += Vector2.down;
            }

            if (left && !right){
                keyMoveVector += Vector2.left;
            }
            else if (right && !left){
                keyMoveVector += Vector2.right;
            }

            _MoveVector = keyMoveVector;

            if (_mouse.leftButton.isPressed){
                _Fire = 1.0f;
            }
            else{
                _Fire = 0.0f;
            }
            
        }

        if (_gamepad != null){
                var leftStick = _gamepad.leftStick.ReadValue();
                var rightStick = _gamepad.rightStick.ReadValue();
                var gamepadFire = _gamepad.rightTrigger.ReadValue();


                if (leftStick.magnitude > GamepadAnalogueDeadZone){
                    _MoveVector = leftStick;
                }
                if (rightStick.magnitude > GamepadAnalogueDeadZone){
                    _AimVector = rightStick;
                }

                if (gamepadFire > 0.05f){
                    _Fire = gamepadFire;
                }
                
        }

    }

    public void HandleInputEvent(InputEventPtr @event){

    }
}
