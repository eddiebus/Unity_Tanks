using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public enum PathNavStatus{
    Completed,
    Moving
}


public class PathNav : MonoBehaviour
{
    protected NavMeshPath _Path; // Path to be moved along
    protected int _PathIndex; //Index To be Walked To
    protected Vector3 _TargetPos; // Target End Position
    protected PathNavStatus _Status;

    public PathNavStatus Status => _Status;

    public void SetDestination(Vector3 newPos){
        _TargetPos = newPos;
        _PathIndex = 0;
        _Path = new NavMeshPath();
        _RefreshPath();
    }

    private void _RefreshPath(){
        _Status = PathNavStatus.Moving;

        NavMeshHit meshHit;
        NavMesh.SamplePosition(_TargetPos,out meshHit,10.0f,NavMesh.AllAreas);
        if (meshHit.hit){
            _Path = new NavMeshPath();
            bool ValidPath = NavMesh.CalculatePath(
                this.transform.position,
                meshHit.position,
                NavMesh.AllAreas,
                _Path);

            if (!ValidPath){
                _Status = PathNavStatus.Completed;
            }
        }
        else{
            // Can't Find ok point on navmesh
            _Status = PathNavStatus.Completed;
        }
    }
    public virtual void GetStatus(){
        
    }
}
