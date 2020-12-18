using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Habrador_Computational_Geometry
{
    //Will try to remove unwanted triangles (slivers) from a mesh to get a mesh withn higher quality
    //The quality of a triangle can be measured as the ratio of the shortest edge to the radius of the triangle's circumcircle
    //I haven't found a general algorithm on how to do it - so compare it with the mesh simplification algorithms
    public static class RemoveUnwantedTriangles
    {
        //To find a needle you can:
        // - One angle in the triangle is close to 0
        // - The ratio between the shortest and longest side is below 0.01
        private const float NEEDLE_RATIO = 0.01f;



        //meshData should be triangles only
        //normalizer is just for debugging
        public static void Remove(HalfEdgeData3 meshData, Normalizer3 normalizer = null)
        {
            //We are going to remove the following (some triangles can be a combination of these):
            // - Caps. Triangle where one angle is close to 180 degrees. Are difficult to remove. If the vertex is connected to three triangles, we can maybe just remove the vertex and build one big triangle. This can be said to be a flat terahedron?

            // - Needles. Triangle where the longest edge is much longer than the shortest one.  Same as saying that the smallest angle is close to 0 degrees? Can often be removed by collapsing the shortest edge
            RemoveNeedles(meshData, normalizer);

            //TODO: The above should be in the same loop because when we have removed a needle we might get a new cap, etc
        }



        //Needles. Triangle where the longest edge is much longer than the shortest one.
        private static void RemoveNeedles(HalfEdgeData3 meshData, Normalizer3 normalizer = null)
        {
            HashSet<HalfEdgeFace3> triangles = meshData.faces;

            int needleCounter = 0;

            bool foundNeedle = false;

            int safety = 0;

            do
            {
                foundNeedle = false;

                foreach (HalfEdgeFace3 triangle in triangles)
                {
                    /*
                    List<HalfEdge3> edges = triangle.GetEdges();

                    //Sort the edges from shortest to longest
                    List<HalfEdge3> edgesSorted = edges.OrderBy(e => e.Length()).ToList();

                    //The ratio between the shortest and longest side
                    float edgeLengthRatio = edgesSorted[0].Length() / edgesSorted[2].Length();
                    */

                    //Instead of using a million lists, we know we have just three edges we have to sort, so we can do better
                    HalfEdge3 e1 = triangle.edge;
                    HalfEdge3 e2 = triangle.edge.nextEdge;
                    HalfEdge3 e3 = triangle.edge.nextEdge.nextEdge;

                    //We want e1 to be the shortest and e3 to be the longest
                    if (e1.SqrLength() > e3.SqrLength()) (e1, e3) = (e3, e1);

                    if (e1.SqrLength() > e2.SqrLength()) (e1, e2) = (e2, e1);

                    //e1 is now the shortest edge, so we just need to check the second and third

                    if (e2.SqrLength() > e3.SqrLength()) (e2, e3) = (e3, e2);


                    //The ratio between the shortest and longest edge
                    float edgeLengthRatio = e1.Length() / e3.Length();

                    //This is a needle
                    if (edgeLengthRatio < NEEDLE_RATIO)
                    {
                        //Debug.Log("We found a needle triangle");

                        TestAlgorithmsHelpMethods.DebugDrawTriangle(triangle, Color.blue, Color.red, normalizer);

                        needleCounter += 1;

                        //Remove the needle by merging the shortest edge
                        MyVector3 mergePosition = (e1.v.position + e1.prevEdge.v.position) * 0.5f;

                        meshData.MergeEdge(e1, mergePosition);

                        foundNeedle = true;

                        //Now we have to restart because the triangulation has changed
                        break;
                    }
                }


                safety += 1;

                if (safety > 100000)
                {
                    Debug.LogWarning("Stuck in infinite loop while removing needles");

                    break;
                }
            }
            while (foundNeedle);

            Debug.Log($"Found {needleCounter} needles");
        }
    }
}
