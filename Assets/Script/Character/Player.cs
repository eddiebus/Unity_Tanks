using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public static Player InstanceObj  {
        get {
            return GameObject.FindFirstObjectByType<Player>();
        }
    }
    public static Player[] AllPlayers {
        get {
            return GameObject.FindObjectsByType<Player>(FindObjectsInactive.Include,FindObjectsSortMode.InstanceID);
        }
    }
    public List<Character> EnemyCharacters;
    // Start is called before the first frame update
    void Start()
    {
        EnemyCharacters = new List<Character>();
        CharacterTag = CharacterNames.Player;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyCharacters = Character.FindCharactersWithTag(CharacterNames.Enemy);
    }
}
