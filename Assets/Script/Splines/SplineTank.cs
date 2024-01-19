using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SplineTank : MonoBehaviour
{
    public RailCamera railCamera;
    public SplineWheel splineWheel;
    public Transform AimRigControl;
    [Range(0.1f, 3.0f)]
    public float MoveSpeed = 1.0f;
    [Range(0.1f,5.0f)]
    public float TurretTurnSpeed;
    public Rigidbody _Rigidbody;

    private Quaternion _TurretQuaternion;
    public Vector3 Right => GetRightVector();

    // Start is called before the first frame update
    void Start()
    {
        splineWheel.OnMoved += _Update;
        _Rigidbody = GetComponent<Rigidbody>();
        _ResetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        _FixPosition();
        _TurnTurret();

        AimAt(railCamera.GetWorldAimPoint());
    }

    public void AimAt(Vector3 TargetPos){
        AimRigControl.position = TargetPos;
    }

    private void _SetRidigBody()
    {
        if (!_Rigidbody) return;
        _Rigidbody.isKinematic = false;
        _Rigidbody.useGravity = true;
        _Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    protected void _TurnTurret(){
        var TargetQuat = Quaternion.LookRotation( 
            AimRigControl.position - 
            _Rigidbody.transform.position );

        _TurretQuaternion = Quaternion.Lerp(_TurretQuaternion,TargetQuat,TurretTurnSpeed * Time.deltaTime);            
    }

    private void _HanldeMovement()
    {
        if (!splineWheel) return;
        else
        {
            var controller = PlayerCon.GetController(0);
            var realMove = GetRightVector() * controller.MoveVector.x * MoveSpeed * Time.deltaTime * 10;
            _Rigidbody.transform.position = _Rigidbody.position + realMove;

            transform.position += splineWheel.Velocity;
            var floorfinder = new FloorFinder(_Rigidbody);
            var MoveRotQuat = Quaternion.LookRotation(splineWheel.GetTangent(), floorfinder.FloorNormal);
            _Rigidbody.MoveRotation(MoveRotQuat);
        }
    }
    private void _Update(SplineWheel splineWheel)
    {
        _SetRidigBody();
        _HanldeMovement();
        _FixPosition();
    }

    public Vector3 GetRightVector()
    {
        if (!_Rigidbody || !splineWheel) return Vector3.right;
        else
        {
            var floorfinder = new FloorFinder(_Rigidbody);
            var rVector = Vector3.Cross( floorfinder.FloorNormal, splineWheel.GetTangent());
            return rVector;
        }
    }

    protected Vector3 GetProjectedPos()
    {
        if (!_Rigidbody || !splineWheel)
        {
            return transform.position;
        }
        var forwardVector = splineWheel.GetTangent();
        var forQuat = Quaternion.LookRotation(forwardVector, Vector3.up);

        var rVector = forQuat * Vector3.right;
        var projPos = Vector3.Project(_Rigidbody.transform.position - splineWheel.transform.position, rVector);
        projPos += splineWheel.transform.position;

        var newpos = _Rigidbody.transform.position;
        newpos.x = projPos.x;
        newpos.z = projPos.z;
        return newpos;
    }

    private void _FixPosition()
    {
        if (!_Rigidbody || !splineWheel) return;
        _Rigidbody.transform.position = GetProjectedPos();

        float wheelDistance = (_Rigidbody.transform.position - splineWheel.transform.position).magnitude;
        if (wheelDistance > 50.0f){
            _ResetPosition();
        }
    }

    private void _ResetPosition(){
        if (!_Rigidbody || !splineWheel) return;
        else{
            var rBounds = new RigidBodyBounds(_Rigidbody);
            var heightOffset = rBounds.Bounds.extents.y;

            _Rigidbody.transform.position = splineWheel.transform.position + Vector3.up * heightOffset;
        }
    }

    void OnDrawGizmos()
    {
        if (splineWheel && _Rigidbody)
        {
            var floorfinder = new FloorFinder(_Rigidbody);
            var forwardVector = splineWheel.GetTangent();
            var forQuat = Quaternion.LookRotation(forwardVector, Vector3.up);

            var lineLength = 2.0f;

            var rVector = forQuat * Vector3.right;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (GetRightVector() * lineLength));

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + (floorfinder.FloorNormal * lineLength));

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_Rigidbody.transform.position, _Rigidbody.transform.position + splineWheel.GetTangent());

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(splineWheel.transform.position, 0.25f);

            if (_Rigidbody)
            {
                Gizmos.color = Color.white;
                var bounds = new RigidBodyBounds(_Rigidbody);
                Gizmos.DrawWireCube(_Rigidbody.transform.position, bounds.Bounds.extents);
            }
        }

        if (railCamera ){
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(railCamera.GetWorldAimPoint(), 0.2f);
        }
    }
}
