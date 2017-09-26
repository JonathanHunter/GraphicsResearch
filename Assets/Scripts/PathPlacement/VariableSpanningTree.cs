namespace GraphicsResearch.PathPlacement
{
    using System.Collections.Generic;
    using UnityEngine;
    using RoomPlacement;

    public class VariableSpanningTree : PathManager
    {
        [SerializeField]
        private bool showLines;

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
            Vector3 start = edge.Start.transform.position;
            Vector3 end = edge.End.transform.position;
            Vector3 es = Vector3.Normalize(start - end);
            end += .5f * es;
            Gizmos.DrawLine(start, end);
        }

        protected override void LocalInit()
        {
        }

        protected override void LocalPlacePaths(RoomManager rooms)
        {
            List<Path> baseEdges = new List<Path>();
            List<GameObject> objects = new List<GameObject>();
            foreach (CircleRoom c1 in rooms.CircleRooms)
            {
                AddUnobstructedEdges(baseEdges, c1, rooms.CircleRooms);
                objects.Add(c1.gameObject);
            }

            Kruskals(baseEdges, objects);
        }

        protected override void LocalClear()
        {
        }

        private void AddUnobstructedEdges(List<Path> baseEdges, CircleRoom c1, List<CircleRoom> circles)
        {
            foreach (CircleRoom c2 in circles)
            {
                if (c1 != c2)
                {
                    if (!IsBlocked(c1, c2))
                    {
                        bool duplicate = false;
                        foreach(Path e in baseEdges)
                        {
                            if ((e.Start == c1.gameObject && e.End == c2.gameObject) ||
                                (e.Start == c2.gameObject && e.End == c1.gameObject))
                                duplicate = true;
                        }
                        if(!duplicate)
                            baseEdges.Add(CreateEdge(c1, c2));
                    }
                }
            }
        }

        private bool IsBlocked(CircleRoom c1, CircleRoom c2)
        {
            Vector3 c12 = c2.transform.position - c1.transform.position;
            RaycastHit[] hits = Physics.RaycastAll(c1.transform.position, c12.normalized, c12.magnitude);
            int hitCount = hits.Length;
            foreach(RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == c1.gameObject || hit.collider.gameObject == c2.gameObject)
                    hitCount--;
            }

            return hitCount > 0;
        }

        private Path CreateEdge(CircleRoom c1, CircleRoom c2)
        {
            float dist = Vector3.Distance(c1.transform.position, c2.transform.position);
            dist -= c1.Radius + c2.Radius;
            return new Path(c1.gameObject, c2.gameObject, dist);
        }

        private void Kruskals(List<Path> baseEdges, List<GameObject> objects)
        {
            DisjointSets<GameObject> disjointSets = new DisjointSets<GameObject>(objects);
            baseEdges.Sort((x, y) => x.Weight.CompareTo(y.Weight));
            foreach(Path edge in baseEdges)
            {
                if (!disjointSets.IsSameSet(edge.Start, edge.End))
                {
                    disjointSets.Merge(edge.Start, edge.End);
                    this.Edges.Add(edge);
                }
                else
                    this.ExtraEdges.Add(edge);
            }
        }
    }
}
