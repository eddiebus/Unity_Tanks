using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerTank : Tank
{
    public PlayerController _PlayerCon;
    public GameCamera _GameCamera;

    private Gun _TankGun;
    // Start is called before the first frame update
    void Start()
    {
        base._TankInit();
        _PlayerCon = this.GetComponent<PlayerController>();
        _GameCamera = GetComponentInChildren<GameCamera>();
        _TankGun = GetComponentInChildren<Gun>();

        CharacterTag = "PlayerTank";
    }

    // Update is called once per frame
    void Update()
    {
        _HandleMoveInput();
        _HandleAimInput();
        _TurnTurrets();

    }

    void LateUpdate(){
        _GameCamera.UpdateTransform();
    }

    private void _HandleMoveInput(){
        if (!_PlayerCon || !_GameCamera) return;
        var InputVector = _PlayerCon.MoveVector;

        Vector3 CamEuler = _GameCamera.transform.rotation.eulerAngles;


        var MoveVector = new Vector3(
            InputVector.x,
            0,
            InputVector.y
        );


        MoveVector = Quaternion.Euler(
            new Vector3(0,CamEuler.y,0)
        ) * MoveVector;

        MoveTo( transform.position + MoveVector);
    }

    private void _HandleAimInput(){
        if (!_PlayerCon || !_GameCamera) return;

        var Cameuler = _GameCamera.GetRotation().eulerAngles;
        var InputVector = _PlayerCon.AimVector;
        Vector3 AimVector = new Vector3(
            InputVector.x,
            0,
            InputVector.y
        );

        _GameCamera.Rotate(_PlayerCon.AimVector);

        AimVector = Quaternion.AngleAxis(Cameuler.y,Vector3.up) * AimVector;
        AimAt(_GameCamera.GetWorldAimPoint());

        if (_PlayerCon.Fire > 0.1f){
            var hasFired = _TankGun.Fire();
        }

        
    }
}
