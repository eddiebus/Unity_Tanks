using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Splines;


public class PlayerSplineTank : SplineTank
{
    public Transform CameraJoint;
    public RailCamera railCamera;

    void Start()
    {
        _TankInit();
    }

    void Update()
    {
        _SetRidigBody();
        _FixPosition();
        _TurnTurret();
        _SetCameraJointPosition();
        AimAt(railCamera.GetWorldAimPoint());
        if (PlayerCon.GetController(0).Fire > 0)
        {
            _Guns[0].Fire();

        }

        _TrackWidth = GetTrackWidth();

        Move(
            new Vector2(PlayerCon.GetController(0).MoveVector.x, 1.0f)
            );
    }

    // Get 1st Instance of Component
    public static PlayerSplineTank GetInstance(){
        return GameObject.FindFirstObjectByType<PlayerSplineTank>();
    }

    private void _SetCameraJointPosition(){
        var rigidBounds = new RigidBodyBounds(_Rigidbody);
        Vector3 backVector = -GetTangent().normalized;

        Vector3 CamPos = GetSplineWorldPosition() + 
        (backVector * (rigidBounds.Bounds.extents.magnitude * 0.3f) ) +
        Vector3.up * 1.1f;

        CameraJoint.position = CamPos;
        CameraJoint.rotation = Quaternion.LookRotation(GetTangent());
    }

    // Get the horizontal space for movement
    public float GetTrackWidth()
    {
        if (!railCamera)
        {
            return 3.0f;
        }
        else
        {
            var myPosition = _Rigidbody.transform.position;
            var CamPos = railCamera.transform.position;
            myPosition.y = CamPos.y;

            var posDistance = (myPosition - CamPos).magnitude;

            Vector3[] ViewportPoints = {
                new Vector3(0.0f,0.5f,posDistance),
                new Vector3(1.0f,0.5f,posDistance)
            };

            Vector3[] WorldPoints = {
                railCamera._CameraComponent.ViewportToWorldPoint(
                    ViewportPoints[0]
                ),
                railCamera._CameraComponent.ViewportToWorldPoint(
                    ViewportPoints[1]
                )
            };

            return (WorldPoints[0] - WorldPoints[1]).magnitude * 1.5f;
        }
    }
}
