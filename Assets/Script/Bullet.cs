using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    private Quaternion Direction = Quaternion.identity;
    public float LifeTime = 10.0f;
    public float Speed;
    public float DamageValue = 0.1f;
    protected Rigidbody _RigidBody;
    public List<string> FriendlyCharacters;
    // Start is called before the first frame update
    void Start()
    {
        _ConfigRigidBody();
    }

    void Update(){
        Move();
        Decay();
    }
    
    protected void _ConfigRigidBody(){
        var body = GetComponent<Rigidbody>();
        if (!body) body = this.AddComponent<Rigidbody>();
        _RigidBody = body;

        body.isKinematic = true;
    }

    protected void Decay(){ 
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0){
            GameObject.Destroy(this.gameObject);
        }
    }

    protected virtual void Move(){
        this.transform.position += (Direction * Vector3.forward) *  (Speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(Direction * Vector3.forward);
    }

    public void SetDirection(Quaternion newQuat){
        Direction = newQuat;
        transform.rotation = Quaternion.LookRotation(Direction * Vector3.forward);
    }



    void OnTriggerEnter(Collider other){

        var Character = other.gameObject.GetComponent<Character>();

        if (Character){
            if (!FriendlyCharacters.Contains(Character.CharacterTag)){
                Character.Damage(DamageValue);
                Debug.Log($"Damaged {Character.gameObject.name} for {DamageValue}");
            }
        }


        GameObject.Destroy(gameObject);
    }
}
