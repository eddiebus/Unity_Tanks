using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class RigidBodyFloor
{
    private Rigidbody _Rigidbody;
    public Vector3 FloorNormal;
    public Vector3 FloorForward;
    public Vector3 FloorRight;
    public bool _OnFloor;

    public bool OnFloor => _OnFloor;

    public RigidBodyFloor(Rigidbody parent)
    {
        _Rigidbody = parent;
        _GetValue();
    }

    public void _GetValue()
    {

        Vector3 resultNormal = Vector3.up;
        var bodyTransform = _Rigidbody.gameObject.transform;
        var bodyBounds = new RigidBodyBounds(_Rigidbody);
        var _DownDisVector = bodyBounds.Bounds.extents.y * 1.2f;

        Ray HitRay = new Ray(
            bodyTransform.position,
            Vector3.down
        );

        RaycastHit[] Hits = Physics.RaycastAll(HitRay, _DownDisVector);

        List<RaycastHit> HitToList = new List<RaycastHit>();

        // Ignore hits on parent obj (self)
        foreach (var hit in Hits)
        {
            var colliderObj = hit.collider.gameObject;
            if (
                colliderObj != _Rigidbody.gameObject ||
                 !colliderObj.transform.IsChildOf(_Rigidbody.gameObject.transform)
                 )
            {
                HitToList.Add(hit);
            }
        }

        if (HitToList.Count > 0)
        {
            _OnFloor = true;
            RaycastHit cloestestHit = Hits[0];
            foreach (var hit in HitToList)
            {
                if (hit.distance < cloestestHit.distance)
                {
                    cloestestHit = hit;
                }
            }

            resultNormal = cloestestHit.normal;
        }
        else
        {
            _OnFloor = false;
        }

        FloorNormal = resultNormal;

        FloorForward = Vector3.Cross(FloorNormal,-Vector3.right);
        FloorRight = Vector3.Cross(FloorNormal,Vector3.forward);
    }

    public void DrawDebugGizmo(){

        var bodyBounds = new RigidBodyBounds(_Rigidbody);
        var lineLength = bodyBounds.Bounds.size.magnitude; 

        // Draw Normal Line
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_Rigidbody.position, _Rigidbody.position + FloorNormal * lineLength);

        // Draw floor Plane
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_Rigidbody.position, _Rigidbody.position + FloorForward * lineLength);
        Gizmos.DrawLine(_Rigidbody.position, _Rigidbody.position + FloorRight * lineLength);
        Gizmos.DrawLine(_Rigidbody.position, _Rigidbody.position - FloorForward * lineLength);
        Gizmos.DrawLine(_Rigidbody.position, _Rigidbody.position - FloorRight * lineLength);
    }
}
