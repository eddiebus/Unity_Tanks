using UnityEngine;

public class SoundFX : MonoBehaviour
{
    public bool Loop;
    private AudioSource _SourceComp;
    // Start is called before the first frame update
    void Start()
    {
        _SourceComp = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Not Looping & Finished Playing. Destroy Object
        if (!Loop && _SourceComp.isPlaying == false){
            GameObject.Destroy(gameObject);
        }        
    }

    public static GameObject SpawnSound(Vector3 WorldPos, AudioClip Clip,float Volume = 0.5f,bool Loop = false){
        // No Clip Given. Do Nothing
        if (!Clip) return null;

        //Create Obj
        var newObj = new GameObject($"SFX : {Clip.name}");
        newObj.transform.position = WorldPos;

        var audioSource = newObj.AddComponent<AudioSource>();
        audioSource.clip = Clip;
        audioSource.loop = Loop;
        audioSource.Play();
        var SoundFXComp = newObj.AddComponent<SoundFX>();
        SoundFXComp.Loop = Loop;

        return newObj;
    }

    
}
