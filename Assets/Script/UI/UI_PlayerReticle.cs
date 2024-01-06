using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_PlayerReticle : MonoBehaviour
{
    public Player _Player;
    public RectTransform OuterCircle;
    public RectTransform InnerCircle;

    public Vector3 delta;
    void LateUpdate()
    {
        if (!_Player)
        {
            _Player = Player.InstanceObj;
        }
        else
        {
            var TankComp = _Player.gameObject.GetComponent<PlayerTank>();
            if (TankComp && InnerCircle)
            {
                //AimDelta /= 360.0f;

                var screenSize = new Vector3(
                    Screen.width,
                    Screen.height,
                    0
                );
                Vector3 screenAimPoint = TankComp._GameCamera.Camera.WorldToScreenPoint(TankComp.GetCurrentAimPoint());
                Vector3 widgetPos = screenAimPoint - (screenSize / 2);


                InnerCircle.anchoredPosition = Vector3.zero + 
                (TankComp.GetTurretAimDelta() *  Vector3.back) * 100;

            }
        }

    }
}
