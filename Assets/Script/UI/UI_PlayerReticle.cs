using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerReticle : MonoBehaviour
{
    public Player _Player;
    public RectTransform OuterCircle;
    public RectTransform InnerCircle;

    public Vector3 delta;
    void LateUpdate()
    {

        Vector2 aimPos = PlayerCon.GetController(0).AimPoint;
        Vector2 retPos = PlayerCon.GetController(0).AimPoint;
        retPos *= 0.5f;
        retPos.x *= Screen.width;
        retPos.y *= Screen.height;
        
        InnerCircle.anchoredPosition = retPos;
    }
}
