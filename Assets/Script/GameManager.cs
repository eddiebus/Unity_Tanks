using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Level
{
    public Level(){
        Refresh();
    }
    private Character _Player;
    private List<GameObject> _Enemies;

    public void Refresh()
    {
        var SearchList  = GetCharactersOfTag(CharacterNames.Player);
        if (SearchList.Count > 0){
            _Player = SearchList[0];
        }

        _Player = FindCharacterByTag(CharacterNames.Player);

        Debug.Log($"Hello LeveLManager | SearchList = {_Player.gameObject.name}");
    }

    public List<Character> GetCharactersOfTag(string targetTag)
    {
        List<Character> resultList = new List<Character>();
        var TaggedObj = GameObject.FindGameObjectsWithTag(Character.GameObjectTagName);

        foreach (var obj in TaggedObj)
        {
            var CharComp = obj.GetComponent<Character>();
            if (CharComp)
            {
                if (CharComp.CharacterTag == targetTag)
                {
                    resultList.Add(CharComp);
                }
            }
        }
        return resultList;
    }

    public Character FindCharacterByTag(string Target){
        var Characters =  UnityEngine.Object.FindObjectsByType<Character>(FindObjectsSortMode.None);
        foreach (var charac in Characters){
            if (charac.CharacterTag == Target){
                return charac;
            }
        }

        return null;
    }
}


public class GameManager
{
    public GameManager()
    {
        _Level = new Level();
    }
    private Level _Level;
    public Level Level
    {
        get
        {
            return _Level;
        }
    }

    private static GameManager _Instance;
    public  static GameManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameManager();
            }

            return _Instance;
        }
    }


}
