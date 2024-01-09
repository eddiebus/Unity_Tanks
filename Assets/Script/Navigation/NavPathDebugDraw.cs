using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavPathDebugDraw
{
    NavMeshPath _path;

    public NavPathDebugDraw(NavMeshPath path)
    {
        _path = path;
    }

    public void DebugDrawPath()
    {
        for (int i = 0; i < _path.corners.Length; i++)
        {

            if (i + 1 <= _path.corners.Length - 1)
            {
                Vector3 Start = _path.corners[i];
                Vector3 End = _path.corners[i + 1];
                Debug.DrawLine(Start, End, Color.red, 1.5f, true);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_path.corners[i], 0.2f);
        }
    }
}
