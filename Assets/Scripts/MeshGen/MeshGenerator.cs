namespace GraphicsResearch.MeshGen
{
    using System.Collections.Generic;
    using UnityEngine;

    public class MeshGenerator : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter meshFilter;
        [SerializeField]
        private Vector2 topLeft;
        [SerializeField]
        private float gridSize;

        private CellularAutomata ca;

        private void Start()
        {
           this.ca = new CellularAutomata(100);
        }

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.M))
            {
                GenerateMesh();
            }
        }

        private void GenerateMesh()
        {
            this.ca.GeneratePattern(30);
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            CalculateTriangles(vertices, triangles, this.ca.GetPattern());
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            this.meshFilter.mesh = mesh;
        }

        private void CalculateTriangles(List<Vector3> vertices, List<int> triangles, int[,] pattern)
        {
            for(int r = 0; r < pattern.GetLength(0); r++)
            {
                for(int c = 0; c < pattern.GetLength(1); c++)
                {
                    if (pattern[r, c] != 0)
                    {
                        AddSquare(vertices, triangles, r, c);
                    }
                }
            }
        }

        private void AddSquare(List<Vector3> vertices, List<int> triangles, int r, int c)
        {
            Vector2 boxCenter = this.topLeft + new Vector2(this.gridSize * r, -this.gridSize * c);
            Vector3 tl = new Vector3(boxCenter.x - this.gridSize / 2f, boxCenter.y + this.gridSize / 2f);
            Vector3 tr = new Vector3(boxCenter.x + this.gridSize / 2f, boxCenter.y + this.gridSize / 2f);
            Vector3 bl = new Vector3(boxCenter.x - this.gridSize / 2f, boxCenter.y - this.gridSize / 2f);
            Vector3 br = new Vector3(boxCenter.x + this.gridSize / 2f, boxCenter.y - this.gridSize / 2f);
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
