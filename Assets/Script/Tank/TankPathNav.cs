using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPathNav : PathNav
{
    public Tank _OwnerTank;
    // Start is called before the first frame update
    void Start()
    {
        _OwnerTank = GetComponent<Tank>();
    }

    // Update is called once per frame
    void Update()
    {
        // Completed Moving to Target. Do Nothing
        if (_Status == PathNavStatus.Completed) return;

        if (_OwnerTank == null){
            Debug.LogWarning($"TankPathNav : Needs Link To Tank Component");
        }

        if (_Path.corners.Length == 0) {
            _Status = PathNavStatus.Completed;
            _PathIndex = 0;
        }
        else{
            Vector3 NextPoint = _Path.corners[_PathIndex];
            var TankBounds = _OwnerTank.GetRigidbodyBounds().Bounds;
            float ErrorFloat = 1.0f;
            TankBounds.Expand(ErrorFloat); //Grow Bound Box for margin of error

            //Bounds overlap point
            //Move to next point
            if (TankBounds.Contains(NextPoint)){
                _PathIndex++; 
            }
            else{
                _OwnerTank.MoveTo(NextPoint);
            }

            if (_PathIndex >= (_Path.corners.Length) ){
                _Status = PathNavStatus.Completed;
            }
        }
    }

    void OnDrawGizmos(){
        if (_Path == null) return;
        if (_Path.corners.Length > 0 && _Status == PathNavStatus.Moving){
            new NavPathDebugDraw(_Path).DebugDrawPath();
        }
    }
}
