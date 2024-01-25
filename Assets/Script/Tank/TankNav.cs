using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyTankNav : MonoBehaviour
{
    public SplineTank _Tank;
    public PlayerTank _PlayerTank;

    public SplineContainer _SplineContainer;
    public Vector2 TargetRelativePosition;
    public Vector2 PlayerRelativePosition;

    // Start is called before the first frame update
    void Start()
    {
        _PlayerTank = PlayerTank.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void _UpdateRelativePosition(){

    }
}
