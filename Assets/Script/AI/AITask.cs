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
    protected AITaskState _State; 

    public AITask(){
    }

    
    public virtual void Update()
    {
        
    }

    public virtual void DrawDebugGizmo(){
        
    }
}
