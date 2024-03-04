using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    protected float _AttackStamina;
    public float AttackStamina => _AttackStamina;
    
    // Start is called before the first frame update
    void Start()
    {
        CharacterTag = CharacterNames.Enemy;
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0){
            this.Destroy();
        }
        _AttackStamina -= Time.deltaTime;
    }

    public void AddAttackStamina(float Time){
        _AttackStamina += Time;
        float limit = 3.0f;
        if (_AttackStamina > 3.0f){
            _AttackStamina = limit;
        }
    }

    void OnDrawGizmos()
    {
    }

}
