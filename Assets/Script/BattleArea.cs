using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/*
Enemy Spawn Class
Enabled Children Obj When Player Enters Area
When Enemies defeated disable blocking objects.
*/

public enum BattleAreaState
{
    Waiting,
    Battle,
    Finished
}

public class BattleArea : Trigger
{
    public AudioClip[] StartSounds;
    public AudioClip[] ClearSounds;
    private uint _EnemyCount;
    private BattleAreaState _State = BattleAreaState.Waiting;
    private bool _BattleStart = false;

    private CharacterSpawner[] EnemySpawner;
    private uint PlayersIn = 0;
    public UnityEvent OnBattleStart;
    public UnityEvent OnBattleFinish;
    // Start is called before the first frame update
    void Start()
    {
        EnemySpawner = GetComponentsInChildren<CharacterSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        // End Loop
        if (_State == BattleAreaState.Finished) return;

        if (_State == BattleAreaState.Battle)
        {
            int SpawnCount = EnemySpawner.Length;
            int LimitHit = 0;

            int EnemyCount = 0;
            foreach (var spawner in EnemySpawner)
            {
                if (spawner.LimitReached)
                    LimitHit += 1;

                EnemyCount += spawner.SpawnedCharacter.Count;
            }

            // All Enemies spawned and destroyed.
            if (LimitHit == SpawnCount && EnemyCount == 0)
            {
                _State = BattleAreaState.Finished;

                // Play Alarm Sound
                if (ClearSounds.Length > 0)
                {
                    int selectIndex = (int)Mathf.Round(
                         Random.Range(0, ClearSounds.Length - 1)
                    );

                    SoundFX.SpawnSound(
                        transform.position,
                        ClearSounds[selectIndex]
                    );
                }


                OnBattleFinish.Invoke();
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        var AllPlayers = Player.AllPlayers;

        if (_State == BattleAreaState.Waiting && AllPlayers.Length > 0)
        {
            var obj = other.gameObject;
            var PlayerComp = obj.GetComponent<Player>();

            // Did player enter battle area
            if (PlayerComp)
            {
                PlayersIn += 1;

                if (PlayersIn == AllPlayers.Length)
                {
                    _State = BattleAreaState.Battle;
                    OnBattleStart.Invoke();

                    foreach (var Spawner in EnemySpawner)
                    {
                        Spawner.Toggle(true);
                    }

                    // Play Alarm Sound
                    if (StartSounds.Length > 0)
                    {
                        int selectIndex = (int)Mathf.Round(
                             Random.Range(0, StartSounds.Length - 1)
                        );

                        SoundFX.SpawnSound(
                            transform.position,
                            StartSounds[selectIndex]
                        );
                    }
                    Debug.Log($"Battle Begin in area {gameObject.name}");
                }
            }
        }

    }


}
