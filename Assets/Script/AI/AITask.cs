using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;


public enum AITaskState{
    Working,
    Completed,
    Failed
}
public class AITask
{
    public AITaskState State => _State;
    protected AITaskState _State = AITaskState.Working; 

    public AITask(){
    }

    
    public virtual void Update()
    {
        
    }

    public virtual void DrawDebugGizmo(){
        
    }
}


public class AITask_Wait: AITask{
    private float _TimeLeft;

    public AITask_Wait(float Time){
        _TimeLeft = Time;
    }

    public override void Update()
    {
        _TimeLeft -= Time.deltaTime;
        if (_TimeLeft <= 0){
            _State = AITaskState.Completed;
        } 
    }
}
