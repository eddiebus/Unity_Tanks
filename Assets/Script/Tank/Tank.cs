using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class AI_TankMoveTo : AITask
{
    private Tank _Caller;
    private Vector3 _TargetPosition;
    private NavMeshPath NavPath = new NavMeshPath();
    private float _PathDistance;
    private float _TimeTaken;

    private int PathIndex = 0;
    public AI_TankMoveTo(Tank Caller, Vector3 TargetPosition)
    {
        _Caller = Caller;
        _TargetPosition = TargetPosition;

        try
        {
            NavMesh.CalculatePath(_Caller.transform.position, _TargetPosition,
            NavMesh.AllAreas, NavPath);
        }
        catch
        {
            Debug.LogWarning($"NavMesh returned invalid Vector Position");
            this._State = AITaskState.Failed;
        }

        for (int i = -1; i < NavPath.corners.Length - 1; i++)
        {
            Vector3 Start;
            Vector3 End;

            if (i < 0)
            {
                Start = Caller.transform.position;
            }
            else
            {
                Start = NavPath.corners[i];
            }

            End = NavPath.corners[i + 1];
            var distance = (End - Start).magnitude;
            _PathDistance += distance;
        }
    }

    public override void Update()
    {
        float TargetTime = _PathDistance / _Caller.MoveSpeed;
        _TimeTaken += Time.deltaTime;

        // Tank taken too long to reach destination
        // End Task
        if (_TimeTaken > TargetTime * 1.1f)
        {
            _State = AITaskState.Failed;
            return;
        }

        if (PathIndex < NavPath.corners.Length)
        {
            _Caller.MoveTo(NavPath.corners[PathIndex]);

            var CurrentPos = _Caller.transform.position;
            CurrentPos.y = 0;

            var TargetPos = NavPath.corners[PathIndex];
            TargetPos.y = 0;

            var DistanceToPoint = (TargetPos - CurrentPos).magnitude;
            if (DistanceToPoint < 0.1f)
            {
                PathIndex++;
            }
        }
        else
        {
            _State = AITaskState.Completed;
        }
    }

    public override void DrawDebugGizmo()
    {

        for (int i = 0; i < NavPath.corners.Length; i++)
        {
            if (i < PathIndex)
            {
                Gizmos.color = new Color(0, 0, 0, 0.5f);
            }
            else if (i > PathIndex)
            {
                Gizmos.color = new Color(0, 1, 0, 0.5f);
            }
            else
            {
                Gizmos.color = new Color(1, 1, 0, 0.75f);
            }

            Gizmos.DrawSphere(NavPath.corners[i], 0.2f);

        }

        var pathdraw = new NavPathDebugDraw(NavPath);
        pathdraw.DebugDrawPath();
    }
}

public class Tank : MonoBehaviour
{
    public float MoveSpeed = 5.0f;
    public float TurnSpeed = 10.0f;
    public float TurretTurnSpeed = 1.0f;
    protected Rigidbody _RigidBody;

    private Vector3 AimPoint;
    protected Quaternion _TurretQuat = Quaternion.identity;
    protected Quaternion _TargetTurretQuat;

    // Obj turrets are rigged to aim at
    public GameObject TurretAimControl;

    // Start is called before the first frame update
    void Start()
    {
        _TankInit();
    }

    void Update()
    {
        _TurnTurrets();
    }
    protected void _TankInit()
    {
        _RigidBody = GetComponentInChildren<Rigidbody>();

        var Collider = GetComponent<Collider>();
        if (Collider)
        {
            var phyMat = new PhysicMaterial();
            phyMat.name = "My Phy Material";
            phyMat.staticFriction = 1.0f;
            phyMat.dynamicFriction = 1.0f;
            phyMat.bounciness = 0;
            phyMat.frictionCombine = PhysicMaterialCombine.Maximum;
            Collider.material = phyMat;
        }

    }

    private void _SetRidigBody()
    {
        if (!_RigidBody) return;
        _RigidBody.isKinematic = false;
        _RigidBody.useGravity = true;
        _RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void MoveTo(Vector3 TargetPos)
    {
        if (!_RigidBody) return;
        _SetRidigBody();
        var floorFinder = new FloorFinder(_RigidBody);
        var MoveVector = Vector3.ClampMagnitude(TargetPos - _RigidBody.transform.position, 1.0f);
        MoveVector.y = 0;

        Vector3 TrueMove = Vector3.ProjectOnPlane(MoveVector, floorFinder.FloorNormal);

        _RigidBody.MovePosition(_RigidBody.transform.position + (TrueMove * Mathf.Abs(MoveSpeed) * (Time.deltaTime * 5.0f)));

        if (MoveVector != Vector3.zero)
        {

            Quaternion TargetBodyRot = Quaternion.RotateTowards(
                _RigidBody.rotation,
                Quaternion.LookRotation(MoveVector, floorFinder.FloorNormal),
                TurnSpeed * (Time.deltaTime * 100)
            );

            TurnBodyTo(TargetPos);

            TurretAimControl.transform.position = AimPoint;
        }
    }

    public void AimAt(Vector3 Target)
    {
        AimPoint = Target;
        _TargetTurretQuat = Quaternion.LookRotation(AimPoint - _RigidBody.transform.position);
    }

    protected void _UpdateRig()
    {
        TurretAimControl.transform.position = AimPoint;
    }

    public void TurnBodyTo(Vector3 TargetPos)
    {
        var relVector = TargetPos - _RigidBody.transform.position;

        if (relVector != Vector3.zero)
        {
            var floorFinder = new FloorFinder(_RigidBody);
            Quaternion TargetBodyRot = Quaternion.RotateTowards(
                _RigidBody.rotation,
                Quaternion.LookRotation(relVector, floorFinder.FloorNormal),
                TurnSpeed * (Time.deltaTime * 100)
            );

            _RigidBody.MoveRotation(TargetBodyRot);
        }
    }

    protected void _TurnTurrets()
    {
        Quaternion TargetQuat = Quaternion.LookRotation(AimPoint - _RigidBody.transform.position);
        _TurretQuat = Quaternion.RotateTowards(_TurretQuat, TargetQuat, TurretTurnSpeed * (Time.deltaTime * 100.0f));

        _UpdateRig();
    }

    public Quaternion GetTurretAimDelta()
    {
        var TargetQuat = Quaternion.LookRotation(
            AimPoint - _RigidBody.transform.position
        );

        Quaternion c = TargetQuat * Quaternion.Inverse(_TurretQuat);

        return c;
    }


    public RigidBodyBounds GetRigidbodyBounds()
    {
        return new RigidBodyBounds(_RigidBody);
    }

    void OnDrawGizmos()
    {
        GetRigidbodyBounds().DrawDebugGizmos();
    }



}
