using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerTank : Tank
{
    public Quaternion TurretDiff;
    public PlayerController _PlayerCon;
    public GameCamera _GameCamera;
    private Gun _TankGun;


    [Range(0.1f, 1.0f)]
    public float VisionUpdateTime = 0.5f;
    private float _CurrentVisionUpdateTime;
    public VisionGrid VisionGrid;
    // Start is called before the first frame update
    void Start()
    {
        var test = GameManager.Instance;
        base._TankInit();
        _PlayerCon = this.GetComponent<PlayerController>();
        _TankGun = GetComponentInChildren<Gun>();

        this.gameObject.tag = Character.GameObjectTagName;

        VisionGrid = new VisionGrid(gameObject);
        _CurrentVisionUpdateTime = 0.0f;

    }

    void Update()
    {
        _HandleMoveInput();
        _HandleAimInput();
        _TurnTurrets();
        _UpdateRig();
        if (_CurrentVisionUpdateTime <= 0)
        {
            _CurrentVisionUpdateTime = VisionUpdateTime;
            VisionGrid.UpdatePoints(this.transform.position);
        }
        else
        {
            _CurrentVisionUpdateTime -= Time.deltaTime;
        }
        TurretDiff = _TargetTurretQuat * Quaternion.Inverse(_TurretQuat);

    }

    void FixedUpdate()
    {
        // _UpdateRig();
    }

    void LateUpdate()
    {
        _UpdateRig();
        AimAt(_GameCamera.GetWorldAimPoint());
    }

    void OnDrawGizmos()
    {
        if (VisionGrid != null)
        {
            VisionGrid.DrawDebugGizmo();
        }

        if (_GameCamera)
        {
            Vector3 AimPoint = TurretAimControl.transform.position;

            float AimSphereGizmoSize = 0.3f;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(AimPoint, AimSphereGizmoSize);

            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawSphere(GetCurrentAimPoint(), AimSphereGizmoSize * 1.2f);
        }

        var rigBounds = new RigidBodyBounds(_RigidBody);
        rigBounds.DrawDebugGizmos();
    }

    private void _HandleMoveInput()
    {
        if (!_GameCamera) return;
        var InputVector = PlayerCon.GetController(0).MoveVector;
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
        if (!_GameCamera) return;

        PlayerCon Controller = PlayerCon.GetController(0);

        if (_GameCamera)
        {
            if (Controller.AimDownScope > 0.0f)
            {
                _GameCamera.SetMode(GameCameraMode.ADS);
            }
            else
            {
                _GameCamera.SetMode(GameCameraMode.Default);
            }

            var InputVector = Controller.AimVector;

            _GameCamera.Rotate(Controller.AimVector);

            AimAt(_GameCamera.GetWorldAimPoint());
        }

        if (Controller.Fire > 0.1f)
        {
            _TankGun.Fire();
        }
    }

    public Vector3 GetCurrentAimPoint()
    {
        var TargetDistance = (transform.position - GetTargetAimPoint()).magnitude;
        var endPoint = (_TurretQuat * (Vector3.forward * TargetDistance)) + transform.position;
        Ray HitRay = new Ray(
            transform.position,
            _TurretQuat * Vector3.forward
            );

        RaycastHit HitInfo;
        bool visionhit = Physics.Raycast(HitRay, out HitInfo, TargetDistance, _GameCamera.VisionLayerMask,
        QueryTriggerInteraction.Ignore);

        if (visionhit)
        {
            return HitInfo.point;
        }
        else
        {
            return endPoint;
        }

    }

    public Vector3 GetTargetAimPoint()
    {
        return _GameCamera.GetWorldAimPoint();
    }

}
