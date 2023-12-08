using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavPathDebugDraw 
{
    NavMeshPath _path;

    public NavPathDebugDraw(NavMeshPath path){
        _path = path;
    }

    public void DebugDrawPath(){
        for (int i = 0; i < _path.corners.Length -1; i++){
            Vector3 Start = _path.corners[i];
            Vector3 End = _path.corners[i + 1];

            Debug.DrawLine(Start,End,Color.red,0.5f,true);
        }
    }
}
