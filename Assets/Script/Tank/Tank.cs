using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;


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
    public Vector3 TankPosition => _TankRigidBody.position;
    public float MoveSpeed = 5.0f;
    public float TurnSpeed = 10.0f;
    public float TurretTurnSpeed = 1.0f;
    protected Rigidbody _TankRigidBody;
    protected Vector3 _RecentVelocity;


    private Vector3 AimPoint;
    protected Quaternion _TurretQuat = Quaternion.identity;
    protected Quaternion _TargetTurretQuat;

    // Obj turrets are rigged to aim at
    public GameObject TurretAimControl;

    public MultiAimConstraint[] Rigconstraints;

    // Start is called before the first frame update
    void Start()
    {
        _TankInit();
    }

    protected void _TankInit()
    {
        _TankRigidBody = GetComponentInChildren<Rigidbody>();

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

    void Update()
    {
        _TurnTurrets();
    }

    private void _SetRidigBody()
    {
        if (!_TankRigidBody) return;
        _TankRigidBody.isKinematic = false;
        _TankRigidBody.useGravity = true;
        _TankRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void Move(Vector3 Direction)
    {
        if (_TankRigidBody)
        {
            _SetRidigBody();
            var floorFinder = new RigidBodyFloor(_TankRigidBody);
            var MoveVector = Vector3.ClampMagnitude(Direction, 1.0f);

            // Project Vector on Movement Plane
            MoveVector = Vector3.ProjectOnPlane(MoveVector, floorFinder.FloorNormal);
            _TankRigidBody.MovePosition(_TankRigidBody.position + (MoveVector * Mathf.Abs(MoveSpeed) * (Time.deltaTime * 5.0f)));
            _RecentVelocity = MoveVector;

            if (MoveVector != Vector3.zero)
            {
                TurnBodyTo(_TankRigidBody.position + MoveVector);
                TurretAimControl.transform.position = AimPoint;
            }
        }
    }


    public void MoveTo(Vector3 TargetPos)
    {
        float TargetVelocity = MoveSpeed * Time.deltaTime;
        Vector3 DeltaVector = TargetPos - _TankRigidBody.position;
        Vector3 moveVector = Vector3.ClampMagnitude(DeltaVector / TargetVelocity, 1.0f);
        Move(moveVector);
        // if (!_TankRigidBody) return;
        // _SetRidigBody();
        // var floorFinder = new RigidBodyFloor(_TankRigidBody);
        // var MoveVector = Vector3.ClampMagnitude(TargetPos - _TankRigidBody.transform.position, 1.0f);
        // MoveVector.y = 0;

        // Vector3 TrueMove = Vector3.ProjectOnPlane(MoveVector, floorFinder.FloorNormal);

        // _TankRigidBody.MovePosition(_TankRigidBody.transform.position + (TrueMove * Mathf.Abs(MoveSpeed) * (Time.deltaTime * 5.0f)));

        // if (MoveVector != Vector3.zero)
        // {

        //     Quaternion TargetBodyRot = Quaternion.RotateTowards(
        //         _TankRigidBody.rotation,
        //         Quaternion.LookRotation(MoveVector, floorFinder.FloorNormal),
        //         TurnSpeed * (Time.deltaTime * 100)
        //     );

        //     TurnBodyTo(TargetPos);

        //     TurretAimControl.transform.position = AimPoint;
        // }
    }

    public void AimAt(Vector3 Target)
    {
        AimPoint = Target;
        _TargetTurretQuat = Quaternion.LookRotation(AimPoint - _TankRigidBody.transform.position);
    }

    protected void _UpdateRig()
    {
        TurretAimControl.transform.position = AimPoint;
    }

    public void TurnBodyTo(Vector3 TargetPos)
    {
        var relVector = TargetPos - _TankRigidBody.position;

        if (relVector != Vector3.zero)
        {
            var floorFinder = new RigidBodyFloor(_TankRigidBody);
            Quaternion TargetBodyRot = Quaternion.RotateTowards(
                _TankRigidBody.rotation,
                Quaternion.LookRotation(relVector, floorFinder.FloorNormal),
                TurnSpeed * (Time.deltaTime * 100)
            );

            _TankRigidBody.MoveRotation(TargetBodyRot);
        }
    }

    protected void _TurnTurrets()
    {
        Quaternion TargetQuat = Quaternion.LookRotation(AimPoint - _TankRigidBody.transform.position);
        _TurretQuat = Quaternion.RotateTowards(_TurretQuat, TargetQuat, TurretTurnSpeed * (Time.deltaTime * 100.0f));

        _UpdateRig();
    }

    public Quaternion GetTurretAimDelta()
    {
        var TargetQuat = Quaternion.LookRotation(
            AimPoint - _TankRigidBody.transform.position
        );

        Quaternion c = TargetQuat * Quaternion.Inverse(_TurretQuat);

        return c;
    }


    public RigidBodyBounds GetRigidbodyBounds()
    {
        return new RigidBodyBounds(_TankRigidBody);
    }

    protected void DrawTankGizmos()
    {
        if (_TankRigidBody)
        {
            GetRigidbodyBounds().DrawDebugGizmos();
            var floorFinder = new RigidBodyFloor(_TankRigidBody);
            floorFinder.DrawDebugGizmo();

            var bodyBounds = new RigidBodyBounds(_TankRigidBody);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(
            _TankRigidBody.position,
            _TankRigidBody.position + (_RecentVelocity * bodyBounds.Bounds.size.magnitude)
            );
        }
    }
    void OnDrawGizmos()
    {
        DrawTankGizmos();
    }



}
