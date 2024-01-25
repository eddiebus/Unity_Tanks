using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyTank : EnemyAI
{
    public Tank _Tank;
    public Enemy _EnemyComp;

    private float Alert = 0.0f; // Alert from finding player
    private GameObject SpottedPlayer;

    public float FireRange = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        AIInit();
        _Tank = GetComponent<Tank>();
        _EnemyComp = GetComponent<Enemy>();
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
        else
        {
            SpottedPlayer = null;
        }

        if (_State == EnemyAIState.Fighting && SpottedPlayer)
        {
            _Tank.AimAt(SpottedPlayer.transform.position);
        }
    }

    private void _UpdateStimuli()
    {

    }

    protected override void HandleAICycle()
    {

    }

    void OnDrawGizmos()
    {
    }
}
