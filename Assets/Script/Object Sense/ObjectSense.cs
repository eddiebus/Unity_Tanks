using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;




public sealed class ObjectSense : MonoBehaviour
{
    [Range(0.1f, 3.0f)]
    public float UpdateTime = 0.5f;
    public float SightDistance;
    [Range(0.0f,180.0f)]
    public float SightRadius;

    public float HearingRadius;

    private float _TimeTillUpdate;
    public List<ObjectStimuli> CurrentStimuli;

    public Action OnStimuliUpdate;
    // Start is called before the first frame update
    void Start()
    {
        CurrentStimuli = new List<ObjectStimuli>();
        _TimeTillUpdate = UpdateTime;

        OnStimuliUpdate = new Action(() => { });
    }

    // Update is called once per frame
    void Update()
    {
        _TimeTillUpdate -= Time.deltaTime;
        if (_TimeTillUpdate <= 0)
        {
            _UpdateStimuli();
            _TimeTillUpdate = UpdateTime;
            OnStimuliUpdate.Invoke();
        }

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
        Vector3.one * 100.0f
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
                            var StimuliRelPos = Stimuli.transform.position - transform.position;
                            var StimuliDistance = StimuliRelPos.magnitude;
                            var QuatToStimuli = Quaternion.LookRotation(StimuliRelPos);
                            var AngleToStimuli = Quaternion.Angle(transform.rotation,QuatToStimuli);

                            // Stimuli is in range and Angle.
                            if (StimuliDistance < SightDistance && AngleToStimuli < SightRadius/2.0f)
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
    
        Vector3 VisionForward = Vector3.forward * SightDistance;
        var VisionWidth = (Quaternion.AngleAxis(SightRadius / 2.0f, Vector3.up) * VisionForward).x;
        Vector3 LVisionPoint = this.transform.rotation * new Vector3(-VisionWidth, 0, SightDistance);
        Vector3 RVisionPoint = this.transform.rotation * new Vector3(VisionWidth, 0, SightDistance);

        LVisionPoint += transform.position;
        RVisionPoint += transform.position;

        Vector3[] DrawPoints =  {
            transform.position,
            LVisionPoint,
            LVisionPoint,
            RVisionPoint,
            RVisionPoint,
            transform.position
        };
        Gizmos.color = Color.yellow;
        Gizmos.DrawLineList(DrawPoints);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, HearingRadius);


        foreach (var stimulus in CurrentStimuli)
        {
            if (stimulus.Type == ObjectStimuliType.Vision)
            {
                Debug.DrawLine(transform.position, stimulus.transform.position, Color.red);
            }
        }
    }
}
