using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public bool Active;
    public uint SpawnLimit;
    [Range(0.2f,10.0f)]
    public float SpawnDelay;
    private float _TimeTillSpawn;

    public bool LimitReached => _LimitReached;
    private bool _LimitReached = false;


    public List<Character> SpawnedCharacter {
        get {
            return new List<Character>(_SpawnedCharacter);
        }
    }
    public List<Character> _SpawnedCharacter;


    public List<GameObject> CharacterPrefab;

    public void Toggle(bool On){
        Active = On;
    }

    private void SpawnChar(){
        if (CharacterPrefab.Count == 0) return;
        int selectIndex = (int) Mathf.Round ( Random.Range(0.0f,CharacterPrefab.Count - 1) );
        var newObj = GameObject.Instantiate(CharacterPrefab[selectIndex],this.transform.position,Quaternion.identity);
        _TimeTillSpawn = SpawnDelay;

        var charComp = newObj.GetComponent<Character>();
        _SpawnedCharacter.Add(charComp);
        charComp.OnDestroy.AddListener(
            () => {
                _SpawnedCharacter.Remove(charComp);
            }
        );


    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (!Active ) return;
        else if (SpawnLimit > 0 && _SpawnedCharacter.Count >= SpawnLimit){
            Active = false;
            _LimitReached = true;
            return;
        }

        if (_TimeTillSpawn > 0){
            _TimeTillSpawn -= Time.deltaTime;
        }
        else {
            SpawnChar();
        }
    }
}
