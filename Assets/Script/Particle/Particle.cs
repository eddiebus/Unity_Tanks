using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [Range(0.1f,10.0f)]    
    public float StartVelocity;
    [Range(-1,1)]
    public  float Gravity;
    private Rigidbody _rigidbody;
    private Vector3 InitScale;

    [Range(0.5f,1)]
    public float LifeTime;
    protected float _CurrentLifeTime;
    // Start is called before the first frame update
    void Start()
    {
        _ParticleInit();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _UpdateBody();
        _Tick();
    }

    protected void _ParticleInit(){
        _CurrentLifeTime = LifeTime;
        InitScale = transform.localScale;
        _rigidbody = GetComponent<Rigidbody>();

        if (_rigidbody){
            _rigidbody.AddForce(
                transform.rotation * Vector3.forward * StartVelocity,
                ForceMode.VelocityChange
            );
        }
    }

    protected void _Tick(){
        _CurrentLifeTime -= Time.deltaTime;
        if (_CurrentLifeTime <= 0 ){
            GameObject.Destroy(gameObject);
        }
        else{
            var remainingTime = _CurrentLifeTime / LifeTime;
            transform.localScale = InitScale * remainingTime;
        }
    }

    protected void _UpdateBody(){
        if (!_rigidbody) return;
        else{
            _rigidbody.useGravity = false;
            _rigidbody.mass = 0.0001f;


            var GravityForce = Physics.gravity * Gravity * 100   * (_rigidbody.mass * _rigidbody.mass); 
            _rigidbody.AddForce(GravityForce,ForceMode.Impulse);
        }
    }
}
