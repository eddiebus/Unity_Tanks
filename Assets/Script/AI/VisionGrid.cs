using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class VisionGridPoint
{
    public Vector3 Position;
    public bool Visible;

    public VisionGridPoint(Vector3 pos, bool visible)
    {
        Position = pos;
        Visible = visible;
    }
}

public class VisionGrid
{
    private float GridSize;
    private float CellPerRow;
    private float CellSize;

    private GameObject TargetObj;

    public List<VisionGridPoint> Points;


    public VisionGrid(GameObject targetObj, float gridSize = 30.0f, int cellsPerRow = 20)
    {
        TargetObj = targetObj;
        Points = new List<VisionGridPoint>();
        CellPerRow = cellsPerRow;
        GridSize = gridSize;

        UpdatePoints(TargetObj.transform.position);

    }

    public void UpdatePoints(Vector3 OriginPoint)
    {

        Points.Clear();
        float posOffset = GridSize / (float)CellPerRow;

        CellSize = posOffset * 0.2f;

        for (int z = 0; z < CellPerRow; z++)
        {
            for (int x = 0; x < CellPerRow; x++)
            {
                float xPos = x * posOffset;
                float zPos = z * posOffset;
                Vector3 cellSamplePos = new Vector3(xPos, 0.0f, zPos);
                cellSamplePos -= new Vector3(GridSize / 2.0f, 0, GridSize / 2.0f);
                cellSamplePos += OriginPoint;

                NavMeshHit NavMeshPoint;
                bool truePos = NavMesh.SamplePosition(cellSamplePos, out NavMeshPoint, 0.2f, NavMesh.AllAreas);

                
                if (truePos)
                {
                    bool vision = false;

                    Vector3 dirToObj = (TargetObj.transform.position - NavMeshPoint.position).normalized;
                    float disToObj = (TargetObj.transform.position - NavMeshPoint.position).magnitude;


                    vision = !Physics.Raycast(NavMeshPoint.position,dirToObj,disToObj * 1.0f,1 << 0 );

                    Points.Add(new VisionGridPoint(NavMeshPoint.position, vision));
                }

            }
        }
    }


    public void DrawDebugGizmo()
    {
        for (int i = 0; i < Points.Count; i++)
        {
            if (Points[i].Visible){
                Gizmos.color = new Color(1.0f, 0, 0, 0.5f);
            }
            else{
                Gizmos.color = new Color(0, 0, 1.0f, 0.5f);
            }
            Gizmos.DrawSphere(Points[i].Position, CellSize);
        }

    }

}
