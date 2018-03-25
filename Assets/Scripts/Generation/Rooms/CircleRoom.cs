namespace GraphicsResearch.Generation.Rooms
{
    using System.Collections.Generic;
    using UnityEngine;
    using Meshes;

    public class CircleRoom : Room
    {
        [SerializeField]
        private float radius;
        [SerializeField]
        private MeshFilter meshFilter;

        private List<Vector3> vertices;
        private List<int> faces;
        private List<Corner> topDisk;
        private List<Corner> bottomDisk;
        private List<Corner> walls;

        public float Radius { get { return this.radius; } internal set { this.radius = value; } }

        public void Init()
        {
            Create(.5f, .5f, true);
        }

        private void Create(float radius, float height, bool inverted)
        {
            this.vertices = new List<Vector3>();
            this.faces = new List<int>();
            this.topDisk = new List<Corner>();
            this.bottomDisk = new List<Corner>();
            this.walls = new List<Corner>();

            CreateDisk(radius, height);
            CreateWalls();

            foreach (Corner corner in this.topDisk)
                corner.AssignVertex(this.vertices);
            foreach (Corner corner in this.bottomDisk)
                corner.AssignVertex(this.vertices);
            foreach (Corner corner in this.walls)
                corner.AssignVertex(this.vertices);

            for (int i = 1; i < this.topDisk.Count - 1; i++)
            {
                GenerationUtilityFunctions.AddTriangle(this.faces, this.topDisk[0].VertexIndex, this.topDisk[i].VertexIndex, this.topDisk[i + 1].VertexIndex, inverted);
                GenerationUtilityFunctions.AddTriangle(this.faces, this.bottomDisk[0].VertexIndex, this.bottomDisk[i].VertexIndex, this.bottomDisk[i + 1].VertexIndex, !inverted);
            }

            GenerationUtilityFunctions.AddTriangle(this.faces, this.topDisk[0].VertexIndex, this.topDisk[this.topDisk.Count - 1].VertexIndex, this.topDisk[1].VertexIndex, inverted);
            GenerationUtilityFunctions.AddTriangle(this.faces, this.bottomDisk[0].VertexIndex, this.bottomDisk[this.bottomDisk.Count - 1].VertexIndex, this.bottomDisk[1].VertexIndex, !inverted);

            int a, b, c, d;
            for (int i = 0; i < this.topDisk.Count - 2; i++)
            {
                a = this.walls[2 * i].VertexIndex;
                b = this.walls[2 * i + 1].VertexIndex;
                c = this.walls[2 * (i + 1)].VertexIndex;
                d = this.walls[2 * (i + 1) + 1].VertexIndex;
                GenerationUtilityFunctions.AddSquare(this.faces, b, d, c, a, inverted);
            }

            a = this.walls[2 * (this.topDisk.Count - 2)].VertexIndex;
            b = this.walls[2 * (this.topDisk.Count - 2) + 1].VertexIndex;
            c = this.walls[0].VertexIndex;
            d = this.walls[1].VertexIndex;
            GenerationUtilityFunctions.AddSquare(this.faces, b, d, c, a, inverted);

            Mesh mesh = new Mesh();
            mesh.vertices = this.vertices.ToArray();
            mesh.triangles = this.faces.ToArray();
            mesh.RecalculateNormals();
            this.meshFilter.mesh = mesh;
        }

        private void CreateDisk(float radius, float height)
        {
            Vector3 center = Vector3.zero;
            this.topDisk.Add(new Corner(center));
            this.bottomDisk.Add(new Corner(center + Vector3.back * height));

            Vector3 pos;
            for (int i = 0; i < 360; i += 5)
            {
                pos = center + new Vector3(Mathf.Cos(Mathf.Deg2Rad * i), Mathf.Sin(Mathf.Deg2Rad * i), 0) * radius;
                this.topDisk.Add(new Corner(pos));
                this.bottomDisk.Add(new Corner(pos + Vector3.back * height));
            }
        }

        private void CreateWalls()
        {
            for (int i = 1; i < this.topDisk.Count; i++)
            {
                this.walls.Add(new Corner(this.topDisk[i].Position));
                this.walls.Add(new Corner(this.bottomDisk[i].Position));
            }
        }
    }
}
