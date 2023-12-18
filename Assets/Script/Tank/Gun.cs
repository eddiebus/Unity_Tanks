using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float FireDelay;  
    protected float _FireDelay;

    public GameObject BulletPrefab;
    public List<AudioClip> FireSfx;
    private AudioSource _AudioSource;
    // Start is called before the first frame update
    void Start()
    {
        _AudioSource = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        _ReloadTick();
        Debug.DrawRay(transform.position,transform.forward * 100,Color.red);
    }

    protected void _GunInit(){
        _FireDelay = FireDelay;

    }

    protected void _ReloadTick(){
        if (_FireDelay > 0.0f){
            _FireDelay -= Time.deltaTime;
        }
    }

    public virtual bool Fire(){
        if (!BulletPrefab) return false;
        if (_FireDelay <= 0 ){
            var newBullet = GameObject.Instantiate(BulletPrefab,this.transform.position,Quaternion.identity);
            var BulletComp = newBullet.GetComponent<Bullet>();
            BulletComp.SetDirection(this.transform.rotation);
            if (FireSfx.Count > 0){
                System.Random numGen = new System.Random();
                var selectIndex = (int) (numGen.NextDouble() * (FireSfx.Count - 1));

                AudioClip toPlay = FireSfx[selectIndex];
                _AudioSource.clip = toPlay;
                _AudioSource.volume = 0.5f;
                _AudioSource.Play();
            }
            _FireDelay = FireDelay;

            return true;
        }

        return false;
    }

    public virtual void FireAt(Vector3 Target){
        if (!BulletPrefab) return;
        if (_FireDelay <= 0 ){
            var newBullet = GameObject.Instantiate(BulletPrefab,this.transform.position,Quaternion.identity);
            var BulletComp = newBullet.GetComponent<Bullet>();
            BulletComp.SetDirection(Quaternion.LookRotation(
                Target - transform.position
            ));

            _FireDelay = FireDelay;
        }
    }

}
