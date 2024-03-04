using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerTank : Tank
{
    public GameCamera _Camera;

    // Start is called before the first frame update
    void Start()
    {
        _TankInit();
    }


    void Update(){
        _HandleAim();
        _TurnTurrets();
        AimAt(_Camera.GetWorldAimPoint());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _HandleMoveMent();   
    }

    private void _HandleMoveMent(){
        var controller = PlayerCon.GetController(0);
        var CamRotation = Quaternion.Euler( 
            0,
            _Camera.GetRotation().eulerAngles.y,
            0);
        
        var MoveVector = new Vector3 (
            controller.MoveVector.x,
            0,
            controller.MoveVector.y
        );

        MoveVector = CamRotation * MoveVector;
        Move(MoveVector);

    }

    private void _HandleAim(){
        if (_Camera){
            var cvontroller = PlayerCon.GetController(0);
            _Camera.Rotate(cvontroller.AimVector);
        }
    }


    void OnDrawGizmos(){
        DrawTankGizmos();
    }
}
