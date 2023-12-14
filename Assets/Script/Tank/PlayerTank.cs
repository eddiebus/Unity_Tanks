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

    public VisionGrid VisionGrid;
    // Start is called before the first frame update
    void Start()
    {
        var test = GameManager.Instance;
        base._TankInit();
        _PlayerCon = this.GetComponent<PlayerController>();
        _GameCamera = GetComponentInChildren<GameCamera>();
        _TankGun = GetComponentInChildren<Gun>();

        this.gameObject.tag = Character.GameObjectTagName;

        VisionGrid = new VisionGrid(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        _HandleMoveInput();
        _HandleAimInput();
        _TurnTurrets();

        VisionGrid.UpdatePoints(this.transform.position);
    }

    void LateUpdate()
    {
        _GameCamera.UpdateTransform();
    }

    void OnDrawGizmos()
    {
        if (VisionGrid != null)
        {
            VisionGrid.DrawDebugGizmo();
        }

        if (_GameCamera)
        {
            Vector3 AimPoint = _GameCamera.GetWorldAimPoint();
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(AimPoint, 0.3f);
        }
    }

    private void _HandleMoveInput()
    {
        if (!_PlayerCon || !_GameCamera) return;
        var InputVector = _PlayerCon.MoveVector;

        Vector3 CamEuler = _GameCamera.transform.rotation.eulerAngles;

        var MoveVector = new Vector3(
            InputVector.x,
            0,
            InputVector.y
        );


        MoveVector = Quaternion.Euler(
            new Vector3(0, CamEuler.y, 0)
        ) * MoveVector;

        MoveTo(transform.position + MoveVector);
    }

    private void _HandleAimInput()
    {
        if (!_PlayerCon || !_GameCamera) return;

        var Cameuler = _GameCamera.GetRotation().eulerAngles;
        var InputVector = _PlayerCon.AimVector;
        Vector3 AimVector = new Vector3(
            InputVector.x,
            0,
            InputVector.y
        );

        _GameCamera.Rotate(_PlayerCon.AimVector);

        AimVector = Quaternion.AngleAxis(Cameuler.y, Vector3.up) * AimVector;
        AimAt(_GameCamera.GetWorldAimPoint());

        if (_PlayerCon.Fire > 0.1f)
        {
            var hasFired = _TankGun.Fire();
        }
    }
}
