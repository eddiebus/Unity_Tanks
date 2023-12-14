using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


public static class CharacterNames {
    public static string Player = "Player";
    public static string Enemy = "Enemy";
    public static string Other = "Other";
}

public class Character : MonoBehaviour
{
    public float Health = 1.0f;
    public string CharacterTag = "EmptyCharacter";


    public static string GameObjectTagName = "Character";
    // Start is called before the first frame update
    void Start()
    {
        tag = GameObjectTagName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float Ammount){
        Health -= Ammount;
    }

    public static List<Character> FindCharactersWithTag(string TargetTag){
        List<Character> returnList = new List<Character>();

        var searchList = GameObject.FindObjectsByType<Character>(FindObjectsSortMode.None);
        foreach (var obj in searchList){
           if (obj.CharacterTag == TargetTag){
                returnList.Add(obj);
           }
        }

        if (returnList.Count > 0){
        return returnList;
        }
        else{ 
            return null;
        }
    }

    public static Character FindCharacterWithTag(string TargetTag){
        var search = FindCharactersWithTag(TargetTag);
        if (search.Count > 0){
            return search[0];
        }
        else return null;
    }
}