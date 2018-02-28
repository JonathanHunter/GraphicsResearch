namespace GraphicsResearch.MultiFloorGeneration
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using PathPlacement;
    using RoomPlacement;
    using System;

    public class MultiFloorManager : MonoBehaviour
    {
        public GameObject meshPrefab;
        public Vector2 gradientRange;
        public Vector2 distanceRange;
        public bool drawPaths;
        public bool drawHandles;
        public Transform topLeft = null;
        public Vector2Int gridDim;
        public Vector2Int subGridDim;
        public Vector2 boxSize;
        public bool invertTriangles;

        public bool showGrid;
        public Vector2Int point;

        private HallwayRasterizer rast;

        private List<MulitFloorPath> paths;
        private List<GameObject> spawnedMeshes;

        public void Init()
        {
            this.paths = new List<MulitFloorPath>();
            this.spawnedMeshes = new List<GameObject>();
        }

        private void OnDrawGizmos()
        {
            if (this.paths != null)
            {
                foreach (MulitFloorPath p in this.paths)
                {
                    if(this.drawPaths)
                        p.Draw();
                    if(this.drawHandles)
                        Handles.Label(Vector3.Lerp(p.Room1.transform.position, p.Room2.transform.position, .5f), 
                            "Gradient: " + p.Gradient + "\nDistance: " + p.Distance);
                }
            }

            if(showGrid)
            {
                rast.DrawGrid(point);
            }
        }

        public void FindRoomPairs(RoomManager rooms1, RoomManager rooms2)
        {
            foreach (Room r1 in rooms1.Rooms)
            {
                foreach(Room r2 in rooms2.Rooms)
                {
                    CompareRoom(r1, r2);
                }
            }
        }

        public IEnumerator FindRoomPairsAsync(RoomManager rooms1, RoomManager rooms2)
        {
            foreach (Room r1 in rooms1.Rooms)
            {
                foreach (Room r2 in rooms2.Rooms)
                {
                    CompareRoom(r1, r2);
                }

                yield return null;
            }

            yield return null;
        }

        public void RasterizeHallways()
        {
            foreach(MulitFloorPath p in this.paths)
            {
                HallwayRasterizer rasterizer = new HallwayRasterizer(this.gridDim, this.subGridDim, this.boxSize, this.topLeft, this.invertTriangles);
                rasterizer.RasterizeCircle((CircleRoom)p.Room1);
                rasterizer.RasterizeCircle((CircleRoom)p.Room2);
                rasterizer.RasterizePath(p);
                Vector3 r21 = Vector3.Normalize(p.Room1.OriginalPosition - p.Room2.OriginalPosition);
                Vector3 start = p.Room1.OriginalPosition - r21 * p.Distance * .01f;
                Vector3 end = p.Room2.OriginalPosition + r21 * p.Distance * .01f;
                rasterizer.MarkForKeeping(start, end, p.Width);
                rasterizer.GenerateMesh(true);
                rasterizer.RaiseMesh(start, end, p.Width, p.Room1.transform.position, p.Room2.transform.position);
                rasterizer.ExtrudeMesh(true);
                for (int r = 0; r < this.gridDim.x; r++)
                {
                    for (int c = 0; c < this.gridDim.y; c++)
                    {
                        if (rasterizer.Triangles[r, c].Count > 0)
                            SpawnMesh(rasterizer.Vertices[r, c], rasterizer.Triangles[r, c]);
                    }
                }

                this.rast = rasterizer;
            }
        }

        private void CompareRoom(Room r1, Room r2)
        {
            float gradient = GetGradient(r1, r2);
            float distance = GetDistance(r1, r2);
            Vector3 dir = Vector3.Normalize(r1.transform.position - r2.transform.position);
            RaycastHit[] hits = Physics.RaycastAll(r1.transform.position, dir, distance);
            bool blocked = false;
            foreach (RaycastHit rh in hits)
            {
                if (rh.collider.gameObject != r1 && rh.collider.gameObject != r2)
                    blocked = true;
            }

            if (!blocked)
            {
                if (gradient >= this.gradientRange.x && gradient <= this.gradientRange.y &&
                    distance >= this.distanceRange.x && distance <= this.distanceRange.y)
                {
                    MulitFloorPath p = new MulitFloorPath(r1, r2, gradient, distance);
                    this.paths.Add(p);
                }
            }
        }

        private float GetGradient(Room r1, Room r2)
        {
            Vector3 refPoint = new Vector3(r1.transform.position.x, r2.transform.position.y, r1.transform.position.z);
            Vector3 r21 = Vector3.Normalize(r1.transform.position - r2.transform.position);
            Vector3 r2ref = Vector3.Normalize(refPoint - r2.transform.position);
            return Vector3.Angle(r2ref, r21);
        }

        private float GetDistance(Room r1, Room r2)
        {
            return Vector3.Distance(r1.transform.position, r2.transform.position);
        }

        private void SpawnMesh(List<Vector3> vertices, List<int> triangles)
        {
            GameObject mesh = Instantiate(this.meshPrefab);
            MeshFilter filter = mesh.GetComponent<MeshFilter>();
            MeshCollider collider = mesh.GetComponent<MeshCollider>();
            Mesh m = new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
            };

            m.RecalculateNormals();
            filter.mesh = m;
            collider.sharedMesh = m;
            mesh.transform.parent = this.gameObject.transform;
            mesh.transform.localPosition = Vector3.zero;
            mesh.transform.localRotation = Quaternion.identity;
            mesh.transform.localScale = Vector3.one;
            this.spawnedMeshes.Add(mesh);
        }
    }
}
