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
        private bool showGrid = false;
        [SerializeField]
        private Transform topLeft = null;
        [SerializeField]
        private int numRows = 0;
        [SerializeField]
        private int numCols = 0;
        [SerializeField]
        private float boxSize = 0;
        [SerializeField]
        private bool extrudeMesh = false;

        public MeshGrid Rasterized { get; private set; }

        private void OnDrawGizmos()
        {
            if (this.showGrid)
            {
                for (int r = 0; r < this.numRows; r++)
                {
                    for (int c = 0; c < this.numCols; c++)
                    {
                        Vector2 boxCenter = this.Rasterized.Squares[r, c].Center;
                        if (this.Rasterized != null && this.Rasterized.Squares[r, c].Filled)
                            Gizmos.DrawCube(boxCenter, new Vector2(this.boxSize, this.boxSize));
                        else
                            Gizmos.DrawWireCube(boxCenter, new Vector2(this.boxSize, this.boxSize));
                    }
                }
            }
        }


        protected override void LocalInit()
        {
            this.Rasterized = new MeshGrid(this.numRows, this.numCols, this.topLeft.position, new Vector2(this.boxSize, this.boxSize));
        }

        protected override void LocalCalculateMesh(RoomManager rooms, PathManager paths)
        {
            RasterizeCircles(rooms.CircleRooms);
            RasterizeLines(paths.GetPaths());
        }

        protected override void LocalCreateMesh()
        {
            // Floor
            for (int r = 0; r < this.numRows; r++)
            {
                for (int c = 0; c < this.numCols; c++)
                {
                    if (this.Rasterized.Squares[r, c].Filled)
                    {

                        Square square = this.Rasterized.Squares[r, c];
                        int tl = this.Rasterized.Corners[(int)square.TopLeft.x, (int)square.TopLeft.y].VertexIndex;
                        int tr = this.Rasterized.Corners[(int)square.TopRight.x, (int)square.TopRight.y].VertexIndex;
                        int bl = this.Rasterized.Corners[(int)square.BottomLeft.x, (int)square.BottomLeft.y].VertexIndex;
                        int br = this.Rasterized.Corners[(int)square.BottomRight.x, (int)square.BottomRight.y].VertexIndex;
                        AddTriangles(tl, tr, bl, br);
                    }
                }
            }

            if (this.extrudeMesh)
            {
                // Ceiling
                MeshGrid dupe = this.Rasterized.Duplicate(1, this.Vertices);
                for (int r = 0; r < this.numRows; r++)
                {
                    for (int c = 0; c < this.numCols; c++)
                    {
                        if (dupe.Squares[r, c].Filled)
                        {
                            Square square = dupe.Squares[r, c];
                            int tl = dupe.Corners[(int)square.TopLeft.x, (int)square.TopLeft.y].VertexIndex;
                            int tr = dupe.Corners[(int)square.TopRight.x, (int)square.TopRight.y].VertexIndex;
                            int bl = dupe.Corners[(int)square.BottomLeft.x, (int)square.BottomLeft.y].VertexIndex;
                            int br = dupe.Corners[(int)square.BottomRight.x, (int)square.BottomRight.y].VertexIndex;
                            AddTriangles(tl, tr, bl, br);
                        }
                    }
                }

                // Walls
                for (int r = 0; r < this.numRows; r++)
                {
                    for (int c = 0; c < this.numCols; c++)
                    {
                        if (this.Rasterized.Squares[r, c].Filled)
                        {
                            AddWalls(r, c, this.Rasterized, dupe);
                        }
                    }
                }
            }
        }

        protected override void LocalClear()
        {
            this.Rasterized = new MeshGrid(this.numRows, this.numCols, this.topLeft.position, new Vector2(this.boxSize, this.boxSize));
        }

        private void RasterizeCircles(List<CircleRoom> circles)
        {
            foreach (CircleRoom circle in circles)
            {
                Vector2 topLeft = new Vector2(circle.transform.position.x - circle.Radius, circle.transform.position.y + circle.Radius);
                Vector2 bottomRight = new Vector2(circle.transform.position.x + circle.Radius, circle.transform.position.y - circle.Radius);
                int r1 = GridUtil.GetRow(topLeft, this.topLeft.position, new Vector2(this.boxSize, this.boxSize), this.numRows);
                int r2 = GridUtil.GetRow(bottomRight, this.topLeft.position, new Vector2(this.boxSize, this.boxSize), this.numRows);
                int c1 = GridUtil.GetCol(topLeft, this.topLeft.position, new Vector2(this.boxSize, this.boxSize), this.numCols);
                int c2 = GridUtil.GetCol(bottomRight, this.topLeft.position, new Vector2(this.boxSize, this.boxSize), this.numCols);
                for (int r = r1; r < r2; r++)
                {
                    for (int c = c1; c < c2; c++)
                    {
                        Vector2 pos = GridUtil.GetPos(r, c, this.topLeft.position, new Vector2(this.boxSize, this.boxSize));
                        if (Vector2.Distance(pos, circle.transform.position) <= circle.Radius)
                            this.Rasterized.Fill(pos, this.Vertices);
                    }
                }
            }
        }

        private void RasterizeLines(List<Path> lines)
        {
            foreach (Path e in lines)
            {
                float change = this.boxSize / Vector2.Distance(e.Start.transform.position, e.End.transform.position);
                float lerp = 0;
                while (lerp <= 1)
                {
                    Vector2 pos = Vector2.Lerp(e.Start.transform.position, e.End.transform.position, lerp);
                    this.Rasterized.Fill(pos, this.Vertices);
                    lerp += change;
                }
            }
        }

        private void AddWalls(int r, int c, MeshGrid top, MeshGrid bottom)
        {
            bool left = r == 0 || !top.Squares[r - 1, c].Filled;
            bool right = r == this.numRows - 1 || !top.Squares[r + 1, c].Filled;
            bool up = c == 0 || !top.Squares[r, c - 1].Filled;
            bool down = c == this.numCols - 1 || !top.Squares[r, c + 1].Filled;

            if (left)
            {
                int tl = top.Corners[(int)top.Squares[r, c].TopLeft.x, (int)top.Squares[r, c].TopLeft.y].VertexIndex;
                int tr = bottom.Corners[(int)bottom.Squares[r, c].TopLeft.x, (int)bottom.Squares[r, c].TopLeft.y].VertexIndex;
                int bl = top.Corners[(int)top.Squares[r, c].TopRight.x, (int)top.Squares[r, c].TopRight.y].VertexIndex;
                int br = bottom.Corners[(int)bottom.Squares[r, c].TopRight.x, (int)bottom.Squares[r, c].TopRight.y].VertexIndex;
                AddTriangles(tl, tr, bl, br);
            }

            if (right)
            {
                int tl = bottom.Corners[(int)bottom.Squares[r, c].BottomLeft.x, (int)bottom.Squares[r, c].BottomLeft.y].VertexIndex;
                int tr = top.Corners[(int)top.Squares[r, c].BottomLeft.x, (int)top.Squares[r, c].BottomLeft.y].VertexIndex;
                int bl = bottom.Corners[(int)bottom.Squares[r, c].BottomRight.x, (int)bottom.Squares[r, c].BottomRight.y].VertexIndex;
                int br = top.Corners[(int)top.Squares[r, c].BottomRight.x, (int)top.Squares[r, c].BottomRight.y].VertexIndex;
                AddTriangles(tl, tr, bl, br);
            }

            if (up)
            {
                int tl = top.Corners[(int)top.Squares[r, c].TopLeft.x, (int)top.Squares[r, c].TopLeft.y].VertexIndex;
                int tr = top.Corners[(int)top.Squares[r, c].BottomLeft.x, (int)top.Squares[r, c].BottomLeft.y].VertexIndex;
                int bl = bottom.Corners[(int)bottom.Squares[r, c].TopLeft.x, (int)bottom.Squares[r, c].TopLeft.y].VertexIndex;
                int br = bottom.Corners[(int)bottom.Squares[r, c].BottomLeft.x, (int)bottom.Squares[r, c].BottomLeft.y].VertexIndex;
                AddTriangles(tl, tr, bl, br);
            }

            if (down)
            {
                int tl = top.Corners[(int)top.Squares[r, c].BottomRight.x, (int)top.Squares[r, c].BottomRight.y].VertexIndex;
                int tr = top.Corners[(int)top.Squares[r, c].TopRight.x, (int)top.Squares[r, c].TopRight.y].VertexIndex;
                int bl = bottom.Corners[(int)bottom.Squares[r, c].BottomRight.x, (int)bottom.Squares[r, c].BottomRight.y].VertexIndex;
                int br = bottom.Corners[(int)bottom.Squares[r, c].TopRight.x, (int)bottom.Squares[r, c].TopRight.y].VertexIndex;
                AddTriangles(tl, tr, bl, br);
            }
        }

        private void AddTriangles(int tl, int tr, int bl, int br)
        {
            this.Triangles.Add(tl);
            this.Triangles.Add(br);
            this.Triangles.Add(tr);
            this.Triangles.Add(tl);
            this.Triangles.Add(bl);
            this.Triangles.Add(br);
        }
    }
}
