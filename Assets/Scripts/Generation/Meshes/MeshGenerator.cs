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
                        List<int> triangles = squares[sector.x, sector.y, r, c].GetTriangles(vert, this.invertTriangles);
                        foreach (int i in triangles)
                            tri.Add(i);
                    }
                }
            }

            if (this.extrudeMesh)
            {
                for (int r = 0; r < grid.GridDimension.x; r++)
                {
                    for (int c = 0; c < grid.GridDimension.y; c++)
                    {
                        if (squares[sector.x, sector.y, r, c].Filled)
                        {
                            //RasterizerUtil.AddWalls(
                            //    this.Grids,
                            //    rasterized,
                            //    dupe,
                            //    this.Vertices,
                            //    this.Triangles,
                            //    this.invertTriangles,
                            //    gridRow,
                            //    gridCol,
                            //    r,
                            //    c);
                        }
                    }
                }
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
