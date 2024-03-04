using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Splines;

public class SplineWheel : MonoBehaviour
{
    public float Speed = 0;
    public float _DistanceCovered;
    private Vector3 _Velocity;
    public Vector3 Velocity => _Velocity;
    public SplineContainer SplineContainer;
    private int _CurrentSpline;

    public Action<SplineWheel> OnMoved = new Action<SplineWheel>((SplineWheel self) => { });

    // Start is called before the first frame update
    void Start()
    {
        if (!SplineContainer) return;
        transform.position = (Vector3)SplineUtility.EvaluatePosition(SplineContainer.Spline, GetNormalisedPosition()) + SplineContainer.transform.position;
        _CurrentSpline = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float trueSpeed = Speed * Time.deltaTime;
        _DistanceCovered += trueSpeed;

        if (_DistanceCovered > SplineContainer.Splines[_CurrentSpline].GetLength())
        {
            _DistanceCovered = SplineContainer.Splines[_CurrentSpline].GetLength();
        }
        else if (_DistanceCovered < 0)
        {
            _DistanceCovered = 0.0f;
        }
        Vector3 currentPos = transform.position;

        Vector3 newPos = (Vector3)SplineUtility.EvaluatePosition(
            SplineContainer.Splines[_CurrentSpline],
             GetNormalisedPosition()) + SplineContainer.transform.position;
        Vector3 MoveDelta = newPos - currentPos;


        _Velocity = MoveDelta;
        if (MoveDelta.magnitude > 0)
        {
            OnMoved.Invoke(this);
        }
        this.transform.position = newPos;
        transform.rotation = Quaternion.LookRotation(GetTangent());

    }

    public Spline GetCurrentSpline()
    {
        return SplineContainer.Splines[_CurrentSpline];
    }

    public float GetNormalisedPosition()
    {
        if (!SplineContainer) return 0.0f;
        else
        {
            return SplineUtility.ConvertIndexUnit(
                GetCurrentSpline(),
                _DistanceCovered,
                PathIndexUnit.Distance,
                PathIndexUnit.Normalized
                );
        }
    }

    public Vector3 GetTangent()
    {
        if (!SplineContainer) return Vector3.forward;
        else
        {
            float sampleFloat = GetNormalisedPosition();
            if (sampleFloat == 0)
            {
                sampleFloat = 0.001f;
            }

            Vector3 tangent = SplineUtility.EvaluateTangent(
                SplineContainer.Splines[_CurrentSpline],
                sampleFloat);
            if (tangent.magnitude > 0)
            {
                return tangent.normalized;
            }
            else
            {
                return Vector3.forward;
            }
        }
    }

    void OnDrawGizmos()
    {
        var spline = SplineContainer.Splines.ElementAt(0);

        var start = spline.Knots.ElementAt(0);
        Vector3 startPos = (Vector3)start.Position + SplineContainer.transform.position;
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(startPos, 1.0f);

        Gizmos.color = Color.yellow;
        Vector3 tangent = GetTangent();
        var renderpos = transform.position + Vector3.up;
        Gizmos.DrawLine(renderpos, renderpos + (tangent * 1.5f));

        Gizmos.color = Color.white;

        var knot = (int)SplineUtility.ConvertIndexUnit(
            SplineContainer[_CurrentSpline],
            _DistanceCovered,
            PathIndexUnit.Distance,
            PathIndexUnit.Knot
            );
        var knotPos = SplineContainer.transform.position + (Vector3)SplineContainer[_CurrentSpline][knot].Position;
        Gizmos.DrawWireSphere(knotPos, 0.5f);
    }
}
