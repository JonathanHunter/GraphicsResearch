namespace GraphicsResearch.PathPlacement
{
    using System.Collections.Generic;
    using UnityEngine;
    using RoomPlacement;

    public class VariableSpanningTree : PathManager
    {
        [SerializeField]
        private bool showLines = false;
        [SerializeField]
        private float pathWidth = .5f;

        private void OnDrawGizmos()
        {
            if (showLines && this.Paths != null && this.ExtraPaths != null)
            {
                foreach (Path path in this.Paths)
                    path.Draw();

                int numToAdd = (int)(this.ExtraPaths.Count * this.edgeAmount);
                for (int i = 0; i < numToAdd; i++)
                    this.ExtraPaths[i].Draw();
            }
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
                AddUnobstructedEdges(basePaths, r1, rooms.Rooms);
                objects.Add(r1);
            }

            Kruskals(basePaths, objects);
        }

        protected override void LocalClear()
        {
        }

        private void AddUnobstructedEdges(List<Path> basePath, Room r1, List<Room> rooms)
        {
            foreach (Room r2 in rooms)
            {
                if (r1 != r2)
                {
                    if (!IsBlocked(r1, r2))
                    {
                        bool duplicate = false;
                        foreach(Path e in basePath)
                        {
                            if ((e.StartRoom.gameObject == r1.gameObject && e.EndRoom == r2) ||
                                (e.StartRoom.gameObject == r2.gameObject && e.EndRoom == r1))
                                duplicate = true;
                        }
                        if(!duplicate)
                            basePath.Add(CreatePath(r1, r2));
                    }
                }
            }
        }

        private bool IsBlocked(Room r1, Room r2)
        {
            Vector3 r12 = r2.transform.position - r1.transform.position;
            Vector3 left = new Vector3(-r12.y, r12.x, r12.z).normalized;
            RaycastHit[] hits = Physics.RaycastAll(r1.transform.position + left * this.pathWidth / 2f, r12.normalized, r12.magnitude);
            int hitCount = hits.Length;
            foreach(RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == r1.gameObject || hit.collider.gameObject == r2.gameObject)
                    hitCount--;
            }

            hits = Physics.RaycastAll(r1.transform.position - left * this.pathWidth / 2f, r12.normalized, r12.magnitude);
            hitCount += hits.Length;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == r1.gameObject || hit.collider.gameObject == r2.gameObject)
                    hitCount--;
            }

            return hitCount > 0;
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
            foreach(Path path in basePaths)
            {
                if (!disjointSets.IsSameSet(path.StartRoom, path.EndRoom))
                {
                    disjointSets.Merge(path.StartRoom, path.EndRoom);
                    this.Paths.Add(path);
                }
                else
                    this.ExtraPaths.Add(path);
            }
        }
    }
}
