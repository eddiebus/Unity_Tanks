using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public virtual void Toggle(bool on){

    }

    public void ToggleChildTriggers(bool on){
        var triggers = GetComponentsInChildren<Trigger>();

        foreach (var trigger in triggers){
            trigger.Toggle(on);
        }
    }
}
