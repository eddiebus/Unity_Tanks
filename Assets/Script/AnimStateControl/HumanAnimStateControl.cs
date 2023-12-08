using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimStateControl 
{
    private Animator ParentAnimComp;
    public float Run = 0.0f;
    public float Move_Straight = 0.0f;
    public float Move_Side = 0.0f;


    public HumanAnimStateControl(Animator parent){
        if (parent == null ){
            Debug.LogWarning("Animation State Control ConstructorP: Null Animator Component Given.");
        }
        ParentAnimComp = parent;
    }
    public void UpdateAnimator(){
        ParentAnimComp.SetFloat("Run",Run);
    }

    

}
