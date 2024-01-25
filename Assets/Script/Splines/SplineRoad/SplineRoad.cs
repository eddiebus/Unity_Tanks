using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;




[ExecuteInEditMode]
public class SplineRoad : MonoBehaviour
{
    public SplineContainer splineContainer;
    public float RoadWidth;
    private Mesh _Mesh; 


    // Update is called once per frame
    void Update()
    {
        ComponentCheck();
        _GenerateMesh();
    }

    private void ComponentCheck(){
        if (!GetComponent<SplineContainer>()) gameObject.AddComponent<SplineContainer>();
        else if (!GetComponent<MeshFilter>()) gameObject.AddComponent<MeshFilter>();
        else if (!GetComponent<MeshRenderer>()) gameObject.AddComponent<MeshRenderer>();

        if (!_Mesh) _Mesh = new Mesh();
        _Mesh.name = "Road Procederal Mesh";

        GetComponent<MeshFilter>().mesh = _Mesh;
        if (!splineContainer) splineContainer = GetComponent<SplineContainer>();
    }
    
    // Find All Intersections
    private SplineKnotIndex[] GetAllIntersectedKnots(){
        List<SplineKnotIndex> returnList = new List<SplineKnotIndex>();
        for (int splineI = 0; splineI < splineContainer.Splines.Count; splineI++){
            for (int knotI = 0; knotI < splineContainer.Splines[splineI].Count; knotI++ ){
                var links = GetLinksAtKnot(new SplineKnotIndex(splineI,knotI));
                if (links.Length >= 2){
                    returnList.Add(new SplineKnotIndex(splineI,knotI));
                }
            }
        }
        return returnList.ToArray();
    }

    private SplineKnotIndex[] GetAllRoadKnots(){
        List<SplineKnotIndex> returnList = new List<SplineKnotIndex>();
        var Intersections = GetAllIntersectedKnots();
        for (int splineI = 0; splineI < splineContainer.Splines.Count; splineI++){
            for (int knotI = 0; knotI < splineContainer.Splines[splineI].Count - 1; knotI++){
                var thisKnotIndex = new SplineKnotIndex(splineI,knotI);
                var nextknotIndex = new SplineKnotIndex(splineI,knotI + 1);

                if (!Intersections.Contains(nextknotIndex) && !Intersections.Contains(thisKnotIndex)){
                    returnList.Add(thisKnotIndex);
                }
                
            }
        }

        return returnList.ToArray();
    }

    private SplineKnotIndex[] GetLinksAtKnot(SplineKnotIndex knotIndex){
        return splineContainer.KnotLinkCollection.GetKnotLinks(knotIndex).ToArray();
    }

    private void _GenerateMesh(){
        _Mesh.Clear();
        List<Vector3> _MeshVertecies = new List<Vector3>();
        List<Vector2> _MeshUV = new List<Vector2>();
        List<int> _MeshTriIndex = new List<int>();

        // mesh points
        foreach (var roadKnot in GetAllRoadKnots()){  

            var knot = SplineHelper.GetKnotTransform(splineContainer,roadKnot);
            var nextKnot = SplineHelper.GetKnotTransform(splineContainer,new SplineKnotIndex(
                roadKnot.Spline,
                roadKnot.Knot + 1
            ));

            float height = 0.095f;
            Vector3[] LocalPoint = {
                new Vector3(-RoadWidth,height),
                new Vector3(RoadWidth,height)
            };

            Vector3[] StartPoint = {
                (knot.Rotation * LocalPoint[0]) + knot.LocalPosition,
                (knot.Rotation * LocalPoint[1]) + knot.LocalPosition,
            };

            Vector3[] EndPoint = {
                (nextKnot.Rotation * LocalPoint[0]) + nextKnot.LocalPosition,
                (nextKnot.Rotation * LocalPoint[1]) + nextKnot.LocalPosition,
            };


            Vector3[] MeshPoint = {
                StartPoint[0],
                StartPoint[1],
                EndPoint[0],
                EndPoint[1]
            };

            Vector2[] MeshUV = {
                new Vector2(0.0f,0.0f),
                new Vector2(0.0f,0.0f),
                new Vector2(0.0f,0.0f),
                new Vector2(0.0f,0.0f)
            };

            int[]  triIndex = {
                0, 2, 1,
                1, 2, 3
            };

            

            _MeshVertecies.AddRange(MeshPoint);
            _MeshUV.AddRange(MeshUV);

            

            int lastIndex = _MeshVertecies.Count - MeshPoint.Length;

            foreach (int index in triIndex){
                if (lastIndex <= 0){
                    _MeshTriIndex.Add(index);
                }
                else{
                    _MeshTriIndex.Add(index + lastIndex);
                }
            }
        
        }

        _Mesh.vertices = _MeshVertecies.ToArray();
        _Mesh.uv = _MeshUV.ToArray();
        _Mesh.triangles = _MeshTriIndex.ToArray();
    }
    void OnDrawGizmos(){

        float BallSize = RoadWidth * 0.5f;
        foreach (var knotIndex in GetAllIntersectedKnots() ){
            var knot = splineContainer.Splines[knotIndex.Spline][knotIndex.Knot];
            var knotWorldPos = (Vector3)knot.Position + splineContainer.transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(knotWorldPos,BallSize);
        }

        foreach (var knotIndex in GetAllRoadKnots() ){
            var knot = splineContainer.Splines[knotIndex.Spline][knotIndex.Knot];
            var knotWorldPos = (Vector3)knot.Position + splineContainer.transform.position;
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(knotWorldPos,BallSize * 0.8f);
        }

        // Mesh Road Points
        foreach (var roadKnot in GetAllRoadKnots()){            
            var knot = SplineHelper.GetKnotTransform(splineContainer,roadKnot);
            var nextKnot = SplineHelper.GetKnotTransform(splineContainer,new SplineKnotIndex(
                roadKnot.Spline,
                roadKnot.Knot + 1
            ));

            Vector3[] Points = {
                new Vector3(-RoadWidth,0.0f),
                new Vector3(RoadWidth,0)
            };

            Vector3[] StartPoint = {
                (knot.Rotation * Points[0]) + knot.LocalPosition,
                (knot.Rotation * Points[1]) + knot.LocalPosition,
            };

            Vector3[] EndPoint = {
                (nextKnot.Rotation * Points[0]) + nextKnot.LocalPosition,
                (nextKnot.Rotation * Points[1]) + nextKnot.LocalPosition,
            };


            List<Vector3> allPoints = new List<Vector3>();
            allPoints.AddRange(StartPoint);
            allPoints.AddRange(EndPoint);

            Gizmos.color = Color.black;
            foreach(var point in allPoints.ToArray()){
                var worldPos = point + splineContainer.transform.position;

                Gizmos.DrawWireSphere(worldPos,BallSize * 0.4f); 
            }
        }
    }
}
