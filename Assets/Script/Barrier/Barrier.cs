using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barrier : Trigger
{

    public bool BarrierOn;

    private Collider[] _Colliders;
    private NavMeshObstacle navMeshObstacle;

    // Start is called before the first frame update
    void Start()
    {
        _Colliders = GetComponents<Collider>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        }

    // Update is called once per frame
    void Update()
    {
        foreach (var col in _Colliders){
            col.isTrigger = !BarrierOn;
        }

        navMeshObstacle.carving = BarrierOn;
    }

    public override void Toggle(bool On){
        BarrierOn = On;
    }
}
