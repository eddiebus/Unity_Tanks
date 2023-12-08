using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorFinder 
{
    private Rigidbody _Rigidbody;
    public  Vector3 FloorNormal;
    public bool IsOnFloor; 

    public FloorFinder(Rigidbody parent){
        _Rigidbody = parent;
        Update();
    }

    public void Update(){

        Vector3 resultNormal = Vector3.up;

        var bodyTransform = _Rigidbody.gameObject.transform;
        var _DownDisVector = _Rigidbody.ClosestPointOnBounds(bodyTransform.position + Vector3.down * 900.0f) - bodyTransform.position;

        Ray HitRay = new Ray(
            bodyTransform.position,
            Vector3.down
        );

        RaycastHit[] Hits = Physics.RaycastAll(HitRay);

        if (Hits.Length > 0){
            IsOnFloor = true;
            RaycastHit cloestestHit = Hits[0];

            foreach (var hit in Hits){
                if (hit.distance < cloestestHit.distance){
                    cloestestHit = hit;
                }
            }

            resultNormal = cloestestHit.normal;
        }
        else{
            IsOnFloor = false;
        }

        FloorNormal = resultNormal;
    }

}
