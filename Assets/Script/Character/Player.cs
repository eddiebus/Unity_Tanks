using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public static Player InstanceObj
    {
        get
        {
            return GameObject.FindFirstObjectByType<Player>();
        }
    }
    public static Player[] AllPlayers
    {
        get
        {
            return GameObject.FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
        }
    }
    public Enemy[] NearbyEnemy;
    [Range(0.1f, 1.5f)]
    public float EnemyStaminaTime = 0.5f; // Time till local Enemies Gain Stamina
    private float _TimeTillEnemyStaminaGive = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        CharacterTag = CharacterNames.Player;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        NearbyEnemy =  GetEnemiesInRange(30.0f);
    }

    public Enemy[] GetEnemiesInRange(float MaxDistance)
    {
        List<Enemy> resultEnemies = new List<Enemy>();
        var allEnemies = Character.FindCharactersWithTag(CharacterNames.Enemy);

        foreach (var enmy in allEnemies)
        {
            var distance = (enmy.transform.position - transform.position).magnitude;
            if (distance < MaxDistance)
            {
                resultEnemies.Add((Enemy)enmy);
            }
        }

        if (resultEnemies.Count > 0)
        {
            return resultEnemies.ToArray();
        }
        else
        {
            return null;
        }
    }
}
