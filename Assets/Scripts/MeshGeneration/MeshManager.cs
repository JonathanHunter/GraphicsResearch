namespace GraphicsResearch.MeshGeneration
{
    using System.Collections.Generic;
    using UnityEngine;
    using PathPlacement;
    using RoomPlacement;

    public abstract class MeshManager : MonoBehaviour
    {
        /// <summary> The MeshFilter used to store the generated mesh. </summary>
        [SerializeField]
        protected MeshFilter meshFilter;
        /// <summary> The collider to use with the generated mesh. </summary>
        [SerializeField]
        protected MeshCollider meshCollider;

        /// <summary> The generated mesh. </summary>
        public Mesh DungeonMesh { get; protected set; }
        /// <summary> The vertices of the generated mesh. </summary>
        public List<Vector3> Vertices { get; protected set; }
        /// <summary> The triangles of the generated mesh. </summary>
        public List<int> Triangles { get; protected set; }

        /// <summary> Initializes this mesh manager. </summary>
        public void Init()
        {
            this.DungeonMesh = new Mesh();
            this.Vertices= new List<Vector3>();
            this.Triangles = new List<int>();
            LocalInit();
        }

        /// <summary> Calculates the mesh. </summary>
        /// <param name="rooms"> The rooms to add to the mesh. </param>
        /// <param name="paths"> The paths to add to the mesh. </param>
        public void CalculateMesh(RoomManager rooms, PathManager paths)
        {
            LocalCalculateMesh(rooms, paths);
        }

        /// <summary> Creates the calculated mesh. </summary>
        public void CreateMesh()
        {
            LocalCreateMesh();
            this.DungeonMesh.vertices = this.Vertices.ToArray();
            this.DungeonMesh.triangles = this.Triangles.ToArray();
            this.DungeonMesh.RecalculateNormals();
            this.meshFilter.mesh = this.DungeonMesh;
            this.meshCollider.sharedMesh = this.DungeonMesh;
        }

        /// <summary> Generates a mesh from the given rooms and paths. </summary>
        /// <param name="rooms"> The rooms to add to the mesh. </param>
        /// <param name="paths"> The paths to add to the mesh. </param>
        public void GenerateMesh(RoomManager rooms, PathManager paths)
        {
            Clear();
            CalculateMesh(rooms, paths);
            CreateMesh();
        }

        /// <summary> Clears the current mesh. </summary>
        public void Clear()
        {
            this.meshFilter.mesh = null;
            this.DungeonMesh.Clear();
            this.Vertices.Clear();
            this.Triangles.Clear();
            LocalClear();
        }

        /// <summary> Local handler for initialization. </summary>
        protected abstract void LocalInit();
        /// <summary> Local handler for Calculating the mesh. </summary>
        protected abstract void LocalCalculateMesh(RoomManager rooms, PathManager paths);
        /// <summary> Local handler for Creating the mesh. </summary>
        protected abstract void LocalCreateMesh();
        /// <summary> Local handler for mesh clearing. </summary>
        protected abstract void LocalClear();
    }
}
