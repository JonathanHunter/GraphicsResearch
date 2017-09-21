namespace GraphicsResearch.DartThrowing
{
    using System.Collections.Generic;
    using UnityEngine;
    using MST;

    public class Rasterize : MonoBehaviour
    {
        [SerializeField]
        private bool showGrid;
        [SerializeField]
        private DartThrower dartThrower = null;
        [SerializeField]
        private VariableSpanningTree vst = null;
        [SerializeField]
        private Transform topLeft = null;
        [SerializeField]
        private int numRows = 0;
        [SerializeField]
        private int numCols = 0;
        [SerializeField]
        private float boxSize;
        [SerializeField]
        private MeshFilter meshFilter;

        public int[,] Rasterized { get; private set; }

        private void Start()
        {
            this.Rasterized = new int[this.numRows, this.numCols];
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.X))
            {
                RasterizeCircles(this.dartThrower.Circles);
                RasterizeLines(this.vst.Edges);
            }

            if (Input.GetKeyUp(KeyCode.Z))
            {
                GenerateMesh();
            }
        }

        private void OnDrawGizmos()
        {
            if (this.showGrid)
            {
                for (int r = 0; r < this.numRows; r++)
                {
                    for (int c = 0; c < this.numCols; c++)
                    {
                        Vector2 boxCenter = GetPos(r, c);
                        if (this.Rasterized[r, c] == 1)
                            Gizmos.DrawCube(boxCenter, new Vector2(this.boxSize, this.boxSize));
                        else
                            Gizmos.DrawWireCube(boxCenter, new Vector2(this.boxSize, this.boxSize));
                    }
                }
            }
        }

        private void RasterizeCircles(List<CircleDart> circles)
        {
            foreach (CircleDart circle in circles)
            {
                Vector2 topLeft = new Vector2(circle.transform.position.x - circle.Radius, circle.transform.position.y + circle.Radius);
                Vector2 bottomRight = new Vector2(circle.transform.position.x + circle.Radius, circle.transform.position.y - circle.Radius);
                int r1 = GetRow(topLeft);
                int r2 = GetRow(bottomRight);
                int c1 = GetCol(topLeft);
                int c2 = GetCol(bottomRight);
                for (int r = r1; r < r2; r++)
                {
                    for (int c = c1; c < c2; c++)
                    {
                        Vector2 pos = GetPos(r, c);
                        if (Vector2.Distance(pos, circle.transform.position) <= circle.Radius)
                            this.Rasterized[r, c] = 1;
                    }
                }
            }
        }

        private void RasterizeLines(List<GraphEdge> lines)
        {
            foreach(GraphEdge e in lines)
            {
                float change = this.boxSize / Vector2.Distance(e.Start.transform.position, e.End.transform.position);
                float lerp = 0;
                while(lerp <= 1)
                {
                    Vector2 pos = Vector2.Lerp(e.Start.transform.position, e.End.transform.position, lerp);
                    this.Rasterized[GetRow(pos), GetCol(pos)] = 1;
                    lerp += change;
                }
            }
        }

        private int GetRow(Vector2 position)
        {
            Vector2 pos = this.topLeft.position;
            float dist = position.x - pos.x + this.boxSize / 2f;
            int row = (int)(dist / this.boxSize);
            if (row < 0)
                return 0;
            else if (row >= this.numRows)
                return this.numRows - 1;
            else
                return row;
        }

        private int GetCol(Vector2 position)
        {
            Vector2 pos = this.topLeft.position;
            float dist = pos.y - position.y + this.boxSize / 2f;
            int col = (int)(dist / this.boxSize);
            if (col < 0)
                return 0;
            else if (col >= this.numCols)
                return this.numCols - 1;
            else
                return col;
        }

        private Vector2 GetPos(int r, int c)
        {
            Vector2 pos = this.topLeft.position;
            return pos + new Vector2(this.boxSize * r, -this.boxSize * c);
        }

        private void GenerateMesh()
        {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            for (int r = 0; r < this.numRows; r++)
            {
                for (int c = 0; c < this.numCols; c++)
                {
                    if (this.Rasterized[r, c] != 0)
                    {
                        Vector2 boxCenter = GetPos(r, c);
                        Vector3 tl = new Vector3(boxCenter.x - this.boxSize / 2f, boxCenter.y + this.boxSize / 2f);
                        Vector3 tr = new Vector3(boxCenter.x + this.boxSize / 2f, boxCenter.y + this.boxSize / 2f);
                        Vector3 bl = new Vector3(boxCenter.x - this.boxSize / 2f, boxCenter.y - this.boxSize / 2f);
                        Vector3 br = new Vector3(boxCenter.x + this.boxSize / 2f, boxCenter.y - this.boxSize / 2f);
                        vertices.Add(tl);
                        vertices.Add(tr);
                        vertices.Add(br);
                        vertices.Add(bl);
                        triangles.Add(vertices.Count - 4);
                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 4);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 1);
                    }
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            this.meshFilter.mesh = mesh;
        }
    }
}
