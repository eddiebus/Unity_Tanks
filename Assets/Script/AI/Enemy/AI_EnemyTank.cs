using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyTank: AI
{
    private Tank _Tank;
    // Start is called before the first frame update
    void Start()
    {
        AIInit();
        _Tank = GetComponent<Tank>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_Tank)return;
        AITick();
    }

    protected override void HandleAICycle()
    {
        var walkarea = 20.0f;
        var TargetPosition = this.transform.position + (Random.insideUnitSphere * walkarea);
        NavMeshHit NavMeshPoint;
        NavMesh.SamplePosition(TargetPosition,out NavMeshPoint,walkarea,NavMesh.AllAreas); 
        var MoveTask = new AI_TankMoveTo(_Tank,NavMeshPoint.position);
        ExecuteAITask(MoveTask);
    }
}
