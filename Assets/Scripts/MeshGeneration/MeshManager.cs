namespace GraphicsResearch.MeshGeneration
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using PathPlacement;
    using RoomPlacement;

    public abstract class MeshManager : MonoBehaviour
    {
        [SerializeField]
        private int gridRows = 0;
        [SerializeField]
        private int gridCols = 0;
        [SerializeField]
        private GameObject meshPrefab = null;

        /// <summary> The generated meshes. </summary>
        public Mesh[,] Meshes { get; protected set; }
        /// <summary> The vertices of the generated meshes. </summary>
        public List<Vector3>[,] Vertices { get; protected set; }
        /// <summary> The triangles of the generated meshes. </summary>
        public List<int>[,] Triangles { get; protected set; }
        /// <summary> The number of rows in the mesh matrix. </summary>
        public int GridRows { get { return this.gridRows; } }
        /// <summary> The number of cols in the mesh matrix. </summary>
        public int GridCols { get { return this.gridCols; } }

        private List<GameObject> spawnedMeshes;


        /// <summary> Initializes this mesh manager. </summary>
        public void Init()
        {
            this.Meshes = new Mesh[this.GridRows, this.GridCols];
            this.Vertices= new List<Vector3>[this.GridRows, this.GridCols];
            this.Triangles = new List<int>[this.GridRows, this.GridCols];
            for (int r = 0; r < this.GridRows; r++)
            {
                for (int c = 0; c < this.GridCols; c++)
                {
                    this.Meshes[r, c] = new Mesh();
                    this.Vertices[r, c] = new List<Vector3>();
                    this.Triangles[r, c] = new List<int>();
                }
            }

            this.spawnedMeshes = new List<GameObject>();
            LocalInit();
        }

        /// <summary> Calculates the mesh. </summary>
        /// <param name="rooms"> The rooms to add to the mesh. </param>
        /// <param name="paths"> The paths to add to the mesh. </param>
        public void CalculateMesh(RoomManager rooms, PathManager paths)
        {
            LocalCalculateMesh(rooms, paths);
        }

        /// <summary> Calculates the mesh. </summary>
        /// <param name="rooms"> The rooms to add to the mesh. </param>
        /// <param name="paths"> The paths to add to the mesh. </param>
        public IEnumerator CalculateMeshAsync(RoomManager rooms, PathManager paths)
        {
            yield return StartCoroutine(LocalCalculateMeshAsync(rooms, paths));

        }

        /// <summary> Creates the calculated mesh. </summary>
        public void CreateMesh()
        {
            LocalCreateMesh();
            for(int r = 0; r < this.GridRows; r++)
            {
                for(int c = 0; c < this.GridCols; c++)
                {
                    SpawnMesh(r, c);
                }
            }
        }

        /// <summary> Creates the calculated mesh. </summary>
        public IEnumerator CreateMeshAsync()
        {
            yield return StartCoroutine(LocalCreateMeshAsync());
            for (int r = 0; r < this.GridRows; r++)
            {
                for (int c = 0; c < this.GridCols; c++)
                {
                    SpawnMesh(r, c);
                }

                yield return null;
            }

            yield return null;
        }

        /// <summary> Generates a mesh from the given rooms and paths. </summary>
        /// <param name="rooms"> The rooms to add to the mesh. </param>
        /// <param name="paths"> The paths to add to the mesh. </param>
        public void GenerateMesh(RoomManager rooms, PathManager paths)
        {
            Clear();
            LocalReserveGridSquares(rooms, paths);
            CalculateMesh(rooms, paths);
            CreateMesh();
        }

        /// <summary> Generates a mesh from the given rooms and paths. </summary>
        /// <param name="rooms"> The rooms to add to the mesh. </param>
        /// <param name="paths"> The paths to add to the mesh. </param>
        public IEnumerator GenerateMeshAsync(RoomManager rooms, PathManager paths)
        {
            Clear();
            LocalReserveGridSquares(rooms, paths);
            yield return StartCoroutine(CalculateMeshAsync(rooms, paths));
            yield return StartCoroutine(CreateMeshAsync());
        }

        /// <summary> Clears the current mesh. </summary>
        public void Clear()
        {
            foreach (GameObject g in spawnedMeshes)
                Destroy(g);

            spawnedMeshes.Clear();
            for (int r = 0; r < this.GridRows; r++)
            {
                for (int c = 0; c < this.GridCols; c++)
                {
                    this.Meshes[r, c].Clear();
                    this.Vertices[r, c].Clear();
                    this.Triangles[r, c].Clear();
                }
            }

            LocalClear();
        }

        /// <summary> Local handler for initialization. </summary>
        protected abstract void LocalInit();
        protected abstract void LocalReserveGridSquares(RoomManager rooms, PathManager paths);
        /// <summary> Local handler for Calculating the mesh. </summary>
        protected abstract void LocalCalculateMesh(RoomManager rooms, PathManager paths);
        /// <summary> Local async handler for Calculating the mesh. </summary>
        protected abstract IEnumerator LocalCalculateMeshAsync(RoomManager rooms, PathManager paths);
        /// <summary> Local handler for Creating the mesh. </summary>
        protected abstract void LocalCreateMesh();
        /// <summary> Local async handler for Creating the mesh. </summary>
        protected abstract IEnumerator LocalCreateMeshAsync();
        /// <summary> Local handler for mesh clearing. </summary>
        protected abstract void LocalClear();

        private void SpawnMesh(int r, int c)
        {
            GameObject mesh = Instantiate(this.meshPrefab);
            MeshFilter filter = mesh.GetComponent<MeshFilter>();
            MeshCollider collider = mesh.GetComponent<MeshCollider>();
            this.Meshes[r, c].vertices = this.Vertices[r, c].ToArray();
            this.Meshes[r, c].triangles = this.Triangles[r, c].ToArray();
            this.Meshes[r, c].RecalculateNormals();
            filter.mesh = this.Meshes[r, c];
            collider.sharedMesh = this.Meshes[r, c];
            mesh.transform.parent = this.gameObject.transform;
            mesh.transform.localPosition = Vector3.zero;
            mesh.transform.localRotation = Quaternion.identity;
            mesh.transform.localScale = Vector3.one;
            this.spawnedMeshes.Add(mesh);
        }
    }
}
