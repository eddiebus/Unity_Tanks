using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager
{
    public GameManager()
    {
    }
    public  static bool Paused => _Paused;
    private static bool _Paused = false;

    private static GameManager _Instance;
    public  static GameManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameManager();
            }

            return _Instance;
        }
    }


}
