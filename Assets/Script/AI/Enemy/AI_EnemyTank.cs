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
    // Start is called before the first frame update
    void Start()
    {
        AIInit();
        _Tank = GetComponent<Tank>();
        _Sense = GetComponentInChildren<ObjectSense>();

        _Sense.OnStimuliUpdate += () =>
        {
            Debug.Log($"Hello Stimuli Action");
            _UpdateStimuli(_Sense);
        };


        _EnemyComp = GetComponent<Enemy>();
    }

    private void _UpdateStimuli(ObjectSense Source)
    {
        var Characters = Source.GetStimuliWithTag(StimuliTag.Character);

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

        if (Alert > 0){
            Alert -= Time.deltaTime / 10;
        }
    }

    protected override void HandleAICycle()
    {

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
        }
        else{
            // Move near to Player
            float maxDistance = 5.0f;
            var PlayerPos = _EnemyComp.Player.transform.position;

            NavMeshHit NavMeshPoint;
            NavMesh.SamplePosition(PlayerPos,out NavMeshPoint,20.0f,NavMesh.AllAreas);

            if (NavMeshPoint.hit){
                var MoveTask = new AI_TankMoveTo(_Tank, NavMeshPoint.position);
                ExecuteAITask(MoveTask);
            }
        }

        _Tank.AimAt(_Tank.transform.position + _Tank.transform.forward * 10);



    }
}
