namespace GraphicsResearch.PathPlacement
{
    using System.Collections.Generic;
    using UnityEngine;
    using GraphicsResearch.RoomPlacement;

    public class FancyPaths : PathManager
    {
        [SerializeField]
        private bool showLines = false;
        [SerializeField]
        private float pathWidth = .5f;
        [SerializeField]
        private int maxOffsetAttempts = 10;
        [SerializeField]
        private int maxRecursionDepth = 10;

        private void OnDrawGizmos()
        {
            if (showLines && this.Edges != null && this.ExtraEdges != null)
            {
                foreach (Path edge in this.Edges)
                {
                    DrawPath(edge);
                    if(edge.EndRoom == null)
                    {
                        Gizmos.DrawWireSphere(edge.End, this.pathWidth / 2f);
                    }
                }
            }
        }

        private void DrawPath(Path edge)
        {
            Vector3 start = edge.Start;
            Vector3 end = edge.End;
            Vector3 es = Vector3.Normalize(start - end);
            Vector3 left = new Vector3(-es.y, es.x, es.z);
            Gizmos.DrawLine(start + left * this.pathWidth / 2f, end + left * this.pathWidth / 2f);
            Gizmos.DrawLine(start - left * this.pathWidth / 2f, end - left * this.pathWidth / 2f);
        }

        protected override void LocalClear()
        {

        }

        protected override void LocalInit()
        {
        }

        protected override void LocalPlacePaths(RoomManager rooms)
        {
            List<Path> paths = FindPath(
                rooms.CircleRooms[0].transform.position, 
                rooms.CircleRooms[1].transform.position,
                rooms.CircleRooms[0],
                rooms.CircleRooms[1],
                new List<Path>(), 
                0);
            if(paths != null)
            {
                foreach (Path p in paths)
                    this.Edges.Add(p);
            }
        }

        private List<Path> FindPath(Vector3 point1, Vector3 point2, Room room1, Room room2, List<Path> paths, int depth)
        {
            if (depth > this.maxRecursionDepth)
            {
                Debug.Log("Hit max depth");
                return null;
            }

            if(CheckStraightPath(point1, point2))
            {
                AddPath(paths, point1, point2, room1, room2);
                return paths;
            }
            else
            {
                Vector3 center = Vector3.Lerp(point1, point2, .5f);
                Vector3 p12 = Vector3.Normalize(point1 - point2);
                Vector3 left = new Vector3(-p12.y, p12.x, p12.z);
                if (FindCenterOffset(left, this.pathWidth, ref center))
                {
                    paths = FindPath(point1, center, room1, null, paths, depth + 1);
                    if (paths == null)
                        return null;

                    paths = FindPath(center, point2, null, room2, paths, depth + 1);
                    return paths;
                }
                else
                    return null;
            }
        }

        private bool CheckStraightPath(Vector3 c1, Vector3 c2)
        {
            Vector3 c12 = c2 - c1;
            Vector3 left = new Vector3(-c12.y, c12.x, c12.z).normalized;
            RaycastHit[] hits = Physics.RaycastAll(c1 + left * this.pathWidth / 2f, c12.normalized, c12.magnitude);
            int hitCount = hits.Length;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.transform.position == c1 || hit.collider.transform.position == c2)
                    hitCount--;
            }

            hits = Physics.RaycastAll(c1 - left * this.pathWidth / 2f, c12.normalized, c12.magnitude);
            hitCount += hits.Length;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.transform.position == c1 || hit.collider.transform.position == c2)
                    hitCount--;
            }

            return hitCount <= 0;
        }

        private void AddPath(List<Path> paths, Vector2 point1, Vector2 point2, Room room1, Room room2)
        {
            float dist = Vector3.Distance(point1, point2);
            paths.Add(new Path(point1, point2, room1, room2, dist, this.pathWidth));
        }

        private bool FindCenterOffset(Vector3 left, float pathWidth, ref Vector3 center)
        {
            int leftAttempts = 0;
            int rightAttempts = 0;
            bool leftFound = false;
            bool rightFound = false;
            Vector3 leftCenter = Vector3.zero;
            Vector3 rightCenter = Vector3.zero;
            while (leftAttempts < maxOffsetAttempts && !leftFound)
            {
                Collider[] overlaps = Physics.OverlapSphere(center + left * (leftAttempts * pathWidth /2f), pathWidth);
                if (overlaps.Length == 0)
                {
                    leftCenter = center + left * (leftAttempts * pathWidth / 2f);
                    leftFound = true;
                }
                else
                    leftAttempts++;
            }
            
            while (rightAttempts < maxOffsetAttempts && !rightFound)
            {
                Collider[] overlaps = Physics.OverlapSphere(center - left * (rightAttempts * pathWidth / 2f), pathWidth);
                if (overlaps.Length == 0)
                {
                    rightCenter = center - left * (rightAttempts * pathWidth / 2f);
                    rightFound = true;
                }
                else
                    rightAttempts++;
            }
            
            if(leftFound && rightFound)
            {
                if (leftAttempts < rightAttempts)
                    center = leftCenter;
                else
                    center = rightCenter;
                return true;
            }
            else if(leftFound)
            {
                center = leftCenter;
                return true;
            }
            else if(rightFound)
            {
                center = rightCenter;
                return true;
            }
            else
                return false;
        }
    }
}
