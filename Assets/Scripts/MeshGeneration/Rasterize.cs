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
                RasterizerUtil.DrawMesh(this.Grids, this.drawR, this.drawC);
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

        public override void ReserveGridSquares(Vector3 start, Vector3 end, float width)
        {
            Vector2 size = this.Grids.BoxSize;
            Vector2 gridSize = this.Grids.Get(0, 0).Squares[0, 0].Size;
            Vector3 es = Vector3.Normalize(start - end);
            Vector3 left = new Vector3(-es.y, es.x, es.z);
            float change = gridSize.x / Vector2.Distance(start, end) / 4f;
            float change2 = gridSize.x / width / 4f;
            float lerp = 0;
            float lerp2 = 0;
            while (lerp <= 1f + change)
            {
                Vector3 pos = Vector2.Lerp(start - left * width / 2f, end - left * width / 2f, lerp);
                lerp2 = 0;
                while (lerp2 <= 1f + change2)
                {
                    Vector3 cell = Vector2.Lerp(pos, pos + left * width, lerp2);
                    Square s = this.Grids.Get(this.Grids.GetRow(cell), this.Grids.GetCol(cell)).GetSquare(cell);
                    s.reserved = true;
                    lerp2 += change2;
                }

                lerp += change;
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
                }
                yield return null;
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
                RasterizerUtil.RasterizeCircle(this.Grids, circle.OriginalPosition, circle.Radius);
            }
        }

        private void RasterizeSquares(List<RectangleRoom> rectangles)
        {
            foreach (RectangleRoom rect in rectangles)
            {
                Vector3 start, end;
                float width;
                RasterizerUtil.GetSquareBounds(rect, out start, out end, out width);
                RasterizerUtil.RasterizeBox(this.Grids, start, end, width);

            }
        }

        private void RasterizePaths(List<Path> paths)
        {
            foreach (Path p in paths)
            {
                foreach (Edge e in p.Edges)
                {
                    RasterizerUtil.RasterizeBox(this.Grids, e.Start, e.End, e.Width);
                    if (e.EndRoom == null)
                        RasterizerUtil.RasterizeCircle(this.Grids, e.End, e.Width / 2f);
                }
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
                        List<int> triangles = rasterized.Squares[r, c].GetTriangles(this.Vertices[gridRow, gridCol], this.invertTriangles);
                        foreach (int i in triangles)
                            this.Triangles[gridRow, gridCol].Add(i);
                    }
                }
            }

            if (this.extrudeMesh)
            {
                // Ceiling triangles are inverted from floor
                MeshGrid dupe = rasterized.Duplicate(-1);
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
                            RasterizerUtil.AddWalls(
                                this.Grids, 
                                rasterized, 
                                dupe, 
                                this.Vertices,
                                this.Triangles,
                                this.invertTriangles,
                                gridRow, 
                                gridCol, 
                                r, 
                                c);
                        }
                    }
                }
            }
        }
    }
}
