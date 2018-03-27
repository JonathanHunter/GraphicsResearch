namespace GraphicsResearch.Generation.Paths
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Rooms;

    public class FloorPathGenerator : MonoBehaviour
    {
        /// <summary> The amoutn of extra edges to include in the path generation. </summary>
        [SerializeField]
        [Range(0f, 1f)]
        protected float edgeAmount;
        [SerializeField]
        private float pathWidth = .5f;
        [SerializeField]
        private int maxOffsetAttempts = 10;
        [SerializeField]
        private int maxRecursionDepth = 6;

        /// <summary> Finds all the allowed floor paths for the given floor. </summary>
        /// <param name="floor"> The floor. </param>
        /// <param name="upperLayer"> The layer above this floor. </param>
        /// <param name="upperLayer"> The layer below this floor. </param>
        public IEnumerator FindFloorPathsAsync(Floor floor, MiddleLayer upperLayer, MiddleLayer lowerLayer)
        {
            List<FloorPath> basePaths = new List<FloorPath>();
            List<Room> objects = new List<Room>();

            foreach (Room r1 in floor.Rooms)
            {
                foreach (Room r2 in floor.Rooms)
                {
                    TryAddPath(r1, r2, basePaths, floor, upperLayer, lowerLayer);
                }

                objects.Add(r1);
                yield return null;
            }

            floor.Paths =  Kruskals(basePaths, objects);
        }

        private void TryAddPath(Room r1, Room r2, List<FloorPath> basePaths, Floor floor, MiddleLayer upperLayer, MiddleLayer lowerLayer)
        {
            if (r1 != r2)
            {
                if (!CheckPathBlocked(r1.Position, r2.Position, r1, r2, 0, floor, upperLayer, lowerLayer))
                {
                    bool duplicate = false;
                    foreach (Path e in basePaths)
                    {
                        if ((e.Start == r1 && e.End == r2) ||
                            (e.End == r2 && e.End == r1))
                            duplicate = true;
                    }

                    if (!duplicate)
                    {
                        List<Edge> edges = FindPath(
                            r1.Position, r2.Position, r1, r2, new List<Edge>(), 0, floor, upperLayer, lowerLayer);
                        float dist = 0;
                        foreach (Edge e in edges)
                            dist += e.Distance;

                        FloorPath path = new FloorPath
                        {
                            Start = r1,
                            End = r2,
                            Distance = dist,
                            Width = this.pathWidth,
                            Edges = edges.ToArray()
                        };

                        basePaths.Add(path);
                    }
                }
            }
        }

        private List<Edge> FindPath(Vector3 point1, Vector3 point2, Room room1, Room room2, List<Edge> edges, int depth, Floor floor, MiddleLayer upperLayer, MiddleLayer lowerLayer)
        {
            if (depth > this.maxRecursionDepth)
                return null;

            if (!CheckEdgeBlocked(point1, point2, room1, room2, floor, upperLayer, lowerLayer))
            {
                Edge e = new Edge
                {
                    Start = point1,
                    End = point2,
                    Distance = Vector3.Distance(point1, point2),
                    Width = this.pathWidth,
                    TailEdge = room2 != null
                };
                edges.Add(e);
                return edges;
            }
            else
            {
                Vector3 center = Vector3.Lerp(point1, point2, .5f);
                Vector3 p12 = Vector3.Normalize(point1 - point2);
                Vector3 left = new Vector3(-p12.y, p12.x, p12.z);
                if (FindCenterOffset(left, this.pathWidth, ref center, floor, upperLayer, lowerLayer))
                {
                    edges = FindPath(point1, center, room1, null, edges, depth + 1, floor, upperLayer, lowerLayer);
                    if (edges == null)
                        return null;

                    return FindPath(center, point2, null, room2, edges, depth + 1, floor, upperLayer, lowerLayer);
                }
                else
                    return null;
            }
        }

        /// <summary> Checks if the given path is blocked. </summary>
        /// <param name="point1"> The start of the path. </param>
        /// <param name="point2"> The end of the path. </param>
        /// <param name="room1"> The starting room of the path. </param>
        /// <param name="room2"> The ending room of the path. </param>
        /// <param name="depth"> The current recursion depth. </param>
        /// <param name="floor"> The floor this path is on. </param>
        /// <param name="upperLayer"> The Layer above this path's floor. </param>
        /// <param name="lowerLayer"> The layer below this path's floor. </param>
        /// <returns> True if there is something blocking this path. </returns>
        private bool CheckPathBlocked(Vector3 point1, Vector3 point2, Room room1, Room room2, int depth, Floor floor, MiddleLayer upperLayer, MiddleLayer lowerLayer)
        {
            if (depth > this.maxRecursionDepth)
                return true;

            if (!CheckEdgeBlocked(point1, point2, room1, room2, floor, upperLayer, lowerLayer))
                return false;
            else
            {
                Vector3 center = Vector3.Lerp(point1, point2, .5f);
                Vector3 p12 = Vector3.Normalize(point1 - point2);
                Vector3 left = new Vector3(-p12.y, p12.x, p12.z);
                if (FindCenterOffset(left, this.pathWidth, ref center, floor, upperLayer, lowerLayer))
                {
                    bool blocked = CheckPathBlocked(point1, center, room1, null, depth + 1, floor, upperLayer, lowerLayer);
                    if (blocked)
                        return true;

                    return CheckPathBlocked(center, point2, null, room2, depth + 1, floor, upperLayer, lowerLayer);
                }
                else
                    return true;
            }
        }

        /// <summary> Checks if the given edge is blocked. </summary>
        /// <param name="begin"> The start of the edge. </param>
        /// <param name="end"> The end of the edge. </param>
        /// <param name="r1"> The starting room of the path. </param>
        /// <param name="r2"> The ending room of the path. </param>
        /// <param name="floor"> The floor this path is on. </param>
        /// <param name="upperLayer"> The Layer above this path's floor. </param>
        /// <param name="lowerLayer"> The layer below this path's floor. </param>
        /// <returns> True if there is something blocking this edge. </returns>
        private bool CheckEdgeBlocked(Vector3 begin, Vector3 end, Room r1, Room r2, Floor floor, MiddleLayer upperLayer, MiddleLayer lowerLayer)
        {
            Vector3 tl, tr, bl, br;
            GenerationUtility.BoxBounds(begin, end, this.pathWidth, out tl, out tr, out bl, out br);

            foreach(Room r in floor.Rooms)
            {
                if(r != r1 && r != r2)
                {
                    if (r.IsIntersectingLine(tl, bl))
                        return true;

                    if (r.IsIntersectingLine(tr, br))
                        return true;
                }
            }

            if (upperLayer != null)
            {
                foreach (LayerPath l in upperLayer.Paths)
                {
                    if (l.IsIntersectingLine(tl, bl))
                        return true;

                    if (l.IsIntersectingLine(tr, br))
                        return true;
                }
            }

            if (lowerLayer != null)
            {
                foreach (LayerPath l in lowerLayer.Paths)
                {
                    if (l.IsIntersectingLine(tl, bl))
                        return true;

                    if (l.IsIntersectingLine(tr, br))
                        return true;
                }
            }

            return false;
        }

        private bool FindCenterOffset(Vector3 left, float pathWidth, ref Vector3 center, Floor floor, MiddleLayer upperLayer, MiddleLayer lowerLayer)
        {
            int leftAttempts = 0;
            int rightAttempts = 0;
            bool leftFound = false;
            bool rightFound = false;
            Vector3 leftCenter = Vector3.zero;
            Vector3 rightCenter = Vector3.zero;
            while (leftAttempts < maxOffsetAttempts && !leftFound)
            {
                Vector3 temp = center + left * (leftAttempts * pathWidth / 2f);
                if (!CheckCircleBlocked(temp, this.pathWidth, floor, upperLayer, lowerLayer))
                {
                    leftCenter = temp;
                    leftFound = true;
                }
                else
                    leftAttempts++;
            }

            while (rightAttempts < maxOffsetAttempts && !rightFound)
            {
                Vector3 temp = center - left * (rightAttempts * pathWidth / 2f);
                if (!CheckCircleBlocked(temp, this.pathWidth, floor, upperLayer, lowerLayer))
                {
                    rightCenter = temp;
                    rightFound = true;
                }
                else
                    rightAttempts++;
            }

            if (leftFound && rightFound)
            {
                if (leftAttempts < rightAttempts)
                    center = leftCenter;
                else
                    center = rightCenter;
                return true;
            }
            else if (leftFound)
            {
                center = leftCenter;
                return true;
            }
            else if (rightFound)
            {
                center = rightCenter;
                return true;
            }
            else
                return false;
        }

        private bool CheckCircleBlocked(Vector3 center, float radius, Floor floor, MiddleLayer upperLayer, MiddleLayer lowerLayer)
        {
            foreach (Room r in floor.Rooms)
            {
                if (r.IsIntersectingCircle(center, radius))
                    return true;
            }

            if (upperLayer != null)
            {
                foreach (LayerPath l in upperLayer.Paths)
                {
                    if (l.IsIntersectingCircle(center, radius))
                        return true;
                }
            }

            if (lowerLayer != null)
            {
                foreach (LayerPath l in lowerLayer.Paths)
                {
                    if (l.IsIntersectingCircle(center, radius))
                        return true;
                }
            }

            return false;
        }

        /// <summary> Use kruskals to find the minimum spanning tree plus a few extra edges. </summary>
        /// <param name="basePaths"> All possible paths. </param>
        /// <param name="objects"> All rooms. </param>
        /// <returns> The final set of paths for this floor. </returns>
        private List<FloorPath> Kruskals(List<FloorPath> basePaths, List<Room> objects)
        {
            List<FloorPath> paths = new List<FloorPath>();
            List<FloorPath> extraPaths = new List<FloorPath>();
            DisjointSets<Room> disjointSets = new DisjointSets<Room>(objects);
            basePaths.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            foreach (FloorPath path in basePaths)
            {
                if (!disjointSets.IsSameSet(path.Start, path.End))
                {
                    disjointSets.Merge(path.Start, path.End);
                    paths.Add(path);
                }
                else
                    extraPaths.Add(path);
            }

            int center = (int)(extraPaths.Count * .75f);
            if (center + 2 < extraPaths.Count)
            {
                FloorPath p1 = extraPaths[center];
                FloorPath p2 = extraPaths[center + 1];
                FloorPath p3 = extraPaths[center + 2];
                extraPaths.Remove(p1);
                extraPaths.Remove(p2);
                extraPaths.Remove(p3);
                extraPaths.Insert(0, p1);
                extraPaths.Insert(1, p2);
                extraPaths.Insert(2, p3);
            }
            
            List<FloorPath> finalPaths = new List<FloorPath>();
            foreach (FloorPath e in paths)
                finalPaths.Add(e);

            int numToAdd = (int)(extraPaths.Count * this.edgeAmount);
            for (int i = 0; i < numToAdd; i++)
                finalPaths.Add(extraPaths[i]);

            return finalPaths;
        }
    }
}
