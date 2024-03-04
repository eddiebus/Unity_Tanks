using UnityEngine;
using UnityEngine.Splines;

public class SplineTank : MonoBehaviour
{
    public SplineContainer _SplineContainer;
    public Transform AimRigControl;
    [Range(0.1f, 3.0f)]
    public float MoveSpeed = 1.0f;
    [Range(0.1f, 5.0f)]
    public float TurretTurnSpeed;
    protected Rigidbody _Rigidbody;
    protected float _TrackWidth;
    public Gun[] _Guns;


    protected int _SplineIndex;
    protected Quaternion _TurretQuaternion;
    protected float _SplineDistance;

    public int SplineIndex => _SplineIndex;
    public float SplineDistance => _SplineDistance;
    // Start is called before the first frame update
    void Start()
    {
        _TankInit();
    }

    // Update is called once per frame
    void Update()
    {
        _FixPosition();
        _TurnTurret();

    }

    protected void _TankInit()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        _Guns = GetComponentsInChildren<Gun>();
        _SplineIndex = 0;
    }

    public void SetSplinePosition(int SplineIndex,float distance){
        _SplineIndex = SplineIndex;
        _SplineDistance = distance;
    }
    public SplineContainer GetSplineContainer(){
        return _SplineContainer;
    }

    public Spline GetSpline()
    {
        if (_SplineContainer.Splines.Count  >= _SplineIndex)
        {
            return _SplineContainer.Splines[_SplineIndex];
        }
        else
        {
            return null;
        }
    }

    public void AimAt(Vector3 TargetPos)
    {
        AimRigControl.position = TargetPos;
    }

    protected void _SetRidigBody()
    {
        if (!_Rigidbody) return;
        _Rigidbody.isKinematic = false;
        _Rigidbody.useGravity = true;
        _Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public Vector3 GetSplineWorldPosition(){
        return (Vector3)SplineUtility.EvaluatePosition(
            GetSpline(),
            GetNormalSplinePosition()
        ) + _SplineContainer.transform.position;

    }

    public float GetNormalSplinePosition()
    {
        return SplineUtility.ConvertIndexUnit(
            GetSpline(),
            _SplineDistance,
            PathIndexUnit.Distance,
            PathIndexUnit.Normalized
        );
    }

    public Vector3 GetTangent()
    {
        return ((Vector3)SplineUtility.EvaluateTangent(
            GetSpline(),
            GetNormalSplinePosition()
        )).normalized;
    }

    public void Move(Vector2 moveVector)
    {
        // Move Along Spline
        Vector3 splineWorldPos = SplineUtility.EvaluatePosition(GetSpline(),
        GetNormalSplinePosition());
        _SplineDistance += MoveSpeed * Time.deltaTime * moveVector.y;

        float NextNormalPos = SplineUtility.ConvertIndexUnit(GetSpline(),
        _SplineDistance, PathIndexUnit.Distance, PathIndexUnit.Normalized);
        Vector3 NextWorldPos = SplineUtility.EvaluatePosition(GetSpline(),
        NextNormalPos);
        _Rigidbody.transform.position += NextWorldPos - splineWorldPos;


        // Move Along Track
        Vector3 rightMove = GetTankRightVector() * moveVector.x * Time.deltaTime;
        _Rigidbody.transform.position += rightMove;
        _Rigidbody.transform.position = GetProjectedPos();

        // Set Rotation
        var floorfinder = new RigidBodyFloor(_Rigidbody);
        _Rigidbody.transform.rotation = Quaternion.LookRotation(GetTangent(),floorfinder.FloorNormal);

    }

    protected void _TurnTurret()
    {
        var TargetQuat = Quaternion.LookRotation(
            AimRigControl.position -
            _Rigidbody.transform.position);

        _TurretQuaternion = Quaternion.Lerp(_TurretQuaternion, TargetQuat, TurretTurnSpeed * Time.deltaTime);
    }

    public Vector3 GetSplineRightVector(){
        return Vector3.Cross (GetTangent(),Vector3.up);
    }

    public Vector3 GetTankRightVector()
    {
        if (!_Rigidbody) return Vector3.right;
        else
        {
            var normalPos = SplineUtility.ConvertIndexUnit(GetSpline(),
            _SplineDistance, PathIndexUnit.Distance, PathIndexUnit.Normalized);
            var splineTangent = GetTangent();

            var floorfinder = new RigidBodyFloor(_Rigidbody);
            var rVector = Vector3.Cross(floorfinder.FloorNormal, splineTangent);
            return rVector;
        }
    }

    protected Vector3 GetProjectedPos()
    {
        if (!_Rigidbody)
        {
            return transform.position;
        }
        Vector3 splineworldPos = SplineUtility.EvaluatePosition(
            GetSpline(),
            GetNormalSplinePosition()
        );

        var forwardVector = GetTangent();
        var forQuat = Quaternion.LookRotation(forwardVector, Vector3.up);

        var rVector = forQuat * Vector3.right;
        var projPos = Vector3.Project(_Rigidbody.transform.position - splineworldPos, rVector);
        projPos += splineworldPos;

        var newpos = _Rigidbody.transform.position;
        newpos.x = projPos.x;
        newpos.z = projPos.z;
        return newpos;
    }

    // Fix Position to Stay on Spline
    protected void _FixPosition()
    {
        if (!_Rigidbody) return;
        _Rigidbody.transform.position = GetProjectedPos();

        Vector3 splineworldPos = GetSplineWorldPosition();

        float wheelDistance = (_Rigidbody.transform.position - splineworldPos).magnitude;
        if (wheelDistance > 50.0f)
        {
            _ResetPosition();
        }


        // Fix Position on Width
        var bodyBounds = new RigidBodyBounds(_Rigidbody);
        var maxTrack = (_TrackWidth * 0.5f) - (bodyBounds.Bounds.extents.magnitude * 2.0f);

        var localVector = _Rigidbody.transform.position - splineworldPos;
        var rightVector = Vector3.Cross(Vector3.up, GetTangent().normalized);

        var rightPoint = splineworldPos + (rightVector * maxTrack);
        var leftPoint = splineworldPos - (rightVector * maxTrack);
        var rightDot = Vector3.Dot(localVector, rightVector);


        if (rightDot > maxTrack)
        {
            var newPos = _Rigidbody.transform.position;
            newPos.x = rightPoint.x;
            newPos.z = rightPoint.z;

            _Rigidbody.transform.position = newPos;
        }
        else if (rightDot < -maxTrack){
            var newPos = _Rigidbody.transform.position;
            newPos.x = leftPoint.x;
            newPos.z = leftPoint.z;

            _Rigidbody.transform.position = newPos;
        }
    }

    // Resent Position Somewhat above spline
    // To be used when lost from track
    private void _ResetPosition()
    {
        if (!_Rigidbody) return;
        else
        {

            Vector3 splineworldPos = SplineUtility.EvaluatePosition(
            GetSpline(),
            GetNormalSplinePosition()
            );
            var rBounds = new RigidBodyBounds(_Rigidbody);
            var heightOffset = rBounds.Bounds.extents.y * 2.0f;
            _Rigidbody.transform.position = splineworldPos + Vector3.up * heightOffset;
        }
    }

    protected void SplineTankDrawGizmo(){
        if (_Rigidbody)
        {
            var floorfinder = new RigidBodyFloor(_Rigidbody);
            var lineLength = 1.0f;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (GetTankRightVector() * lineLength));

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + (floorfinder.FloorNormal * lineLength));

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_Rigidbody.transform.position, _Rigidbody.transform.position + GetTangent());



            if (_Rigidbody)
            {
                Gizmos.color = Color.white;
                var bounds = new RigidBodyBounds(_Rigidbody);
                Gizmos.DrawWireCube(_Rigidbody.transform.position, bounds.Bounds.extents);
            }

        }
    }
    void OnDrawGizmos()
    {
        if (_Rigidbody)
        {
            var floorfinder = new RigidBodyFloor(_Rigidbody);
            var lineLength = 1.0f;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (GetTankRightVector() * lineLength));

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + (floorfinder.FloorNormal * lineLength));

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_Rigidbody.transform.position, _Rigidbody.transform.position + GetTangent());



            if (_Rigidbody)
            {
                Gizmos.color = Color.white;
                var bounds = new RigidBodyBounds(_Rigidbody);
                Gizmos.DrawWireCube(_Rigidbody.transform.position, bounds.Bounds.extents);
            }

        }

    }
}
