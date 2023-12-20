using System.Linq;
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


    private ControlMode _controlMode = ControlMode.KeyboardMouse;
    public Vector2 MoveVector => _MoveVector;
    public Vector2 AimVector => _AimVector;
    public float Fire => _Fire;
    public float AimDownScope =>  _AimDownScope;

    private Vector2 _MoveVector;
    private Vector2 _AimVector;
    private float _Fire;
    private float _AimDownScope;

    //Get Gamepad that controls this PlayerController
    //Player Index must match
    public Gamepad GetGamepad(){
        if (Gamepad.all.Count >= PlayerIndex + 1){
            return Gamepad.all[(int)PlayerIndex];
        }
        else{
            return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onEvent += (eventP,device) => {
            this.HandleInputEvent(eventP, device);
        };
    }

    // Update is called once per frame
    void Update()
    {
        // Lock Cursor When Not Paused
        if (!GameManager.Paused && Application.isFocused){
            Cursor.lockState = CursorLockMode.Locked;
        }
        else{
            Cursor.lockState = CursorLockMode.None;
        }

        _MoveVector = Vector2.zero;
        _AimVector = Vector2.zero;
        
        // Get Input based on Active (most recent) Device
        if (_controlMode == ControlMode.KeyboardMouse){
            Keyboard _keyboard = Keyboard.current;
            Mouse _mouse = Mouse.current;

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

            if (_mouse.rightButton.isPressed){
                _AimDownScope = 1.0f;
            }
            else{
                _AimDownScope = 0.0f;
            }
            _AimVector = _mouse.delta.ReadValue() / 30;
                        
        }
        else
        if (_controlMode == ControlMode.Gamepad){
                Gamepad _gamepad = GetGamepad();
                var leftStick = _gamepad.leftStick.ReadValue();
                var rightStick = _gamepad.rightStick.ReadValue();
                var rightTrigger = _gamepad.rightTrigger.ReadValue();
                var leftTrigger = _gamepad.leftTrigger.ReadValue();

                if (leftStick.magnitude > GamepadAnalogueDeadZone){
                    _MoveVector = leftStick;
                }
                if (rightStick.magnitude > GamepadAnalogueDeadZone){
                    _AimVector = rightStick;
                }

                float triggerDeadZone = 0.1f;
                if (rightTrigger >= triggerDeadZone){
                    _Fire = rightTrigger;
                }
                else{
                    _Fire = 0.0f;
                }

                if (leftTrigger >= triggerDeadZone){
                    _AimDownScope = leftTrigger;
                }
                else{
                    _AimDownScope = 0.0f;
                }
        }

    }

    public void HandleInputEvent(InputEventPtr @event, InputDevice inputDevice){

        if (inputDevice is Keyboard){
            _controlMode = ControlMode.KeyboardMouse;
        }
        else if (inputDevice is Gamepad){
            if ((Gamepad)inputDevice == GetGamepad())
            _controlMode = ControlMode.Gamepad;
        }
    }
}
