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
public class BattleArea : MonoBehaviour
{
    private uint _EnemyCount;
    private bool _BattleStart = false;
    public UnityEvent OnBattleStart;
    public UnityEvent OnBattleFinish;
    // Start is called before the first frame update
    void Start()
    {
        var childEnemy = GetComponentsInChildren<Enemy>();
        _EnemyCount = (uint)childEnemy.Length;
    }

    // Update is called once per frame
    void Update()
    {
    }


    protected void SetChildrenActive(bool value)
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_BattleStart)
        {
            var obj = other.gameObject;
            var PlayerComp = obj.GetComponent<Player>();

            // Did player enter battle area
            if (PlayerComp){
                OnBattleStart.Invoke();
            }
        }



    }


}
