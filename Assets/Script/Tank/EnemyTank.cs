using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.Splines;

public class EnemyTank : SplineTank
{
    public Vector3 _PlayerRelativePos; //Position Relative to Player
    private PlayerTank _PlayerTank;
    // Start is called before the first frame update
    void Start()
    {
        _TankInit();
        _PlayerTank = PlayerTank.GetInstance();
        _SplineContainer = _PlayerTank.GetSplineContainer();
        _SplineIndex = _PlayerTank.SplineIndex;
    }

    // Update is called once per frame
    void Update()
    {
        _PlayerTank = PlayerTank.GetInstance();
        _TrackWidth = _PlayerTank.GetTrackWidth();
        _SetPosition();
        _SetRidigBody();
    }

    public float GetLocalTrackXPosition(){
        var splineWorldPos = GetSplineWorldPosition();
        var localPosition  = _Rigidbody.transform.position - splineWorldPos;

        var trackRightVector = GetTankRightVector() * _TrackWidth;
        
        return Vector3.Dot(trackRightVector,localPosition);
    }

    private void _SetPosition(){
        _SplineDistance = _PlayerTank.SplineDistance + 5;
        _FixPosition();
    }

    void OnDrawGizmos(){
        var targetSplinePos = (Vector3)SplineUtility.EvaluatePosition(
            GetSpline(),GetNormalSplinePosition()) + _SplineContainer.transform.position;

        targetSplinePos += (GetTankRightVector() * 0.5f ) / _TrackWidth;

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(targetSplinePos,1.0f);
    }
}
