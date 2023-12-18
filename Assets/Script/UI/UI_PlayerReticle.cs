using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_PlayerReticle : MonoBehaviour
{
    public Player _Player;
    public RectTransform OuterCircle;
    public RectTransform InnerCircle;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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
                var screenSize = new Vector3(
                    Screen.width,
                    Screen.height,
                    0
                );
                var screenAimPoint =  TankComp._GameCamera._CamComp.WorldToScreenPoint(TankComp.GetCurrentAimPoint());
                var widgetPos = screenAimPoint - (screenSize/2);
                Debug.Log($"Screen Pos = {screenAimPoint} | {screenSize}| {widgetPos}");
                InnerCircle.anchoredPosition = widgetPos;
            }
        }

    }
}