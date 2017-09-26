namespace GraphicsResearch.MeshGeneration
{
    using System.Collections.Generic;
    using UnityEngine;
    using PathPlacement;
    using RoomPlacement;
    using Util;

    public class Rasterize : MeshManager
    {
        [SerializeField]
        private bool showGrid;
        [SerializeField]
        private Transform topLeft = null;
        [SerializeField]
        private int numRows = 0;
        [SerializeField]
        private int numCols = 0;
        [SerializeField]
        private float boxSize;

        public Grid2D<int> Rasterized { get; private set; }
        
        private void OnDrawGizmos()
        {
            if (this.showGrid)
            {
                for (int r = 0; r < this.numRows; r++)
                {
                    for (int c = 0; c < this.numCols; c++)
                    {
                        Vector2 boxCenter = this.Rasterized.GetPos(r, c);
                        if (this.Rasterized != null && this.Rasterized.Get(r,c) == 1)
                            Gizmos.DrawCube(boxCenter, new Vector2(this.boxSize, this.boxSize));
                        else
                            Gizmos.DrawWireCube(boxCenter, new Vector2(this.boxSize, this.boxSize));
                    }
                }
            }
        }


        protected override void LocalInit()
        {
            this.Rasterized = new Grid2D<int>(this.numRows, this.numCols, this.topLeft, new Vector2(this.boxSize, this.boxSize));
        }

        protected override void LocalCalculateMesh(RoomManager rooms, PathManager paths)
        {
            RasterizeCircles(rooms.CircleRooms);
            RasterizeLines(paths.GetPaths());
        }

        protected override void LocalCreateMesh()
        {
            for (int r = 0; r < this.numRows; r++)
            {
                for (int c = 0; c < this.numCols; c++)
                {
                    if (this.Rasterized.Get(r, c) != 0)
                    {
                        Vector2 boxCenter = this.Rasterized.GetPos(r, c);
                        Vector3 tl = new Vector3(boxCenter.x - this.boxSize / 2f, boxCenter.y + this.boxSize / 2f);
                        Vector3 tr = new Vector3(boxCenter.x + this.boxSize / 2f, boxCenter.y + this.boxSize / 2f);
                        Vector3 bl = new Vector3(boxCenter.x - this.boxSize / 2f, boxCenter.y - this.boxSize / 2f);
                        Vector3 br = new Vector3(boxCenter.x + this.boxSize / 2f, boxCenter.y - this.boxSize / 2f);
                        this.Vertices.Add(tl);
                        this.Vertices.Add(tr);
                        this.Vertices.Add(br);
                        this.Vertices.Add(bl);
                        this.Triangles.Add(this.Vertices.Count - 4);
                        this.Triangles.Add(this.Vertices.Count - 3);
                        this.Triangles.Add(this.Vertices.Count - 2);
                        this.Triangles.Add(this.Vertices.Count - 4);
                        this.Triangles.Add(this.Vertices.Count - 2);
                        this.Triangles.Add(this.Vertices.Count - 1);
                    }
                }
            }
        }

        protected override void LocalClear()
        {
            this.Rasterized = new Grid2D<int>(this.numRows, this.numCols, this.topLeft, new Vector2(this.boxSize, this.boxSize));
        }

        private void RasterizeCircles(List<CircleRoom> circles)
        {
            foreach (CircleRoom circle in circles)
            {
                Vector2 topLeft = new Vector2(circle.transform.position.x - circle.Radius, circle.transform.position.y + circle.Radius);
                Vector2 bottomRight = new Vector2(circle.transform.position.x + circle.Radius, circle.transform.position.y - circle.Radius);
                int r1 = this.Rasterized.GetRow(topLeft);
                int r2 = this.Rasterized.GetRow(bottomRight);
                int c1 = this.Rasterized.GetCol(topLeft);
                int c2 = this.Rasterized.GetCol(bottomRight);
                for (int r = r1; r < r2; r++)
                {
                    for (int c = c1; c < c2; c++)
                    {
                        Vector2 pos = this.Rasterized.GetPos(r, c);
                        if (Vector2.Distance(pos, circle.transform.position) <= circle.Radius)
                            this.Rasterized.Set(r,c, 1);
                    }
                }
            }
        }

        private void RasterizeLines(List<Path> lines)
        {
            foreach(Path e in lines)
            {
                float change = this.boxSize / Vector2.Distance(e.Start.transform.position, e.End.transform.position);
                float lerp = 0;
                while(lerp <= 1)
                {
                    Vector2 pos = Vector2.Lerp(e.Start.transform.position, e.End.transform.position, lerp);
                    this.Rasterized.Set(this.Rasterized.GetRow(pos), this.Rasterized.GetCol(pos), 1);
                    lerp += change;
                }
            }
        }
    }
}
