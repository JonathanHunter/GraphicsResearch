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
            Vector3 start = edge.Start.transform.position;
            Vector3 end = edge.End.transform.position;
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
            List<GameObject> objects = new List<GameObject>();
            List<GameObject> roomLists = new List<GameObject>();
            foreach (CircleRoom c in rooms.CircleRooms)
                roomLists.Add(c.gameObject);

            foreach (RectangleRoom r in rooms.RectangleRooms)
                roomLists.Add(r.gameObject);

            foreach (GameObject c1 in roomLists)
            {
                AddUnobstructedEdges(baseEdges, c1, roomLists);
                objects.Add(c1.gameObject);
            }

            Kruskals(baseEdges, objects);
        }

        protected override void LocalClear()
        {
        }

        private void AddUnobstructedEdges(List<Path> baseEdges, GameObject c1, List<GameObject> circles)
        {
            foreach (GameObject c2 in circles)
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

        private bool IsBlocked(GameObject c1, GameObject c2)
        {
            Vector3 c12 = c2.transform.position - c1.transform.position;
            Vector3 left = new Vector3(-c12.y, c12.x, c12.z).normalized;
            RaycastHit[] hits = Physics.RaycastAll(c1.transform.position + left * this.pathWidth / 2f, c12.normalized, c12.magnitude);
            int hitCount = hits.Length;
            foreach(RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == c1.gameObject || hit.collider.gameObject == c2.gameObject)
                    hitCount--;
            }

            hits = Physics.RaycastAll(c1.transform.position - left * this.pathWidth / 2f, c12.normalized, c12.magnitude);
            hitCount += hits.Length;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == c1.gameObject || hit.collider.gameObject == c2.gameObject)
                    hitCount--;
            }

            return hitCount > 0;
        }

        private Path CreateEdge(GameObject c1, GameObject c2)
        {
            float dist = Vector3.Distance(c1.transform.position, c2.transform.position);
            return new Path(c1.gameObject, c2.gameObject, dist, this.pathWidth);
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
