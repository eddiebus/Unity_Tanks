using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[ExecuteAlways]
public class BillboardObject : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void UpdateDirection(Camera TargetCamera){

        BillboardObject[] components = FindObjectsByType<BillboardObject>(FindObjectsInactive.Include,FindObjectsSortMode.None);

        foreach (var comp in components){
            comp.transform.rotation = Quaternion.LookRotation(
                TargetCamera.transform.position - 
                comp.transform.position
            );
        }

    }

    private void _FaceCamera(Camera cam){
        this.transform.rotation = Quaternion.LookRotation(
            cam.transform.position -
            transform.position
        );
    }
}
