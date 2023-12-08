using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AimAtPoint(Vector3 Target){
        Target.y = this.transform.position.y;

        Vector3 forwardVector = (Target - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(forwardVector);

        this.transform.rotation = lookRot;
    }
}
