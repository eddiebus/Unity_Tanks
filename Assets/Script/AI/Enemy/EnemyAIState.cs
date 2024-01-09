using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAIState {
    Searching,
    Fighting,
    Waiting
};


public class EnemyAI : AI{
    protected EnemyAIState _State;

    protected override void SetAIState(int NewState)
    {
        _State = (EnemyAIState) NewState;
    }
}