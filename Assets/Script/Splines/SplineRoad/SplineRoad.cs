using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
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

    private void ComponentCheck()
    {
        if (!GetComponent<SplineContainer>()) gameObject.AddComponent<SplineContainer>();
        else if (!GetComponent<MeshFilter>()) gameObject.AddComponent<MeshFilter>();
        else if (!GetComponent<MeshRenderer>()) gameObject.AddComponent<MeshRenderer>();

        if (!_Mesh) _Mesh = new Mesh();
        _Mesh.name = "Road Procederal Mesh";
        GetComponent<MeshFilter>().mesh = _Mesh;
        if (!splineContainer) splineContainer = GetComponent<SplineContainer>();
    }

    private Quaternion GetKnotRotation(SplineKnotIndex KnotInxdex){
        var knot = splineContainer.Splines[KnotInxdex.Spline][KnotInxdex.Knot];

        var knotRot = knot.Rotation.value;
        return new Quaternion(knotRot.x,knotRot.y,knotRot.z,knotRot.w);
    }

    private Vector3 GetKnotLocalPos(SplineKnotIndex KnotIndex){
        var Knot = splineContainer.Splines[KnotIndex.Spline][KnotIndex.Knot];
        return Knot.Position; 
    }

    // Find All Intersections
    private SplineKnotIndex[] GetAllIntersectedKnots()
    {
        List<SplineKnotIndex> returnList = new List<SplineKnotIndex>();
        for (int splineI = 0; splineI < splineContainer.Splines.Count; splineI++)
        {
            for (int knotI = 0; knotI < splineContainer.Splines[splineI].Count; knotI++)
            {
                var links = GetLinksAtKnot(new SplineKnotIndex(splineI, knotI));
                // 2 Link at least
                // No Road To Road Links
                if (links.Count >= 2)
                {
                    returnList.Add(new SplineKnotIndex(splineI, knotI));
                }
            }
        }
        return returnList.ToArray();
    }

    private SplineKnotIndex[] GetAllRoadKnots()
    {
        List<SplineKnotIndex> returnList = new List<SplineKnotIndex>();
        var Intersections = GetAllIntersectedKnots();
        for (int splineI = 0; splineI < splineContainer.Splines.Count; splineI++)
        {
            for (int knotI = 0; knotI < splineContainer.Splines[splineI].Count - 1; knotI++)
            {
                var thisKnotIndex = new SplineKnotIndex(splineI, knotI);
                var nextknotIndex = new SplineKnotIndex(splineI, knotI + 1);

                if (!Intersections.Contains(nextknotIndex) && !Intersections.Contains(thisKnotIndex))
                {
                    returnList.Add(thisKnotIndex);
                }

            }
        }
        return returnList.ToArray();
    }

    private ReadOnlyArray<SplineKnotIndex> GetAdjacentKnots(SplineKnotIndex targetKnotIndex)
    {
        List<SplineKnotIndex> returnList = new List<SplineKnotIndex>();

        var spline = splineContainer[targetKnotIndex.Spline];
        if (spline != null)
        {

            int[] index = { targetKnotIndex.Knot - 1, targetKnotIndex.Knot + 1 };
            var maxIndex = spline.Count - 1;


            foreach (var i in index)
            {
                // is Index in range of spline count
                if (i <= maxIndex && i > 0)
                {
                    var newKnotIndex = new SplineKnotIndex();
                    newKnotIndex.Spline = targetKnotIndex.Spline;
                    newKnotIndex.Knot = i;
                    returnList.Add(
                        newKnotIndex
                    );
                }
            }
        }
        return returnList.ToArray();

    }

    // Find the Same Knot linked on other splines
    private ReadOnlyArray<SplineKnotIndex> GetLinksAtKnot(SplineKnotIndex knotIndex)
    {
        return splineContainer.KnotLinkCollection.GetKnotLinks(knotIndex).ToArray();
    }

    private void _GenerateMesh()
    {
        _Mesh.Clear();
        List<Vector3> _MeshVertecies = new List<Vector3>();
        List<Vector2> _MeshUV = new List<Vector2>();
        List<int> _MeshTriIndex = new List<int>();

        // mesh points
        foreach (var roadKnot in GetAllRoadKnots())
        {

            var knot = SplineHelper.GetKnotTransform(splineContainer, roadKnot);
            var nextKnot = SplineHelper.GetKnotTransform(splineContainer, new SplineKnotIndex(
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

            int[] triIndex = {
                0, 2, 1,
                1, 2, 3
            };



            _MeshVertecies.AddRange(MeshPoint);
            _MeshUV.AddRange(MeshUV);



            int lastIndex = _MeshVertecies.Count - MeshPoint.Length;

            foreach (int index in triIndex)
            {
                if (lastIndex <= 0)
                {
                    _MeshTriIndex.Add(index);
                }
                else
                {
                    _MeshTriIndex.Add(index + lastIndex);
                }
            }

        }

        _Mesh.vertices = _MeshVertecies.ToArray();
        _Mesh.uv = _MeshUV.ToArray();
        _Mesh.triangles = _MeshTriIndex.ToArray();
    }
    
    void OnDrawGizmos()
    {
        float BallSize = RoadWidth * 0.5f;
        // Intersection
        foreach (var knotIndex in GetAllIntersectedKnots())
        {
            var knot = splineContainer.Splines[knotIndex.Spline][knotIndex.Knot];
            var knotWorldPos = (Vector3)knot.Position + splineContainer.transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(knotWorldPos, BallSize * 0.5f);
        }


        // Road Points
        foreach (var knotIndex in GetAllRoadKnots())
        {
            var knot = splineContainer.Splines[knotIndex.Spline][knotIndex.Knot];
            var knotWorldPos = (Vector3)knot.Position + splineContainer.transform.position;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(knotWorldPos, BallSize * 0.5f);
        }

        // Mesh Road Points
        foreach (var roadKnot in GetAllRoadKnots())
        {
            var knot = SplineHelper.GetKnotTransform(splineContainer, roadKnot);
            var nextKnot = SplineHelper.GetKnotTransform(splineContainer, new SplineKnotIndex(
                roadKnot.Spline,
                roadKnot.Knot + 1
            ));

            Vector3[] Points = {
                Vector3.left * -RoadWidth,
                Vector3.right * -RoadWidth
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
            foreach (var point in allPoints.ToArray())
            {
                var worldPos = point + splineContainer.transform.position;

                Gizmos.DrawWireSphere(worldPos, BallSize * 0.4f);
            }
        }


        // Intersection
        Gizmos.color = Color.white;
        foreach (var IntersectionKnot in GetAllIntersectedKnots())
        {

            var knotNormalPos = SplineUtility.ConvertIndexUnit(
                splineContainer.Splines[IntersectionKnot.Spline],
                IntersectionKnot.Knot,
                PathIndexUnit.Knot,
                PathIndexUnit.Normalized
            );
            var knotTangent = ((Vector3)SplineUtility.EvaluateTangent(
                splineContainer.Splines[IntersectionKnot.Spline],
                knotNormalPos
            )).normalized;


            List<SplineKnotIndex> LinkedIntersect = new List<SplineKnotIndex>();

            // Get Neighbouring Knots linked to intersection
            foreach (var linkedKnot in GetLinksAtKnot(IntersectionKnot))
            {
                var Neighbours = GetAdjacentKnots(linkedKnot);
                foreach (var adjKnot in Neighbours)
                {
                    // Dont Add Intersection itself
                    if (!LinkedIntersect.Contains(adjKnot))
                    {
                        LinkedIntersect.Add(adjKnot);
                    }
                }
            }


            // Sort Container
            List<BubbleSortObject<Vector3>> PointSort = new List<BubbleSortObject<Vector3>>();


            foreach (var otherKnot in LinkedIntersect)
            {
                // Transform of intersect knot
                var centerLocalTransform = SplineHelper.GetKnotTransform(
                    splineContainer,
                    IntersectionKnot
                );
                // Neighbour knot local position
                var otherLocalPosition = GetKnotLocalPos(otherKnot);

                Vector3 roadRight = Vector3.right * RoadWidth;
                Vector3[] localRoadMeshPoint = {
                    otherLocalPosition +
                    (GetKnotRotation(otherKnot) * roadRight),
                    otherLocalPosition +
                    (GetKnotRotation(otherKnot) * -roadRight),
                };

                foreach (var roadPoint in localRoadMeshPoint)
                {
                    // Pos relative to intersection
                    var otherRelativePos = roadPoint - GetKnotLocalPos(IntersectionKnot);
                    var intersectRightVector = GetKnotRotation(IntersectionKnot) * Vector3.right;
                    var intersectForwardVector = GetKnotRotation(IntersectionKnot) * Vector3.forward;
                    

                    // Linked Knot's Dot Protduct to Right of Intersection Vector
                    var LinkedKnotDotProduct = Vector3.Dot(
                        otherRelativePos,
                        intersectRightVector
                    );



                    // Angle of linked knot
                    var otherVectorAngle = Vector3.Angle(
                        intersectForwardVector,
                        otherRelativePos
                    );

                    // if (LinkedKnotDotProduct > 0)
                    // {
                    //     otherVectorAngle = -otherVectorAngle;
                    // }

                    PointSort.Add(new BubbleSortObject<Vector3>(
                        roadPoint,
                        otherVectorAngle
                    ));
                }

            }


            var SortedPoints = BubbleSort.Sort(PointSort.ToArray());
            for (int i = 0; i < SortedPoints.Length; i++)
            {
                var meshPointPos = SortedPoints[i].obj;
                float floatIndex = (float)i / (SortedPoints.Length - 1);

                Color knotColor = new Color(0, floatIndex, 0);
                Gizmos.color = knotColor;
                Gizmos.DrawWireSphere(meshPointPos + splineContainer.gameObject.transform.position, BallSize * 1.0f);
            }
        }


    }


}
