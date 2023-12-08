using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float Health = 1.0f;
    public string CharacterTag = "EmptyCharacter";
    // Start is called before the first frame update

    void Start()
    {
        tag = "Character";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float Ammount){
        Health -= Ammount;
    }
}
