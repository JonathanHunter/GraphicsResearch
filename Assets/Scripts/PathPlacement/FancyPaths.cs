namespace GraphicsResearch.PathPlacement
{
    using System.Collections;
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
        private int maxRecursionDepth = 6;
        [SerializeField]
        private int maxFancyPathsPerRoom = 3;

        private void OnDrawGizmos()
        {
            if (showLines && this.Paths != null && this.ExtraPaths != null)
            {
                foreach (Path p in GetPaths())
                    p.Draw();
            }
        }

        protected override void LocalClear()
        {

        }

        protected override void LocalInit()
        {
        }

        protected override void LocalPlacePaths(RoomManager rooms)
        {
            List<Path> basePaths = new List<Path>();
            List<Room> objects = new List<Room>();

            foreach (Room r1 in rooms.Rooms)
            {
                int fancyPaths = 0;
                foreach (Room r2 in rooms.Rooms)
                {
                    TryAddPath(r1, r2, basePaths, ref fancyPaths);
                }

                objects.Add(r1);
            }

            Kruskals(basePaths, objects);
        }

        protected override IEnumerator LocalPlacePathsAsync(RoomManager rooms)
        {
            List<Path> basePaths = new List<Path>();
            List<Room> objects = new List<Room>();

            foreach (Room r1 in rooms.Rooms)
            {
                int fancyPaths = 0;
                foreach (Room r2 in rooms.Rooms)
                {
                    TryAddPath(r1, r2, basePaths, ref fancyPaths);
                }

                objects.Add(r1);
                yield return null;
            }

            Kruskals(basePaths, objects);
            yield return null;
        }

        private void TryAddPath(Room r1, Room r2, List<Path> basePaths, ref int fancyPaths)
        {
            if (r1 != r2)
            {
                if (CheckStraightPath(r1.transform.position, r2.transform.position))
                {
                    bool duplicate = false;
                    foreach (Path e in basePaths)
                    {
                        if ((e.StartRoom == r1 && e.EndRoom == r2) ||
                            (e.StartRoom == r2 && e.EndRoom == r1))
                            duplicate = true;
                    }
                    if (!duplicate)
                        basePaths.Add(CreatePath(r1, r2));
                }
                else if (fancyPaths < maxFancyPathsPerRoom && CheckForPath(r1.transform.position, r2.transform.position, r1, r2, 0))
                {
                    bool duplicate = false;
                    foreach (Path e in basePaths)
                    {
                        if ((e.StartRoom == r1 && e.EndRoom == r2) ||
                            (e.StartRoom == r2 && e.EndRoom == r1))
                            duplicate = true;
                    }
                    if (!duplicate)
                    {
                        fancyPaths++;
                        List<Edge> edges = FindPath(
                            r1.transform.position,
                            r2.transform.position,
                            r1,
                            r2,
                            new List<Edge>(),
                            0);
                        basePaths.Add(new Path(edges));
                    }
                }
            }
        }

        private bool CheckForPath(Vector3 point1, Vector3 point2, Room room1, Room room2, int depth)
        {
            if (depth > this.maxRecursionDepth)
            {
                Debug.Log("Hit max depth");
                return false;
            }

            if (CheckStraightPath(point1, point2))
                return true;
            else
            {
                Vector3 center = Vector3.Lerp(point1, point2, .5f);
                Vector3 p12 = Vector3.Normalize(point1 - point2);
                Vector3 left = new Vector3(-p12.y, p12.x, p12.z);
                if (FindCenterOffset(left, this.pathWidth, ref center))
                {
                    bool ok = CheckForPath(point1, center, room1, null, depth + 1);
                    if (!ok)
                        return false;

                    return CheckForPath(center, point2, null, room2, depth + 1);
                }
                else
                    return false;
            }
        }

        private List<Edge> FindPath(Vector3 point1, Vector3 point2, Room room1, Room room2, List<Edge> edges, int depth)
        {
            if (depth > this.maxRecursionDepth)
            {
                Debug.Log("Hit max depth");
                return null;
            }

            if(CheckStraightPath(point1, point2))
            {
                AddPath(edges, point1, point2, room1, room2);
                return edges;
            }
            else
            {
                Vector3 center = Vector3.Lerp(point1, point2, .5f);
                Vector3 p12 = Vector3.Normalize(point1 - point2);
                Vector3 left = new Vector3(-p12.y, p12.x, p12.z);
                if (FindCenterOffset(left, this.pathWidth, ref center))
                {
                    edges = FindPath(point1, center, room1, null, edges, depth + 1);
                    if (edges == null)
                        return null;

                    edges = FindPath(center, point2, null, room2, edges, depth + 1);
                    return edges;
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

        private void AddPath(List<Edge> edges, Vector2 point1, Vector2 point2, Room room1, Room room2)
        {
            float dist = Vector3.Distance(point1, point2);
            edges.Add(new Edge(point1, point2, room1, room2, dist, this.pathWidth));
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

        private Path CreatePath(Room r1, Room r2)
        {
            float dist = Vector3.Distance(r1.transform.position, r2.transform.position);
            Edge e = new Edge(r1.transform.position, r2.transform.position, r1, r2, dist, this.pathWidth);
            return new Path(new List<Edge> { e });
        }

        private void Kruskals(List<Path> basePaths, List<Room> objects)
        {
            DisjointSets<Room> disjointSets = new DisjointSets<Room>(objects);
            basePaths.Sort((x, y) => x.Weight.CompareTo(y.Weight));
            foreach (Path path in basePaths)
            {
                if (!disjointSets.IsSameSet(path.StartRoom, path.EndRoom))
                {
                    disjointSets.Merge(path.StartRoom, path.EndRoom);
                    this.Paths.Add(path);
                }
                else
                    this.ExtraPaths.Add(path);
            }

            int center = (int)(this.ExtraPaths.Count * .75f);
            if (center + 2 < this.ExtraPaths.Count)
            {
                Path p1 = this.ExtraPaths[center];
                Path p2 = this.ExtraPaths[center + 1];
                Path p3 = this.ExtraPaths[center + 2];
                this.ExtraPaths.Remove(p1);
                this.ExtraPaths.Remove(p2);
                this.ExtraPaths.Remove(p3);
                this.ExtraPaths.Insert(0, p1);
                this.ExtraPaths.Insert(1, p2);
                this.ExtraPaths.Insert(2, p3);
            }
        }
    }
}
