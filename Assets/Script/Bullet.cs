using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private Quaternion Direction = Quaternion.identity;
    public LayerMask CollisionMask;
    public float LifeTime = 0.5f;
    public float Speed;
    public float DamageValue = 0.1f;
    protected Rigidbody _RigidBody;
    public List<string> FriendlyCharacters;

    public GameObject HitParticle;
    // Start is called before the first frame update
    void Start()
    {
        _ConfigRigidBody();
    }

    void Update()
    {
        Move();
        Decay();
    }

    protected void _ConfigRigidBody()
    {
        var body = GetComponent<Rigidbody>();
        if (!body) body = this.AddComponent<Rigidbody>();
        _RigidBody = body;

        body.isKinematic = true;
    }

    protected void Decay()
    {
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    protected virtual void Move()
    {
        this.transform.position += (Direction * Vector3.forward) * (Speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(Direction * Vector3.forward);
    }

    public void SetDirection(Quaternion newQuat)
    {
        Direction = newQuat;
        transform.rotation = Quaternion.LookRotation(Direction * Vector3.forward);
    }


    protected void SpawnHitParticle()
    {
        if (!HitParticle) return;
        else
        {
            var SpawnCount = (int)MathF.Round(UnityEngine.Random.Range(0.0f, 5.0f));
            for (int i = 0; i < SpawnCount; i++)
            {
                var direction = Quaternion.LookRotation(
                     UnityEngine.Random.insideUnitSphere.normalized
                     );

                GameObject.Instantiate(HitParticle, transform.position, direction);
            }
        }
    }



    void OnTriggerEnter(Collider other)
    {

        var Character = other.gameObject.GetComponent<Character>();
        if (other.isTrigger) return;

        if (Character)
        {
            if (!FriendlyCharacters.Contains(Character.CharacterTag))
            {
                Character.Damage(DamageValue);
                Debug.Log($"Damaged {Character.gameObject.name} for {DamageValue}");
                GameObject.Destroy(gameObject);
                SpawnHitParticle();
            }
        }
        else
        {
            if (((1 << other.gameObject.layer) & CollisionMask) != 0)
            {
                GameObject.Destroy(gameObject);
            }
        }

    }
}
