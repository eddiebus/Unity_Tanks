using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailCamera : MonoBehaviour
{
    public Transform ParentObj;
    public LayerMask SightMask;
    public Camera _CameraComponent;
    // Start is called before the first frame update
    void Start()
    {
        _CameraComponent = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector3 GetWorldAimPoint()
    {
        var controller = PlayerCon.GetController(0);
        var AimPoint = controller.AimPoint / 2 + Vector2.one * 0.5f;

        var worldAimPoint = _CameraComponent.ViewportToWorldPoint(
            new Vector3(
                AimPoint.x,
                AimPoint.y,
                _CameraComponent.farClipPlane
            )
        );

        Ray scanRay = new Ray(
            _CameraComponent.transform.position,
            worldAimPoint - _CameraComponent.transform.position
        );

        RaycastHit[] RayHits = Physics.RaycastAll(
            scanRay,
            _CameraComponent.farClipPlane,
            SightMask,
            QueryTriggerInteraction.Ignore
        );

        List<RaycastHit> HitList = new List<RaycastHit>();

        // Search hits ignore hits that involve parent
        if (ParentObj)
        {
            foreach (var hit in RayHits)
            {
                var transform = hit.collider.transform;
                if (transform != ParentObj && !transform.IsChildOf(ParentObj))
                {
                    HitList.Add(hit);
                }
            }
        }
        Debug.Log($"Hit List = {HitList.Count} | {RayHits.Length}");

        // No Hits used default point
        if (HitList.Count == 0)
        {
            return worldAimPoint;
        }
        // One or more points
        // Look for closest point
        else
        {
            RaycastHit ClosestPoint = HitList[0];
            foreach (var hit in HitList)
            {
                if (hit.distance < ClosestPoint.distance)
                    ClosestPoint = hit;
            }
            return ClosestPoint.point;
        }
    }
}
