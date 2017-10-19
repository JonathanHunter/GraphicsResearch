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
            if (showLines && this.Edges != null && this.ExtraEdges != null)
            {
                foreach(Path edge in this.Edges)
                    DrawPath(edge);

                int numToAdd = (int)(this.ExtraEdges.Count * this.edgeAmount);
                for (int i = 0; i < numToAdd; i++)
                    DrawPath(this.ExtraEdges[i]);
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

        protected override void LocalInit()
        {
        }

        protected override void LocalPlacePaths(RoomManager rooms)
        {
            List<Path> baseEdges = new List<Path>();
            List<Room> objects = new List<Room>();

            foreach (Room r1 in rooms.Rooms)
            {
                AddUnobstructedEdges(baseEdges, r1, rooms.Rooms);
                objects.Add(r1);
            }

            Kruskals(baseEdges, objects);
        }

        protected override void LocalClear()
        {
        }

        private void AddUnobstructedEdges(List<Path> baseEdges, Room r1, List<Room> rooms)
        {
            foreach (Room r2 in rooms)
            {
                if (r1 != r2)
                {
                    if (!IsBlocked(r1, r2))
                    {
                        bool duplicate = false;
                        foreach(Path e in baseEdges)
                        {
                            if ((e.StartRoom.gameObject == r1.gameObject && e.EndRoom == r2) ||
                                (e.StartRoom.gameObject == r2.gameObject && e.EndRoom == r1))
                                duplicate = true;
                        }
                        if(!duplicate)
                            baseEdges.Add(CreateEdge(r1, r2));
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

        private Path CreateEdge(Room r1, Room r2)
        {
            float dist = Vector3.Distance(r1.transform.position, r2.transform.position);
            return new Path(r1.transform.position, r2.transform.position, r1, r2, dist, this.pathWidth);
        }

        private void Kruskals(List<Path> baseEdges, List<Room> objects)
        {
            DisjointSets<Room> disjointSets = new DisjointSets<Room>(objects);
            baseEdges.Sort((x, y) => x.Weight.CompareTo(y.Weight));
            foreach(Path edge in baseEdges)
            {
                if (!disjointSets.IsSameSet(edge.StartRoom, edge.EndRoom))
                {
                    disjointSets.Merge(edge.StartRoom, edge.EndRoom);
                    this.Edges.Add(edge);
                }
                else
                    this.ExtraEdges.Add(edge);
            }
        }
    }
}
