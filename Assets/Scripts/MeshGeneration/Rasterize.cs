namespace GraphicsResearch.MeshGeneration
{
    using System.Collections;
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
                        Vector2 boxCenter = this.Grids.GetPos(r, c);
                        Gizmos.DrawWireCube(boxCenter, new Vector2(this.boxSize * this.subGridRows, this.boxSize * this.subGridCols));
                    }
                }

                for (int r = 0; r < this.subGridRows; r++)
                {
                    for (int c = 0; c < this.subGridCols; c++)
                    {
                        MeshGrid m = this.Grids.Get(drawR, drawC);
                        Square s = m.Squares[r, c];

                        if (this.Grids.Get(drawR, drawC) != null && this.Grids.Get(drawR, drawC).Squares[r, c].Filled)
                        {
                            Gizmos.DrawCube(s.Center, new Vector2(s.Size.x / 2f, s.Size.y / 2f));
                            Gizmos.DrawWireCube(s.Center, new Vector2(s.Size.x, s.Size.y));

                            //Gizmos.color = Color.red;
                            //if (s.Top.Filled)
                            //    Gizmos.DrawSphere(s.Top.Position, this.boxSize / 10);
                            //if (s.Bottom.Filled)
                            //    Gizmos.DrawSphere(s.Bottom.Position, this.boxSize / 10);
                            //if (s.Left.Filled)
                            //    Gizmos.DrawSphere(s.Left.Position, this.boxSize / 10);
                            //if (s.Right.Filled)
                            //    Gizmos.DrawSphere(s.Right.Position, this.boxSize / 10);

                            //Gizmos.color = Color.red;
                            //if (s.TopLeft.Filled)
                            //    Gizmos.DrawSphere(s.TopLeft.Position, this.boxSize / 10);

                            //Gizmos.color = Color.blue;
                            //if (s.TopRight.Filled)
                            //    Gizmos.DrawSphere(s.TopRight.Position, this.boxSize / 10);

                            //Gizmos.color = Color.green;
                            //if (s.BottomLeft.Filled)
                            //    Gizmos.DrawSphere(s.BottomLeft.Position, this.boxSize / 10);

                            //Gizmos.color = Color.yellow;
                            //if (s.BottomRight.Filled)
                            //    Gizmos.DrawSphere(s.BottomRight.Position, this.boxSize / 10);

                            Gizmos.color = Color.white;
                        }
                        else
                            Gizmos.DrawWireCube(s.Center, new Vector2(this.boxSize, this.boxSize));
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

        protected override IEnumerator LocalCalculateMeshAsync(RoomManager rooms, PathManager paths)
        {
            RasterizeCircles(rooms.CircleRooms);
            yield return null;
            RasterizeSquares(rooms.RectangleRooms);
            yield return null;
            RasterizePaths(paths.GetPaths());
            yield return null;
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

        protected override IEnumerator LocalCreateMeshAsync()
        {
            for (int r = 0; r < this.GridRows; r++)
            {
                for (int c = 0; c < this.GridCols; c++)
                {
                    ProcessRasterizedGrid(this.Grids.Get(r, c), r, c);
                    yield return null;
                }
            }

            yield return null;
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
                Vector3 start = rect.transform.position + rect.transform.up * rect.Dimentions.y / 2f;
                Vector3 end = rect.transform.position - rect.transform.up * rect.Dimentions.y / 2f;
                float width = rect.Dimentions.x;
                RasterizeBox(
                    start,
                    end,
                    width);

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
                            mesh.Squares[sr, sc].FillCircle(center, radius);
                        }
                    }
                }
            }
        }

        private void RasterizeBox(Vector3 start, Vector3 end, float width)
        {
            Vector3 es = Vector3.Normalize(start - end);
            Vector3 left = new Vector3(-es.y, es.x, es.z);
            float change = this.boxSize / Vector2.Distance(start, end) / 4f;
            float change2 = this.boxSize / width / 4;
            float lerp = 0;
            float lerp2 = 0;
            List<Square> squares = new List<Square>();
            while (lerp <= 1f + change)
            {
                Vector3 pos = Vector2.Lerp(start - left * width / 2f, end - left * width / 2f, lerp);
                lerp2 = 0;
                while (lerp2 <= 1f + change2)
                {
                    Vector3 cell = Vector2.Lerp(pos, pos + left * width, lerp2);
                    Square s = this.Grids.Get(this.Grids.GetRow(cell), this.Grids.GetCol(cell)).GetSquare(cell);
                    if (!squares.Contains(s))
                    {
                        s.FillBox(start, end, width);
                        squares.Add(s);
                    }

                    lerp2 += change2;
                }

                lerp += change;
            }

            squares.Clear();
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
                        List<int> triangles = rasterized.Squares[r, c].GetTriangles(this.Vertices[gridRow, gridCol], this.invertTriangles);
                        foreach (int i in triangles)
                            this.Triangles[gridRow, gridCol].Add(i);
                    }
                }
            }

            if (this.extrudeMesh)
            {
                // Ceiling triangles are inverted from floor
                MeshGrid dupe = rasterized.Duplicate(1);
                for (int r = 0; r < this.subGridRows; r++)
                {
                    for (int c = 0; c < this.subGridCols; c++)
                    {
                        if (dupe.Squares[r, c].Filled)
                        {
                            List<int> triangles = dupe.Squares[r, c].GetTriangles(this.Vertices[gridRow, gridCol], !this.invertTriangles);
                            foreach (int i in triangles)
                                this.Triangles[gridRow, gridCol].Add(i);
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

            Square ts = top.Squares[subGridRow, subGridCol];
            Square bs = bottom.Squares[subGridRow, subGridCol];
            Lib.Direction dir = Lib.Direction.None;

            if (left)
                dir = Lib.Direction.Left;
            else if (right)
                dir = Lib.Direction.Right;
            else if (up)
                dir = Lib.Direction.Up;
            else if (down)
                dir = Lib.Direction.Down;
            
            List<int> topPoints = ts.GetWallPoints(dir);
            List<int> bottomPoints = bs.GetWallPoints(dir);

            for(int i = 0; i < topPoints.Count - 1; i += 2)
            {
                Lib.AddSquare(this.Triangles[gridRow, gridCol], bottomPoints[i], bottomPoints[i + 1], topPoints[i + 1], topPoints[i], this.invertTriangles);
            }
        }
    }
}
