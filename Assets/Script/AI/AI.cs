using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AI : MonoBehaviour
{
    [Range(0.1f,1.0f)]
    public float ThinkTime = 0.5f;
    private float _TimeTillThink;
    private AITask _CurrentTask = null;

    protected void AIInit()
    {
        _TimeTillThink = ThinkTime;
    }

    protected void AITick()
    {
        
        if (_TimeTillThink > 0)
        {
            _TimeTillThink -= Time.deltaTime;
        }
        else{
            HandleAICycle();
            _TimeTillThink = ThinkTime;
        }

        if (_CurrentTask != null){
            if (_CurrentTask.State == AITaskState.Working){
                _CurrentTask.Update();
            }
        }

    }

    protected virtual void SetAIState(int NewState){
        throw new NotImplementedException("Set Function For State Not Implimented on AI Class");
    }

    public string GetCurrentStateName(){
        throw new NotImplementedException();
    }

    protected virtual void HandleAICycle()
    {
        throw new NotImplementedException("AI Cycle State Not Implimented on AI Class");
    } 

    protected void ExecuteAITask(AITask NewTask)
    {
        // Task Exists
        if (_CurrentTask != null)
        {
            //Task Completed or failed
            if (
                _CurrentTask.State == AITaskState.Completed ||
                _CurrentTask.State == AITaskState.Failed
            )
            {
                _CurrentTask = NewTask;
            }
        }
        //No Task. Execute new task
        else
        {
            _CurrentTask = NewTask;
        }

    }

    protected virtual void DrawDebugGizmo(){
        if (_CurrentTask != null){
            _CurrentTask.DrawDebugGizmo();
        }
    }

    void OnDrawGizmos(){
        DrawDebugGizmo();
    }
}
