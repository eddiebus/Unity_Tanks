using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private Character _PlayerObject;
    public Character Player
    {
        get
        {
            return Character.FindCharacterWithTag(CharacterNames.Player);
        }
    }
    public float EyeHeight = 0.5f;
    public float VisionDistance = 10.0f;
    public float LookRadius = 45.0f;
    public float PlayerVisionTime;

    public bool IsPlayerInView()
    {
        if (!Player)
        {
            return false;
        }

        Vector3 VectorToPlayer = Player.transform.position - transform.position;
        Vector3 forwardVector = transform.forward;


        Quaternion QuatToPlayer = Quaternion.LookRotation(VectorToPlayer, transform.up);
        float AngleToPlayer = Quaternion.Angle(transform.rotation, QuatToPlayer);

        if (VectorToPlayer.magnitude <= VisionDistance && AngleToPlayer < (LookRadius / 2.0f))
        {
            Ray TestRay = new Ray();
            TestRay.origin = transform.position;
            TestRay.origin += Vector3.up * EyeHeight;
            TestRay.direction = VectorToPlayer.normalized;
            // Check hits on default layer
            bool visionBlocked = Physics.Raycast(TestRay, VectorToPlayer.magnitude, 0 << 0);

            if (!visionBlocked)
            {
                return true;
            }
        }

        return false;
    }
    // Start is called before the first frame update
    void Start()
    {
        CharacterTag = CharacterNames.Enemy;
        Debug.Log($"Player Obj from Enemy = {Player.gameObject.name}");
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0){
            OnDestroy.Invoke();
            GameObject.Destroy(gameObject);
        }

        if (IsPlayerInView())
        {
            PlayerVisionTime += Time.deltaTime;
            if (PlayerVisionTime > 3.0f) PlayerVisionTime = 3.0f;
        }
        else
        {
            if (PlayerVisionTime > 0) PlayerVisionTime -= Time.deltaTime;
        }
    }
    void OnDrawGizmos()
    {
        DrawDebugGizmo();
    }

    protected void DrawDebugGizmo()
    {
    }
}
