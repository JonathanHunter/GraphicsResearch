namespace GraphicsResearch.MultiFloorGeneration
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using RoomPlacement;
    using PathPlacement;
    using MeshGeneration;
    using Util;

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
                    if (this.drawPaths)
                        p.Draw();
                    if (this.drawHandles)
                        Handles.Label(Vector3.Lerp(p.Room1.transform.position, p.Room2.transform.position, .5f),
                            "Gradient: " + p.Gradient + "\nDistance: " + p.Distance);
                }
            }

            if (showGrid)
            {
                rast.DrawGrid(point);
            }
        }

        public void FindRoomPairs(RoomManager rooms1, RoomManager rooms2, PathManager paths1, PathManager paths2)
        {
            foreach (Room r1 in rooms1.Rooms)
            {
                foreach (Room r2 in rooms2.Rooms)
                {
                    CompareRoom(r1, r2, rooms1, rooms2, paths1, paths2);
                }
            }
        }

        public IEnumerator FindRoomPairsAsync(RoomManager rooms1, RoomManager rooms2, PathManager paths1, PathManager paths2)
        {
            foreach (Room r1 in rooms1.Rooms)
            {
                foreach (Room r2 in rooms2.Rooms)
                {
                    CompareRoom(r1, r2, rooms1, rooms2, paths1, paths2);
                }

                yield return null;
            }

            yield return null;
        }

        public void RasterizeHallways(MeshManager floor1, MeshManager floor2)
        {
            HallwayRasterizer rasterizer = new HallwayRasterizer(this.gridDim, this.subGridDim, this.boxSize, this.topLeft, this.invertTriangles);

            foreach (MulitFloorPath p in this.paths)
            {
                if (p.Room1 is CircleRoom)
                    rasterizer.RasterizeCircle((CircleRoom)p.Room1);
                else
                    rasterizer.RasterizeRectangle((RectangleRoom)p.Room1);

                if (p.Room2 is CircleRoom)
                    rasterizer.RasterizeCircle((CircleRoom)p.Room2);
                else
                    rasterizer.RasterizeRectangle((RectangleRoom)p.Room2);

                rasterizer.RasterizePath(p);
                Vector3 start = GetRoomEdgePoint(p.Room1, p);
                Vector3 end = GetRoomEdgePoint(p.Room2, p);
                rasterizer.MarkForKeeping(start, end, p.Width);
                floor1.ReserveGridSquares(start, Vector3.Lerp(start, end, .02f), p.Width);
                floor2.ReserveGridSquares(Vector3.Lerp(start, end, .98f), end, p.Width);
                Vector3 startHeight = this.transform.InverseTransformPoint(p.Room1.transform.position);
                Vector3 endHeight = this.transform.InverseTransformPoint(p.Room2.transform.position);
                rasterizer.RaiseMesh(start, end, p.Width, startHeight, endHeight);
            }

            rasterizer.GenerateMesh(true);
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

        private void CompareRoom(Room r1, Room r2, RoomManager rooms1, RoomManager rooms2, PathManager paths1, PathManager paths2)
        {
            float gradient = GetGradient(r1, r2);
            float distance = GetDistance(r1, r2);
            bool blocked = false;
            Vector3 start = r1.OriginalPosition;
            Vector3 end = r2.OriginalPosition;
            Vector3 mid = Vector3.Lerp(start, end, .5f);

            foreach (Room r in rooms1.Rooms)
            {
                if (r1 != r)
                {
                    if (CheckIntersection(start, mid, r))
                    {
                        blocked = true;
                        break;
                    }
                }
            }

            //if(!blocked)
            //{
            //    foreach(Path p in paths1.Paths)
            //    {
            //        foreach (Edge e in p.Edges)
            //        {
            //            if(Lib.LineLineIntersection(e.Start, e.End, start, mid) != Vector2.zero)
            //            {
            //                blocked = true;
            //                break;
            //            }
            //        }
            //    }
            //}

            if (!blocked)
            {
                foreach (Room r in rooms2.Rooms)
                {
                    if (r2 != r)
                    {
                        if (CheckIntersection(end, mid, r))
                        {
                            blocked = true;
                            break;
                        }
                    }
                }
            }

            //if (!blocked)
            //{
            //    foreach (Path p in paths2.Paths)
            //    {
            //        foreach (Edge e in p.Edges)
            //        {
            //            if (Lib.LineLineIntersection(e.Start, e.End, end, mid) != Vector2.zero)
            //            {
            //                blocked = true;
            //                break;
            //            }
            //        }
            //    }
            //}

            if (!blocked)
            {
                foreach (MulitFloorPath p in this.paths)
                {
                    if (Lib.LineLineIntersection(
                        r1.transform.position,
                        r2.transform.position,
                        p.Room1.transform.position,
                        p.Room2.transform.position) != Vector2.zero)
                    {
                        blocked = true;
                        break;
                    }
                }
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
            //collider.sharedMesh = m;
            mesh.transform.parent = this.gameObject.transform;
            mesh.transform.localPosition = Vector3.zero;
            mesh.transform.localRotation = Quaternion.identity;
            mesh.transform.localScale = Vector3.one;
            this.spawnedMeshes.Add(mesh);
        }

        private Vector3 GetRoomEdgePoint(Room room, MulitFloorPath p)
        {
            if (room is CircleRoom)
            {
                CircleRoom c = (CircleRoom)room;
                return Lib.CircleLineIntersection(p.Room1.OriginalPosition, p.Room2.OriginalPosition, c.OriginalPosition, c.Radius);
            }
            else
            {
                RectangleRoom r = (RectangleRoom)room;
                Vector3 start, end;
                float width;
                RasterizerUtil.GetSquareBounds(r, out start, out end, out width);
                Vector3 es = Vector3.Normalize(start - end);
                Vector3 left = new Vector3(-es.y, es.x, es.z);
                Vector3 tl = start + left * width / 2f;
                Vector3 tr = start - left * width / 2f;
                Vector3 bl = end + left * width / 2f;
                Vector3 br = end - left * width / 2f;
                return Lib.BoxLineIntersection(p.Room1.OriginalPosition, p.Room2.OriginalPosition, tl, tr, bl, br);
            }
        }

        private bool CheckIntersection(Vector3 start, Vector3 end, Room room)
        {
            if (room is CircleRoom)
            {
                CircleRoom c = (CircleRoom)room;
                return Lib.CircleLineIntersection(start, end, c.OriginalPosition, c.Radius) != Vector2.zero;
            }
            else
            {
                RectangleRoom r = (RectangleRoom)room;
                Vector3 s, e;
                float width;
                RasterizerUtil.GetSquareBounds(r, out s, out e, out width);
                Vector3 es = Vector3.Normalize(s - e);
                Vector3 left = new Vector3(-es.y, es.x, es.z);
                Vector3 tl = s + left * width / 2f;
                Vector3 tr = s - left * width / 2f;
                Vector3 bl = e + left * width / 2f;
                Vector3 br = e - left * width / 2f;
                return Lib.BoxLineIntersection(start, end, tl, tr, bl, br) != Vector2.zero;
            }
        }
    }
}
