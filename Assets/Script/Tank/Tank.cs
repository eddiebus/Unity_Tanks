using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class AI_TankMoveTo : AITask{
    private Tank _Caller;
    private Vector3 _TargetPosition;
    private NavMeshPath NavPath = new NavMeshPath();
    private float _TimeTaken;

    private int PathIndex = 0;

    public AI_TankMoveTo(Tank Caller, Vector3 TargetPosition){
        _Caller = Caller;
        _TargetPosition = TargetPosition;

        NavMesh.CalculatePath(_Caller.transform.position,_TargetPosition,
        NavMesh.AllAreas,NavPath);
    }

    public override void Update()
    {
        if (PathIndex < NavPath.corners.Length){
            _Caller.MoveTo(NavPath.corners[PathIndex]);
            
            var CurrentPos = _Caller.transform.position;
            CurrentPos.y = 0;

            var TargetPos = NavPath.corners[PathIndex];
            TargetPos.y = 0;

            var DistanceToPoint = (TargetPos - CurrentPos).magnitude;
            if (DistanceToPoint < 0.1f){
                PathIndex++;
            }
        }
        else{
            _State = AITaskState.Completed;
        }


        
    }

    public override void DrawDebugGizmo(){
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_Caller.gameObject.transform.position,0.5f);

        foreach (var corner in NavPath.corners){
            Gizmos.DrawSphere(corner,0.2f);
        }

        var pathdraw = new NavPathDebugDraw(NavPath);
        pathdraw.DebugDrawPath();
    }
}

public class Tank : Character
{

    public float MoveSpeed = 5.0f;
    public float TurnSpeed = 10.0f;
    public float TurretTurnSpeed = 1.0f;
    protected Rigidbody _RigidBody;

    private Vector3     AimPoint;
    private Quaternion _TurretQuat = Quaternion.identity;
    private Quaternion _TargetTurretQuat;

    // Obj turrets are rigged to aim at
    public GameObject TurretAimControl;

    // Start is called before the first frame update
    void Start()
    {
        _TankInit();
    }


    void Update(){
        _TurnTurrets();

        if (Health <= 0){
            GameObject.Destroy(gameObject);
        }
    }

    protected void _TankInit(){
        CharacterTag = "Tank";
        _RigidBody = GetComponent<Rigidbody>();

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

    public void MoveTo(Vector3 TargetPos){
        if (!_RigidBody) return;
        _SetRidigBody();
        var floorFinder = new FloorFinder(_RigidBody);
        var MoveVector  =  (TargetPos - this.transform.position).normalized;
        MoveVector.y =  0;

        Vector3 TrueMove = Vector3.ProjectOnPlane(MoveVector,floorFinder.FloorNormal);

        _RigidBody.MovePosition(this.transform.position + (TrueMove *  Mathf.Abs(MoveSpeed) * (Time.deltaTime * 5.0f)));

        Quaternion TargetBodyRot = Quaternion.RotateTowards(
            _RigidBody.rotation,
            Quaternion.LookRotation(MoveVector,floorFinder.FloorNormal),
            TurnSpeed * (Time.deltaTime * 100)
        );
        
        if (TrueMove.magnitude > 0){
            _RigidBody.MoveRotation(TargetBodyRot);
        }
    }

    public void AimAt(Vector3 Target){
        AimPoint = Target;
    }


    protected void _TurnTurrets(){
        Quaternion TargetQuat = Quaternion.LookRotation(AimPoint - transform.position);
        _TurretQuat = Quaternion.RotateTowards(_TurretQuat,TargetQuat,TurretTurnSpeed * (Time.deltaTime * 100.0f));

        var ControlPos = (_TurretQuat * Vector3.forward * (AimPoint - transform.position).magnitude) + this.transform.position;
        TurretAimControl.transform.position = ControlPos;
    }



}
