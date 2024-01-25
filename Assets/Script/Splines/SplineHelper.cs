using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;


public struct KnotTransform{
    public Vector3 LocalPosition; 
    public Vector3 WorldPosition;
    public Quaternion Rotation; 
}

public class SplineHelper 
{
    public static KnotTransform GetKnotTransform(SplineContainer container, SplineKnotIndex KnotIndex){
        KnotTransform returnTransform = new KnotTransform(); 
        var Knot = container.Splines[KnotIndex.Spline][KnotIndex.Knot];

        returnTransform.LocalPosition = Knot.Position;
        returnTransform.WorldPosition = (Vector3)Knot.Position + container.transform.position;
        var knotRot = Knot.Rotation.value;
        returnTransform.Rotation = new Quaternion(knotRot.x,knotRot.y,knotRot.z,knotRot.w);

        return returnTransform;        
    }
}
