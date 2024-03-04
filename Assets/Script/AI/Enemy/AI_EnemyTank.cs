using UnityEditor;
using UnityEngine;

public class AI_EnemyTank : EnemyAI
{
    public Tank _Tank;
    public TankPathNav _TankNav;
    public ObjectSense _Sense;
    public Enemy _EnemyComp;

    private float Alert = 0.0f; // Alert from finding player
    private GameObject SpottedPlayer;

    public float FireRange = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        AIInit();
        _Tank = GetComponent<Tank>();
        _Sense = GetComponentInChildren<ObjectSense>();
        _EnemyComp = GetComponent<Enemy>();
        _TankNav = GetComponent<TankPathNav>();
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
                    Alert = 1.5f;
                }
            }
        }

    }

    protected override void HandleAICycle()
    {
        _UpdateStimuli();

        if (Alert > 0)
        {
            SetAIState((int)EnemyAIState.Fighting);
        }
        else
        {
            SetAIState((int)EnemyAIState.Searching);
        }

        switch (_State)
        {
            case EnemyAIState.Searching:
                {
                    var walkarea = 10.0f;
                    var TargetPosition = this.transform.position + (Random.insideUnitSphere * walkarea);

                    if (_TankNav.Status == PathNavStatus.Completed)
                        _TankNav.SetDestination(TargetPosition);

                    var fowardPoint = transform.position + transform.forward * 10.0f;
                    _Tank.AimAt(fowardPoint);
                    break;
                }
            case EnemyAIState.Fighting:
                {
                    var PlayerPos = SpottedPlayer.transform.position;
                    var DistanceToPlayer = (PlayerPos - transform.position).magnitude;
                    var MinFireDistance = FireRange * 0.75f;
                    // Too far to fire
                    // Engage!
                    if (DistanceToPlayer > FireRange)
                    {
                        var EngagePos = PlayerPos + (Random.insideUnitSphere * (FireRange * 0.2f));
                        _TankNav.SetDestination(EngagePos);
                    }
                    // Too close back up
                    else if (DistanceToPlayer < MinFireDistance)
                    {
                        var DirectionToSelf = (transform.position - PlayerPos).normalized;
                        var TargetMovePos = PlayerPos + (DirectionToSelf * MinFireDistance);
                        _TankNav.SetDestination(TargetMovePos);
                    }

                    _Tank.AimAt(PlayerPos);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    void OnDrawGizmos()
    {
        var selectObj = Selection.activeGameObject;
        if (!selectObj) return;
        else if (
            selectObj == this.gameObject ||
            this.gameObject.transform.IsChildOf(selectObj.transform) ||
            selectObj.transform.IsChildOf(transform))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,
            FireRange);
        }
    }
}