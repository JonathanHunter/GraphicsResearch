namespace GraphicsResearch.Generation.Meshes
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Grids;

    public class MeshGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject meshPrefab = null;

        [SerializeField]
        private bool extrudeMesh = false;

        [SerializeField]
        private bool invertTriangles = false;

        [SerializeField]
        private float zOffset = 1f;

        private bool InvertTriangles { get { return this.zOffset > 0 ? this.invertTriangles : !this.invertTriangles; } }

        public IEnumerator GenerateMesh(Floor f)
        {
            List<Vector3>[,] verts = new List<Vector3>[f.rasterizationGrid.SectorDimension.x, f.rasterizationGrid.SectorDimension.y];
            List<int>[,] tris = new List<int>[f.rasterizationGrid.SectorDimension.x, f.rasterizationGrid.SectorDimension.y];
            for (int r = 0; r < f.rasterizationGrid.SectorDimension.x; r++)
            {
                for (int c = 0; c < f.rasterizationGrid.SectorDimension.y; c++)
                {
                    verts[r, c] = new List<Vector3>();
                    tris[r, c] = new List<int>();
                    EvaluateGrid(new Vector2Int(r, c), f.Squares, f.rasterizationGrid, verts[r, c], tris[r, c]);
                    GameObject mesh = SpawnMesh(verts[r, c], tris[r, c]);
                    Vector3 pos = mesh.transform.position;
                    Quaternion rot = mesh.transform.rotation;
                    Vector3 size = mesh.transform.localScale;
                    mesh.transform.SetParent(f.meshParent);
                    mesh.transform.localPosition = pos;
                    mesh.transform.localRotation = rot;
                    mesh.transform.localScale = size;
                }
                yield return null;
            }
        }

        public IEnumerator GenerateMesh(MiddleLayer l)
        {
            List<Vector3>[,] verts = new List<Vector3>[l.rasterizationGrid.SectorDimension.x, l.rasterizationGrid.SectorDimension.y];
            List<int>[,] tris = new List<int>[l.rasterizationGrid.SectorDimension.x, l.rasterizationGrid.SectorDimension.y];
            for (int r = 0; r < l.rasterizationGrid.SectorDimension.x; r++)
            {
                for (int c = 0; c < l.rasterizationGrid.SectorDimension.y; c++)
                {
                    verts[r, c] = new List<Vector3>();
                    tris[r, c] = new List<int>();
                    EvaluateGrid(new Vector2Int(r, c), l.Squares, l.rasterizationGrid, verts[r, c], tris[r, c]);
                    GameObject mesh = SpawnMesh(verts[r, c], tris[r, c]);
                    Vector3 pos = mesh.transform.position;
                    Quaternion rot = mesh.transform.rotation;
                    Vector3 size = mesh.transform.localScale;
                    mesh.transform.SetParent(l.meshParent);
                    mesh.transform.localPosition = pos;
                    mesh.transform.localRotation = rot;
                    mesh.transform.localScale = size;
                }
                yield return null;
            }
        }

        private void EvaluateGrid(Vector2Int sector, Square[,,,] squares, RasterizationGrid grid, List<Vector3> vert, List<int> tri)
        {
            for (int r = 0; r < grid.GridDimension.x; r++)
            {
                for (int c = 0; c < grid.GridDimension.y; c++)
                {
                    if (squares[sector.x, sector.y, r, c].Filled)
                    {
                        List<int> triangles = squares[sector.x, sector.y, r, c].GetTriangles(vert, this.InvertTriangles);
                        foreach (int i in triangles)
                            tri.Add(i);
                    }
                }
            }

            if (this.extrudeMesh)
            {
                // Duplicate and offset mesh mesh
                int oldVerts = vert.Count;
                for(int i = 0; i < oldVerts; i++)
                {
                    Vector3 v = new Vector3(vert[i].x, vert[i].y, vert[i].z + this.zOffset);
                    vert.Add(v);
                }

                // Duplicate triangles in reverse order to invert
                int oldTris = tri.Count;
                for(int i = 0; i < oldTris - 2; i += 3)
                {
                    int a = tri[i] + oldVerts;
                    int b = tri[i + 1] + oldVerts;
                    int c = tri[i + 2] + oldVerts;
                    tri.Add(c);
                    tri.Add(b);
                    tri.Add(a);
                }

                for (int r = 0; r < grid.GridDimension.x; r++)
                {
                    for (int c = 0; c < grid.GridDimension.y; c++)
                    {
                        if (squares[sector.x, sector.y, r, c].Filled)
                        {
                            AddWalls(sector, new Vector2Int(r, c), squares, grid, vert, tri, oldVerts);
                        }
                    }
                }
            }
        }

        private void AddWalls(
            Vector2Int sector, 
            Vector2Int grid, 
            Square[,,,] squares, 
            RasterizationGrid rastGrid, 
            List<Vector3> vert, 
            List<int> tri, 
            int oldVerts)
        {
            bool left = grid.x == 0 || !squares[sector.x, sector.y, grid.x - 1, grid.y].Filled;
            bool right = grid.x == rastGrid.GridDimension.x - 1 || !squares[sector.x, sector.y, grid.x + 1, grid.y].Filled;
            bool up = grid.y == 0 || !squares[sector.x, sector.y, grid.x, grid.y - 1].Filled;
            bool down = grid.y == rastGrid.GridDimension.y - 1 || !squares[sector.x, sector.y, grid.x, grid.y + 1].Filled;

            if (grid.x == 0 && sector.x != 0)
                left = !squares[sector.x - 1, sector.y, rastGrid.GridDimension.x - 1, grid.y].Filled;

            if (grid.x == rastGrid.GridDimension.x - 1 && sector.x != rastGrid.SectorDimension.x - 1)
                right = !squares[sector.x + 1, sector.y, 0, grid.y].Filled;

            if (grid.y == 0 && sector.y != 0)
                up = !squares[sector.x, sector.y - 1, grid.x, rastGrid.GridDimension.y - 1].Filled;

            if (grid.y == rastGrid.GridDimension.y - 1 && sector.y != rastGrid.SectorDimension.y - 1)
                down = !squares[sector.x, sector.y + 1, grid.x, 0].Filled;
            
            List<int> walls = squares[sector.x, sector.y, grid.x, grid.y].GetWallPoints(up, down, left, right);

            for (int i = 0; i < walls.Count - 1; i += 2)
            {
                Vector3 pos1 = vert[walls[i] + oldVerts];
                int a = vert.Count;
                vert.Add(pos1);
                
                Vector3 pos2 = vert[walls[i + 1] + oldVerts];
                int b = vert.Count;
                vert.Add(pos2);
                
                Vector3 pos3 = vert[walls[i + 1]];
                int c = vert.Count;
                vert.Add(pos3);
                
                Vector3 pos4 = vert[walls[i]];
                int d = vert.Count;
                vert.Add(pos4);

                MeshUtility.AddSquare(tri, a, b, c, d, this.InvertTriangles);
            }
        }

        private GameObject SpawnMesh(List<Vector3> vert, List<int> tri)
        {
            GameObject mesh = Instantiate(this.meshPrefab);
            Mesh m = new Mesh()
            {
                vertices = vert.ToArray(),
                triangles = tri.ToArray()
            };
            m.RecalculateNormals();
            mesh.GetComponent<MeshFilter>().mesh = m;
            //mesh.GetComponent<MeshCollider>().sharedMesh = m;
            mesh.transform.parent = this.gameObject.transform;
            mesh.transform.localPosition = Vector3.zero;
            mesh.transform.localRotation = Quaternion.identity;
            mesh.transform.localScale = Vector3.one;
            return mesh;
        }
    }
}
