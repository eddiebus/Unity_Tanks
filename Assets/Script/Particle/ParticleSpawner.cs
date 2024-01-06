using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ParticleSpawner : Trigger
{
    public bool Active;
    public GameObject Particle;
    [Range(0.05f, 1.0f)]
    public float SpawnDelay;
    public float Radius;

    private float _TimeTillSpawn;
    private bool InitOk;
    // Start is called before the first frame update
    void Start()
    {
        if (PrefabUtility.GetPrefabInstanceHandle(Particle) == null)
        {
            InitOk = false;
            return;
        }
        else
        {
            var particleComp = Particle.GetComponent<Particle>();
            if (!particleComp)
            {
                InitOk = false;
                return;
            }

        }

        InitOk = true;

    }

    void Update()
    {
        if (!Active) return;
        Radius = Mathf.Abs(Radius);
        _TimeTillSpawn -= Time.deltaTime;
        if (_TimeTillSpawn <= 0)
        {
            SpawnParticle();
            _TimeTillSpawn = SpawnDelay;
        }
    }


    public override void Toggle(bool value){
        Active = value;
    }


    private void SpawnParticle()
    {
        if (Particle == null) return;
        else
        {
            var SpawnPos = transform.position;
            var offset = Random.insideUnitCircle * Radius;
            var OffsetVector  = new Vector3(
                offset.x,
                0,
                offset.y
            );

            // Apply rotation
            OffsetVector = transform.rotation * OffsetVector;
            SpawnPos += OffsetVector;

            Quaternion SpawnDirection = Quaternion.LookRotation(
                this.transform.rotation * Vector3.up
            );
            GameObject.Instantiate(Particle, SpawnPos,SpawnDirection,transform);
        }
    }
    
    void OnDrawGizmos(){
        Gizmos.color = Color.grey;

        Gizmos.DrawWireSphere(transform.position,Radius);


        Debug.DrawLine(
            transform.position, 
            transform.position + transform.rotation * Vector3.up,
            Color.red
        );

        }
}
