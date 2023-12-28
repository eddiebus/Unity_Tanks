using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        int selectIndex = (int) Mathf.Round ( Random.Range(0.0f,(float)_SpawnedCharacter.Count) );
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
        foreach (var obj in CharacterPrefab){
            if (PrefabUtility.GetPrefabInstanceHandle(obj) == null){
                Debug.LogWarning($"Character Spawner ({name}) | Object in spawn list was not prefab. Obj Removed");
                CharacterPrefab.Remove(obj);
            }
            else{
                var charComp = obj.GetComponent<Character>();
                if (!charComp){
                    Debug.LogWarning($"Character Spawner ({name}) | Object in spawn list was not prefab. Obj Removed");
                    CharacterPrefab.Remove(obj);
                }
                else{
                    Debug.Log("Prefab OK");
                }
            }
        }
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
