using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class FloorFinder
{
    private Rigidbody _Rigidbody;
    public Vector3 FloorNormal;
    public bool IsOnFloor;

    public FloorFinder(Rigidbody parent)
    {
        _Rigidbody = parent;
        Update();
    }

    public void Update()
    {

        Vector3 resultNormal = Vector3.up;
        var bodyTransform = _Rigidbody.gameObject.transform;
        var bodyBounds = new RigidBodyBounds(_Rigidbody);
        var _DownDisVector = bodyBounds.Bounds.extents.y * 1.5f;

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
            IsOnFloor = true;
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
            IsOnFloor = false;
        }

        FloorNormal = resultNormal;
    }

}
