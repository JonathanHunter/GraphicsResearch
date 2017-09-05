namespace GraphicsResearch.MST
{
    using System.Collections.Generic;
    using UnityEngine;
    using DartThrowing;

    public class VariableSpanningTree : MonoBehaviour
    {
        [SerializeField]
        private DartThrower dartThrower;
        [SerializeField]
        [Range(0f, 1f)]
        private float edgeAmount;
        [SerializeField]
        private bool showArrows;

        public List<GraphEdge> Edges { get; private set; }

        private List<GraphEdge> rejectedEdges;

        private void Start()
        {
            this.Edges = new List<GraphEdge>();
        }

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.M))
            {
                CreateGraph();
            }
            if(Input.GetKeyUp(KeyCode.R))
            {
                if(this.rejectedEdges != null)
                {
                    int n = this.rejectedEdges.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = Random.Range(0, n + 1);
                        GraphEdge edge = this.rejectedEdges[k];
                        this.rejectedEdges[k] = this.rejectedEdges[n];
                        this.rejectedEdges[n] = edge;
                    }
                }
            }
            if(Input.GetKeyUp(KeyCode.S))
            {
                this.rejectedEdges.Sort((x, y) => x.Weight.CompareTo(y.Weight));
            }
        }

        private void OnDrawGizmos()
        {
            if(showArrows && this.Edges != null)
            {
                foreach(GraphEdge edge in this.Edges)
                    DrawArrow(edge);

                int numToAdd = (int)(rejectedEdges.Count * this.edgeAmount);
                for (int i = 0; i < numToAdd; i++)
                    DrawArrow(rejectedEdges[i]);
            }
        }

        private void DrawArrow(GraphEdge edge)
        {
            Vector3 start = edge.Start.transform.position;
            Vector3 end = edge.End.transform.position;
            Vector3 es = Vector3.Normalize(start - end);
            Vector3 left = new Vector3(-es.y, es.x);
            Vector3 leftDiagonal = Vector3.Normalize(es + left / 2f) / 3f;
            Vector3 rightDiagonal = Vector3.Normalize(es - left / 2f) / 3f;
            end += .5f * es;
            Gizmos.DrawLine(start, end);
            //Gizmos.DrawLine(end, end + leftDiagonal);
            //Gizmos.DrawLine(end, end + rightDiagonal);
        }

        private void CreateGraph()
        {
            List<GraphEdge> baseEdges = new List<GraphEdge>();
            List<CircleDart> circles = this.dartThrower.Circles;
            List<GameObject> objects = new List<GameObject>();
            foreach(CircleDart c1 in circles)
            {
                AddUnobstructedEdges(baseEdges, c1, circles);
                objects.Add(c1.gameObject);
            }
                    
            this.Edges = Kruskals(baseEdges, objects, out this.rejectedEdges);
        }

        private void AddUnobstructedEdges(List<GraphEdge> baseEdges, CircleDart c1, List<CircleDart> circles)
        {
            foreach (CircleDart c2 in circles)
            {
                if (c1 != c2)
                {
                    if (!IsBlocked(c1, c2))
                    {
                        bool duplicate = false;
                        foreach(GraphEdge e in baseEdges)
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

        private bool IsBlocked(CircleDart c1, CircleDart c2)
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

        private GraphEdge CreateEdge(CircleDart c1, CircleDart c2)
        {
            float dist = Vector3.Distance(c1.transform.position, c2.transform.position);
            dist -= c1.Radius + c2.Radius;
            return new GraphEdge(c1.gameObject, c2.gameObject, dist);
        }

        private List<GraphEdge> Kruskals(List<GraphEdge> baseEdges, List<GameObject> objects, out List<GraphEdge> rejectedEdges)
        {
            List<GraphEdge> MST = new List<GraphEdge>();
            DisjointSets<GameObject> disjointSets = new DisjointSets<GameObject>(objects);
            rejectedEdges = new List<GraphEdge>();
            baseEdges.Sort((x, y) => x.Weight.CompareTo(y.Weight));
            foreach(GraphEdge edge in baseEdges)
            {
                if (!disjointSets.IsSameSet(edge.Start, edge.End))
                {
                    disjointSets.Merge(edge.Start, edge.End);
                    MST.Add(edge);
                }
                else
                    rejectedEdges.Add(edge);
            }

            return MST;
        }
    }
}
