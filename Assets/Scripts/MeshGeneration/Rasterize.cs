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
        private int subGridRows = 0;
        [SerializeField]
        private int subGridCols = 0;
        [SerializeField]
        private float boxSize = 0;
        [SerializeField]
        private bool extrudeMesh = false;
        [SerializeField]
        private bool invertTriangles = false;

        [SerializeField]
        private int drawR = 0;
        [SerializeField]
        private int drawC = 0;

        public Grid2D<MeshGrid> Grids { get; private set; }

        private void OnDrawGizmos()
        {
            if (this.showGrid && this.Grids != null)
            {
                for (int r = 0; r < this.GridRows; r++)
                {
                    for (int c = 0; c < this.GridCols; c++)
                    {
                        Vector2 boxCenter = this.Grids.GetPos(r,c);
                        Gizmos.DrawWireCube(boxCenter, new Vector2(this.boxSize * this.subGridRows, this.boxSize * this.subGridCols));
                    }
                }

                for (int r = 0; r < this.subGridRows; r++)
                {
                    for (int c = 0; c < this.subGridCols; c++)
                    {
                        Vector2 boxCenter = this.Grids.Get(drawR, drawC).Squares[r, c].Center;
                        if (this.Grids.Get(drawR, drawC) != null && this.Grids.Get(drawR, drawC).Squares[r, c].Filled)
                            Gizmos.DrawCube(boxCenter, new Vector2(this.boxSize, this.boxSize));
                        else
                            Gizmos.DrawWireCube(boxCenter, new Vector2(this.boxSize, this.boxSize));
                    }
                }
            }
        }

        protected override void LocalInit()
        {
            this.Grids = new Grid2D<MeshGrid>(this.GridRows, this.GridCols, this.topLeft, new Vector2(this.boxSize * this.subGridRows, this.boxSize * this.subGridCols));
            Vector2 gridSize = new Vector2(this.boxSize, this.boxSize);
            for (int r = 0; r < this.GridRows; r++)
            {
                for (int c = 0; c < this.GridCols; c++)
                {
                    Vector3 pos = this.Grids.GetPos(r, c);
                    pos -= new Vector3((this.boxSize * this.subGridRows) / 2f, -(this.boxSize * this.subGridCols) / 2f);
                    this.Grids.Set(r, c, new MeshGrid(
                        this.subGridRows,
                        this.subGridRows,
                        pos,
                        gridSize));
                }
            }
        }

        protected override void LocalCalculateMesh(RoomManager rooms, PathManager paths)
        {
            RasterizeCircles(rooms.CircleRooms);
            RasterizeSquares(rooms.RectangleRooms);
            RasterizePaths(paths.GetPaths());
        }

        protected override void LocalCreateMesh()
        {
            for (int r = 0; r < this.GridRows; r++)
            {
                for (int c = 0; c < this.GridCols; c++)
                {
                    ProcessRasterizedGrid(this.Grids.Get(r, c), r, c);
                }
            }
        }

        protected override void LocalClear()
        {
            this.Grids = new Grid2D<MeshGrid>(this.GridRows, this.GridCols, this.topLeft, new Vector2(this.boxSize * this.subGridRows, this.boxSize * this.subGridCols));
            Vector2 gridSize = new Vector2(this.boxSize, this.boxSize);
            for (int r = 0; r < this.GridRows; r++)
            {
                for (int c = 0; c < this.GridCols; c++)
                {
                    Vector3 pos = this.Grids.GetPos(r, c);
                    pos -= new Vector3((this.boxSize * this.subGridRows) / 2f, -(this.boxSize * this.subGridCols) / 2f);
                    this.Grids.Set(r, c, new MeshGrid(
                        this.subGridRows,
                        this.subGridRows,
                        pos,
                        gridSize));
                }
            }
        }

        private void RasterizeCircles(List<CircleRoom> circles)
        {
            foreach (CircleRoom circle in circles)
            {
                RasterizeCircle(circle.transform.position, circle.Radius);
            }
        }

        private void RasterizeSquares(List<RectangleRoom> rectangles)
        {
            foreach (RectangleRoom rect in rectangles)
            {
                RasterizeBox(
                    rect.transform.position - rect.transform.up * rect.Dimentions.y / 2f,
                    rect.transform.position + rect.transform.up * rect.Dimentions.y / 2f,
                    rect.Dimentions.x / 2f);

            }
        }

        private void RasterizePaths(List<Path> paths)
        {
            foreach (Path p in paths)
            {
                foreach (Edge e in p.Edges)
                {
                    RasterizeBox(e.Start, e.End, e.Width);
                    if (e.EndRoom == null)
                        RasterizeCircle(e.End, e.Width / 2f);
                }
            }
        }

        private void RasterizeCircle(Vector3 center, float radius)
        {
            Vector2 topLeft = new Vector2(center.x - radius, center.y + radius);
            Vector2 bottomRight = new Vector2(center.x + radius, center.y - radius);
            Vector2 size = new Vector2(this.boxSize * this.subGridRows, this.boxSize * this.subGridCols);
            Vector2 gridSize = new Vector2(this.boxSize, this.boxSize);
            int gridRowLeft = GridUtil.GetRow(topLeft, this.topLeft.position, size, this.GridRows);
            int gridRowRight = GridUtil.GetRow(bottomRight, this.topLeft.position, size, this.GridRows);
            int gridColTop = GridUtil.GetCol(topLeft, this.topLeft.position, size, this.GridCols);
            int gridColBottom = GridUtil.GetCol(bottomRight, this.topLeft.position, size, this.GridCols);
            for (int r = gridRowLeft; r <= gridRowRight; r++)
            {
                for (int c = gridColTop; c <= gridColBottom; c++)
                {
                    MeshGrid mesh = this.Grids.Get(r, c);
                    int r1 = GridUtil.GetRow(topLeft, mesh.TopLeft, gridSize, this.subGridRows);
                    int r2 = GridUtil.GetRow(bottomRight, mesh.TopLeft, gridSize, this.subGridRows);
                    int c1 = GridUtil.GetCol(topLeft, mesh.TopLeft, gridSize, this.subGridCols);
                    int c2 = GridUtil.GetCol(bottomRight, mesh.TopLeft, gridSize, this.subGridCols);
                    for (int sr = r1; sr <= r2; sr++)
                    {
                        for (int sc = c1; sc <= c2; sc++)
                        {
                            Vector2 pos = mesh.Squares[sr, sc].Center;
                            if (Vector2.Distance(pos, center) <= radius)
                            {
                                mesh.Fill(pos, this.Vertices[r, c]);
                            }
                        }
                    }
                }
            }
        }

        private void RasterizeBox(Vector3 start, Vector3 end, float width)
        {
            Vector3 es = Vector3.Normalize(start - end);
            Vector3 left = new Vector3(-es.y, es.x, es.z);
            float change = this.boxSize / Vector2.Distance(start, end) / 2f;
            float change2 = this.boxSize / width / 2f;
            float lerp = 0;
            float lerp2 = 0;
            while (lerp <= 1)
            {
                Vector3 pos = Vector2.Lerp(start - left * width / 2f, end - left * width / 2f, lerp);
                lerp2 = 0;
                while (lerp2 <= 1)
                {
                    Vector3 cell = Vector2.Lerp(pos, pos + left * width, lerp2);
                    int row = this.Grids.GetRow(cell);
                    int col = this.Grids.GetCol(cell);
                    this.Grids.Get(row, col).Fill(cell, this.Vertices[row, col]);
                    lerp2 += change2;
                }

                lerp += change;
            }
        }

        private void ProcessRasterizedGrid(MeshGrid rasterized, int gridRow, int gridCol)
        {
            // Floor
            for (int r = 0; r < this.subGridRows; r++)
            {
                for (int c = 0; c < this.subGridCols; c++)
                {
                    if (rasterized.Squares[r, c].Filled)
                    {

                        Square square = rasterized.Squares[r, c];
                        int tl = rasterized.Corners[(int)square.TopLeft.x, (int)square.TopLeft.y].VertexIndex;
                        int tr = rasterized.Corners[(int)square.TopRight.x, (int)square.TopRight.y].VertexIndex;
                        int bl = rasterized.Corners[(int)square.BottomLeft.x, (int)square.BottomLeft.y].VertexIndex;
                        int br = rasterized.Corners[(int)square.BottomRight.x, (int)square.BottomRight.y].VertexIndex;
                        AddTriangles(this.Triangles[gridRow, gridCol], tl, tr, bl, br, this.invertTriangles);
                    }
                }
            }


            if (this.extrudeMesh)
            {
                // Ceiling triangles are inverted from floor
                MeshGrid dupe = rasterized.Duplicate(1, this.Vertices[gridRow, gridCol]);
                for (int r = 0; r < this.subGridRows; r++)
                {
                    for (int c = 0; c < this.subGridCols; c++)
                    {
                        if (dupe.Squares[r, c].Filled)
                        {
                            Square square = dupe.Squares[r, c];
                            int tl = dupe.Corners[(int)square.TopLeft.x, (int)square.TopLeft.y].VertexIndex;
                            int tr = dupe.Corners[(int)square.TopRight.x, (int)square.TopRight.y].VertexIndex;
                            int bl = dupe.Corners[(int)square.BottomLeft.x, (int)square.BottomLeft.y].VertexIndex;
                            int br = dupe.Corners[(int)square.BottomRight.x, (int)square.BottomRight.y].VertexIndex;
                            AddTriangles(this.Triangles[gridRow, gridCol], tl, tr, bl, br, !this.invertTriangles);
                        }
                    }
                }

                // Walls
                for (int r = 0; r < this.subGridRows; r++)
                {
                    for (int c = 0; c < this.subGridCols; c++)
                    {
                        if (rasterized.Squares[r, c].Filled)
                        {
                            AddWalls(gridRow, gridCol, r, c, rasterized, dupe);
                        }
                    }
                }
            }
        }

        private void AddWalls(int gridRow, int gridCol, int subGridRow, int subGridCol, MeshGrid top, MeshGrid bottom)
        {
            bool left = subGridRow == 0 || !top.Squares[subGridRow - 1, subGridCol].Filled;
            bool right = subGridRow == this.subGridRows - 1 || !top.Squares[subGridRow + 1, subGridCol].Filled;
            bool up = subGridCol == 0 || !top.Squares[subGridRow, subGridCol - 1].Filled;
            bool down = subGridCol == this.subGridCols - 1 || !top.Squares[subGridRow, subGridCol + 1].Filled;

            if (subGridRow == 0 && gridRow != 0)
                left = !this.Grids.Get(gridRow - 1, gridCol).Squares[this.subGridRows - 1, subGridCol].Filled;

            if (subGridRow == this.subGridRows - 1 && gridRow != this.GridRows - 1)
                right = !this.Grids.Get(gridRow + 1, gridCol).Squares[0, subGridCol].Filled;

            if (subGridCol == 0 && gridCol != 0)
                up = !this.Grids.Get(gridRow, gridCol - 1).Squares[subGridRow, this.subGridCols - 1].Filled;

            if (subGridCol == this.subGridCols - 1 && gridCol != this.GridCols - 1)
                down = !this.Grids.Get(gridRow, gridCol + 1).Squares[subGridRow, 0].Filled;

            if (left)
            {
                int tl = top.Corners[(int)top.Squares[subGridRow, subGridCol].TopLeft.x, (int)top.Squares[subGridRow, subGridCol].TopLeft.y].VertexIndex;
                int tr = bottom.Corners[(int)bottom.Squares[subGridRow, subGridCol].TopLeft.x, (int)bottom.Squares[subGridRow, subGridCol].TopLeft.y].VertexIndex;
                int bl = top.Corners[(int)top.Squares[subGridRow, subGridCol].TopRight.x, (int)top.Squares[subGridRow, subGridCol].TopRight.y].VertexIndex;
                int br = bottom.Corners[(int)bottom.Squares[subGridRow, subGridCol].TopRight.x, (int)bottom.Squares[subGridRow, subGridCol].TopRight.y].VertexIndex;
                AddTriangles(this.Triangles[gridRow, gridCol], tl, tr, bl, br, this.invertTriangles);
            }

            if (right)
            {
                int tl = bottom.Corners[(int)bottom.Squares[subGridRow, subGridCol].BottomLeft.x, (int)bottom.Squares[subGridRow, subGridCol].BottomLeft.y].VertexIndex;
                int tr = top.Corners[(int)top.Squares[subGridRow, subGridCol].BottomLeft.x, (int)top.Squares[subGridRow, subGridCol].BottomLeft.y].VertexIndex;
                int bl = bottom.Corners[(int)bottom.Squares[subGridRow, subGridCol].BottomRight.x, (int)bottom.Squares[subGridRow, subGridCol].BottomRight.y].VertexIndex;
                int br = top.Corners[(int)top.Squares[subGridRow, subGridCol].BottomRight.x, (int)top.Squares[subGridRow, subGridCol].BottomRight.y].VertexIndex;
                AddTriangles(this.Triangles[gridRow, gridCol], tl, tr, bl, br, this.invertTriangles);
            }

            if (up)
            {
                int tl = top.Corners[(int)top.Squares[subGridRow, subGridCol].TopLeft.x, (int)top.Squares[subGridRow, subGridCol].TopLeft.y].VertexIndex;
                int tr = top.Corners[(int)top.Squares[subGridRow, subGridCol].BottomLeft.x, (int)top.Squares[subGridRow, subGridCol].BottomLeft.y].VertexIndex;
                int bl = bottom.Corners[(int)bottom.Squares[subGridRow, subGridCol].TopLeft.x, (int)bottom.Squares[subGridRow, subGridCol].TopLeft.y].VertexIndex;
                int br = bottom.Corners[(int)bottom.Squares[subGridRow, subGridCol].BottomLeft.x, (int)bottom.Squares[subGridRow, subGridCol].BottomLeft.y].VertexIndex;
                AddTriangles(this.Triangles[gridRow, gridCol], tl, tr, bl, br, this.invertTriangles);
            }

            if (down)
            {
                int tl = top.Corners[(int)top.Squares[subGridRow, subGridCol].BottomRight.x, (int)top.Squares[subGridRow, subGridCol].BottomRight.y].VertexIndex;
                int tr = top.Corners[(int)top.Squares[subGridRow, subGridCol].TopRight.x, (int)top.Squares[subGridRow, subGridCol].TopRight.y].VertexIndex;
                int bl = bottom.Corners[(int)bottom.Squares[subGridRow, subGridCol].BottomRight.x, (int)bottom.Squares[subGridRow, subGridCol].BottomRight.y].VertexIndex;
                int br = bottom.Corners[(int)bottom.Squares[subGridRow, subGridCol].TopRight.x, (int)bottom.Squares[subGridRow, subGridCol].TopRight.y].VertexIndex;
                AddTriangles(this.Triangles[gridRow, gridCol], tl, tr, bl, br, this.invertTriangles);
            }
        }

        private void AddTriangles(List<int> triangles, int tl, int tr, int bl, int br, bool invert)
        {
            if (invert)
            {
                triangles.Add(br);
                triangles.Add(bl);
                triangles.Add(tl);
                triangles.Add(tr);
                triangles.Add(br);
                triangles.Add(tl);
            }
            else
            {
                triangles.Add(tl);
                triangles.Add(br);
                triangles.Add(tr);
                triangles.Add(tl);
                triangles.Add(bl);
                triangles.Add(br);
            }
        }
    }
}
