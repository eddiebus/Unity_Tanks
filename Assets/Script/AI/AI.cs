using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AI : MonoBehaviour
{
    private float ThinkTime = 0.5f;
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

    protected virtual void HandleAICycle()
    {

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
