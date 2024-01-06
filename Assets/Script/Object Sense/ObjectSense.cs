using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[Serializable]
public class ObjectSenseEye
{
    public Transform Point;
    public Vector3 RotationOffset;
}

public sealed class ObjectSense : MonoBehaviour
{
    [Range(0.1f, 3.0f)]
    public float UpdateTime = 0.5f;
    public float EyeHeight = 0.5f;
    public float SightDistance;
    [Range(0.0f, 180.0f)]
    public float SightRadius;

    public float HearingRadius;

    private float _TimeTillUpdate;
    public List<ObjectStimuli> CurrentStimuli;

    public List<Transform> EyePoints;

    public List<ObjectSenseEye> Eyes;

    public Action OnStimuliUpdate = new Action(() => { });
    // Start is called before the first frame update
    void Start()
    {
        CurrentStimuli = new List<ObjectStimuli>();
        _TimeTillUpdate = UpdateTime;
    }

    // Update is called once per frame
    void Update()
    {
        _TimeTillUpdate -= Time.deltaTime;
        if (_TimeTillUpdate <= 0)
        {
            _UpdateStimuli();
            _TimeTillUpdate = UpdateTime;
        }

    }

    private Vector3 GetEyePoint()
    {
        return transform.position + (transform.up * EyeHeight);
    }

    public List<ObjectStimuli> GetStimuliWithTag(StimuliTag TargetTag)
    {
        List<ObjectStimuli> returnList = new List<ObjectStimuli>();

        foreach (var stim in CurrentStimuli)
        {
            if (stim.Tag == TargetTag)
            {
                returnList.Add(stim);
            }
        }

        return returnList;
    }

    public List<ObjectStimuli> GetAllStimuli()
    {
        List<ObjectStimuli> copyList = new List<ObjectStimuli>(CurrentStimuli);
        return copyList;
    }

    private void _UpdateStimuli()
    {
        CurrentStimuli.Clear();
        var NearbyObjs = Physics.OverlapBox(
        this.transform.position,
        Vector3.one * (SightDistance + HearingRadius)
        );

        foreach (var Collision in NearbyObjs)
        {
            var obj = Collision.gameObject;
            var Stimuli = obj.GetComponentInChildren<ObjectStimuli>();

            if (Stimuli)
            {
                switch (Stimuli.Type)
                {
                    case ObjectStimuliType.Sound:
                        {
                            var distanceToObj = (obj.transform.position - this.transform.position).magnitude;

                            if (distanceToObj <= HearingRadius)
                            {
                                CurrentStimuli.Add(Stimuli);
                            }
                            break;
                        }
                    case ObjectStimuliType.Vision:
                        {
                            List<Transform> eyePoints = new List<Transform>(EyePoints);


                            bool ToAdd = false;

                            //Loop eyePoints
                            // Check if Stimuli is visible under eye
                            foreach (var eye in Eyes)
                            {
                                var TrueEyeRotation = eye.Point.rotation * Quaternion.Euler(eye.RotationOffset);

                                var StimuliRelPos = Stimuli.transform.position - eye.Point.position;
                                var StimuliDistance = StimuliRelPos.magnitude;
                                var QuatToStimuli = Quaternion.LookRotation(StimuliRelPos);
                                var AngleToStimuli = Quaternion.Angle(TrueEyeRotation, QuatToStimuli);

                                Ray VisionRay = new Ray();
                                VisionRay.origin = eye.Point.position;
                                VisionRay.direction = StimuliRelPos.normalized;

                                bool Visible = !Physics.Raycast(VisionRay, StimuliDistance, 0 << 0);

                                //Is Look Rotation -> EyeAngle in range
                                if (
                                    StimuliDistance < SightDistance &&
                                    AngleToStimuli < SightRadius / 2.0f &&
                                    Visible)
                                {
                                    ToAdd = true;
                                }
                            }

                            if (ToAdd)
                            {
                                CurrentStimuli.Add(Stimuli);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }

    }

    void OnDrawGizmos()
    {
        GameObject SelectObj = Selection.activeGameObject;
        if (SelectObj == null) return;
        else
        if (SelectObj != this.gameObject && !SelectObj.transform.IsChildOf(this.gameObject.transform)) return;

        List<ObjectSenseEye> eyePoints = new List<ObjectSenseEye>(Eyes);
        if (EyePoints.Count == 0)
        {
            var defaultEye = new ObjectSenseEye();
            defaultEye.Point = transform;
            defaultEye.RotationOffset = Vector3.zero;
            eyePoints.Add(defaultEye);
        }

        foreach (var eye in Eyes)
        {
            var eyeRotation = eye.Point.rotation * Quaternion.Euler(eye.RotationOffset);

            Vector3 VisionForward = Vector3.forward * SightDistance;
            var VisionWidth = (Quaternion.AngleAxis(SightRadius / 2.0f, Vector3.up) * VisionForward).x;
            Vector3 LVisionPoint = eyeRotation * new Vector3(-VisionWidth, 0, SightDistance);
            Vector3 RVisionPoint = eyeRotation * new Vector3(VisionWidth, 0, SightDistance);

            LVisionPoint += eye.Point.position;
            RVisionPoint += eye.Point.position;

            Vector3[] DrawPoints =  {
            eye.Point.position,
            LVisionPoint,
            LVisionPoint,
            RVisionPoint,
            RVisionPoint,
            eye.Point.position
            };
            Gizmos.color = Color.yellow;
            Gizmos.DrawLineList(DrawPoints);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, HearingRadius);

        if (CurrentStimuli == null) return;
        foreach (var stimulus in CurrentStimuli)
        {
            if (stimulus.Type == ObjectStimuliType.Vision)
            {
                Debug.DrawLine(transform.position, stimulus.transform.position, Color.red);
            }
        }
    }
}
