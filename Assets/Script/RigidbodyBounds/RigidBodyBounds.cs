using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyBounds
{
    private Rigidbody _body;
    private Bounds _ResultBounds;
    public Bounds Bounds => _ResultBounds;
    public RigidBodyBounds(Rigidbody ownerBody){
        _body = ownerBody;
        _CalculateBounds();
    }

    private void _CalculateBounds(){
        _ResultBounds = new Bounds(Vector3.zero,Vector3.zero);
        if (!_body) return;
        var gameobject = _body.gameObject;
        if (!gameobject) return;
        Collider[] allColliders = gameobject.GetComponentsInChildren<Collider>();
        for (int i = 0; i < allColliders.Length; i++){
            if (i == 0){
                _ResultBounds = allColliders[i].bounds;
            }
            else{
                _ResultBounds.Encapsulate(allColliders[i].bounds);
            }
        }
    }

    // Get Bounds of rigibody
    // If successful. Size should be greater than zero
    public Bounds GetBounds(){
        return _ResultBounds;
    }

    public void DrawDebugGizmos(){
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_ResultBounds.center,_ResultBounds.size);
    }
}
