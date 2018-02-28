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

        protected override void LocalReserveGridSquares(RoomManager rooms, PathManager paths)
        {
            //CircleRoom circleRoom = rooms.CircleRooms[0];
            //Vector3 circle = circleRoom.OriginalPosition + Vector3.up * (circleRoom.Radius);
            //Debug.Log("loc: " + circle);
            //Vector2 size = new Vector2(this.boxSize * this.subGridRows, this.boxSize * this.subGridCols);
            //Vector2 gridSize = new Vector2(this.boxSize, this.boxSize);
            //int row = GridUtil.GetRow(circle, this.topLeft.position, size, this.GridRows);
            //int col = GridUtil.GetCol(circle, this.topLeft.position, size, this.GridCols);
            //MeshGrid mesh = this.Grids.Get(row, col);
            //Debug.Log("row: " + row + ", col: " + col);
            //mesh.GetSquare(circle).reserved = true;
            //Debug.Log("Square: " + mesh.GetSquare(circle).Index);

            //RectangleRoom rectangle = rooms.RectangleRooms[0];
            //Vector3 rect = rectangle.OriginalPosition + rectangle.transform.up * (rectangle.transform.localScale.y / 2f);
            //Debug.Log("loc: " + rect);
            //size = new Vector2(this.boxSize * this.subGridRows, this.boxSize * this.subGridCols);
            //gridSize = new Vector2(this.boxSize, this.boxSize);
            //row = GridUtil.GetRow(rect, this.topLeft.position, size, this.GridRows);
            //col = GridUtil.GetCol(rect, this.topLeft.position, size, this.GridCols);
            //mesh = this.Grids.Get(row, col);
            //Debug.Log("row: " + row + ", col: " + col);
            //mesh.GetSquare(rect).reserved = true;
            //Debug.Log("Square: " + mesh.GetSquare(rect).Index);
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
