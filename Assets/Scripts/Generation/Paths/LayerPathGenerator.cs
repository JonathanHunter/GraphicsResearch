namespace GraphicsResearch.Generation.Paths
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Rooms;

    public class LayerPathGenerator : MonoBehaviour
    {
        /// <summary> The range of allowed gradients. </summary>
        [SerializeField]
        private Vector2 gradientRange;

        /// <summary> The range of allowed distances. </summary>
        [SerializeField]
        private Vector2 distanceRange;

        /// <summary> The number of paths to keep. </summary>
        [SerializeField]
        private int pathsToKeep;

        /// <summary> Finds all the allowed middle layer paths for the given floors. </summary>
        /// <param name="floor1"> The top floor. </param>
        /// <param name="floor2"> The bottom floor. </param>
        /// <param name="layer"> The layer to hold the paths. </param>
        public IEnumerator FindMiddleLayerPathsAsync(Floor floor1, Floor floor2, MiddleLayer layer)
        {
            List<LayerPath> paths = new List<LayerPath>();
            foreach (Room r1 in floor1.Rooms)
            {
                foreach (Room r2 in floor2.Rooms)
                {
                    LayerPath p = CompareRoom(r1, r2, floor1, floor2, paths);
                    if (p != null)
                        paths.Add(p);
                }

                yield return null;
            }
            List<LayerPath> finalPaths = new List<LayerPath>();

            int i = 0;
            while(i < this.pathsToKeep && i < paths.Count)
            {
                LayerPath path = paths[Random.Range(0, paths.Count - 1)];
                bool blocked = false;
                foreach (LayerPath p in finalPaths)
                {
                    if (GenerationUtilityFunctions.LineLineIntersection(
                        path.Start.Position,
                        path.End.Position,
                        p.Start.Position,
                        p.End.Position) != Vector2.zero)
                    {
                        blocked = true;
                    }
                }

                if(!blocked)
                {
                    i++;
                    finalPaths.Add(path);
                }
            }

            layer.Paths = finalPaths;
        }

        private LayerPath CompareRoom(Room r1, Room r2, Floor floor1, Floor floor2, List<LayerPath> paths)
        {
            float gradient = GetGradient(r1, r2);
            float distance = GetDistance(r1, r2);
            Vector3 start = r1.WorldPosition;
            Vector3 end = r2.WorldPosition;
            Vector3 mid = Vector3.Lerp(start, end, .5f);

            foreach (Room r in floor1.Rooms)
            {
                if (r1 != r)
                {
                    if (CheckIntersection(start, mid, r))
                        return null;
                }
            }

            foreach (Room r in floor2.Rooms)
            {
                if (r2 != r)
                {
                    if (CheckIntersection(end, mid, r))
                        return null;
                }
            }

            if (gradient >= this.gradientRange.x && gradient <= this.gradientRange.y &&
                distance >= this.distanceRange.x && distance <= this.distanceRange.y)
            {
                LayerPath p = new LayerPath
                {
                    Start = r1,
                    End = r2,
                    Distance = distance,
                    Width = 0.5f,
                    Gradient = gradient
                };
                
                return p;
            }

            return null;
        }

        private float GetGradient(Room r1, Room r2)
        {
            Vector3 refPoint = new Vector3(r1.WorldPosition.x, r2.WorldPosition.y, r1.WorldPosition.z);
            Vector3 r21 = Vector3.Normalize(r1.WorldPosition - r2.WorldPosition);
            Vector3 r2ref = Vector3.Normalize(refPoint - r2.WorldPosition);
            return Vector3.Angle(r2ref, r21);
        }

        private float GetDistance(Room r1, Room r2)
        {
            return Vector3.Distance(r1.WorldPosition, r2.WorldPosition);
        }

        private bool CheckIntersection(Vector3 start, Vector3 end, Room room)
        {
            if (room is CircleRoom)
            {
                CircleRoom c = (CircleRoom)room;
                return GenerationUtilityFunctions.CircleLineIntersection(start, end, c.WorldPosition, c.Radius) != Vector2.zero;
            }
            else
            {
                RectangleRoom rect = (RectangleRoom)room;
                Vector3 s, e;
                float width;
                s = rect.WorldPosition + rect.transform.TransformDirection(rect.transform.forward) * rect.Dimentions.y / 2f;
                e = rect.WorldPosition - rect.transform.TransformDirection(rect.transform.forward) * rect.Dimentions.y / 2f;
                width = rect.Dimentions.x;
                Vector3 es = Vector3.Normalize(s - e);
                Vector3 left = new Vector3(-es.y, es.x, es.z);
                Vector3 tl = s + left * width / 2f;
                Vector3 tr = s - left * width / 2f;
                Vector3 bl = e + left * width / 2f;
                Vector3 br = e - left * width / 2f;
                return GenerationUtilityFunctions.BoxLineIntersection(start, end, tl, tr, bl, br) != Vector2.zero;
            }
        }
    }
}
