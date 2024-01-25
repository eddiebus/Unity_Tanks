using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NavPoint{
    Vector3 Position;
    float   Distance;
    float   Score;
}


public class NavPointGrid 
{
    protected NavPoint[] _Points = new NavPoint[0]; 
    public NavPointGrid(){

    }

    protected void _GeneratePoints(){

    }


    public void DrawDebugGizmos(){}

}
