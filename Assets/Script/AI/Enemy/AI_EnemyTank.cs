using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyTank : AI
{
    public Tank _Tank;
    public ObjectSense _Sense;
    public Enemy _EnemyComp;

    private float Alert = 0.0f;
    private GameObject SpottedPlayer;

    public float FireRange = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        AIInit();
        _Tank = GetComponent<Tank>();
        _Sense = GetComponentInChildren<ObjectSense>();
        _EnemyComp = GetComponent<Enemy>();
    }

    private void _UpdateStimuli()
    {
        if (!_Sense) return;
        var Characters = _Sense.GetStimuliWithTag(StimuliTag.Character);

        foreach (var charobj in Characters)
        {
            var characterComp = charobj.gameObject.GetComponent<Character>();

            if (characterComp)
            {
                if (characterComp.CharacterTag == CharacterNames.Player)
                {
                    SpottedPlayer = charobj.gameObject;
                    Alert = 1.0f;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_Tank) return;
        AITick();

        if (Alert > 0)
        {
            Alert -= Time.deltaTime / 10;
        }
        
    }

    protected override void HandleAICycle()
    {
        _UpdateStimuli();
        if (Alert <= 0)
        {
            var walkarea = 20.0f;
            var TargetPosition = this.transform.position + (Random.insideUnitSphere * walkarea);
            NavMeshHit NavMeshPoint;
            NavMesh.SamplePosition(TargetPosition, out NavMeshPoint, walkarea, NavMesh.AllAreas);
            if (NavMeshPoint.hit)
            {
                var MoveTask = new AI_TankMoveTo(_Tank, NavMeshPoint.position);
                ExecuteAITask(MoveTask);
            }

            _Tank.AimAt(_Tank.transform.position + _Tank.transform.forward * 10);
        }
        else
        {
            var PlayerPos = _EnemyComp.Player.transform.position;
            var VectorToPlayer = _EnemyComp.Player.transform.position - this.transform.position;
            var DistanceToPlayer = VectorToPlayer.magnitude;

            if (DistanceToPlayer > FireRange)
            {
                var DirToSelf = -VectorToPlayer.normalized;
                var TargetPos = PlayerPos + (DirToSelf * FireRange);
                NavMeshHit NavMeshPoint;
                NavMesh.SamplePosition(TargetPos, out NavMeshPoint, 20.0f, NavMesh.AllAreas);

                if (NavMeshPoint.hit)
                {
                    var MoveTask = new AI_TankMoveTo(_Tank, NavMeshPoint.position);
                    ExecuteAITask(MoveTask);
                }
            }

            _Tank.AimAt(PlayerPos);
        }


    }
}
