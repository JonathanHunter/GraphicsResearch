namespace GraphicsResearch.MultiFloorGeneration
{
    using System.Collections.Generic;
    using UnityEngine;
    using MeshGeneration;
    using PathPlacement;
    using RoomPlacement;
    using Util;
    using System;

    public class HallwayRasterizer
    {
        /// <summary> The vertices of the generated meshes. </summary>
        public List<Vector3>[,] Vertices { get; protected set; }
        /// <summary> The triangles of the generated meshes. </summary>
        public List<int>[,] Triangles { get; protected set; }
        /// <summary> The grids holding the rasterized info. </summary>
        public Grid2D<MeshGrid> Grids { get; private set; }
        
        /// <summary> Whether or not to invert the triangles. </summary>
        private bool invertTriangles;

        /// <summary> Initializes a new rasterizer. </summary>
        /// <param name="gridDim"> The dimensions of the top level grid. </param>
        /// <param name="subGridDim"> The dimensions of the individual sub grids. </param>
        /// <param name="boxSize"> The size of a grid square. </param>
        /// <param name="topLeft"> The 3d loction to spawn the top left of the grid. </param>
        /// <param name="invertTriangles"> Whether or not to invert the triangles during generation. </param>
        public HallwayRasterizer(Vector2Int gridDim, Vector2Int subGridDim, Vector2 boxSize, Transform topLeft, bool invertTriangles)
        {
            this.Vertices = new List<Vector3>[gridDim.x, gridDim.y];
            this.Triangles = new List<int>[gridDim.x, gridDim.y];
            this.Grids = new Grid2D<MeshGrid>(gridDim.x, gridDim.y, topLeft, new Vector2(boxSize.x * subGridDim.x, boxSize.y * subGridDim.y));
            Vector2 gridSize = boxSize;
            for (int r = 0; r < gridDim.x; r++)
            {
                for (int c = 0; c < gridDim.y; c++)
                {
                    this.Vertices[r, c] = new List<Vector3>();
                    this.Triangles[r, c] = new List<int>();

                    Vector3 pos = this.Grids.GetPos(r, c);
                    pos -= new Vector3((boxSize.x * subGridDim.x) / 2f, -(boxSize.y * subGridDim.y) / 2f);
                    this.Grids.Set(r, c, new MeshGrid(subGridDim.x, subGridDim.y, pos, gridSize));
                }
            }

            this.invertTriangles = invertTriangles;
        }

        /// <summary> Draws the grid using gizmos. </summary>
        /// <param name="selectedGridSquare"> The specific sub-grid to draw. </param>
        public void DrawGrid(Vector2Int selectedGridSquare)
        {
            RasterizerUtil.DrawMesh(this.Grids, selectedGridSquare.x, selectedGridSquare.y);
        }

        /// <summary> Rasterize a circle room to the grid. </summary>
        /// <param name="circle"> The room to rasterize. </param>
        public void RasterizeCircle(CircleRoom circle)
        {
            RasterizerUtil.RasterizeCircle(this.Grids, circle.OriginalPosition, circle.Radius);
        }

        /// <summary> Rasterize a rectangle room to the grid. </summary>
        /// <param name="rect"> The room to rasterize. </param>
        public void RasterizeRectangle(RectangleRoom rect)
        {
            Vector3 start, end;
            float width;
            RasterizerUtil.GetSquareBounds(rect, out start, out end, out width);
            RasterizerUtil.RasterizeBox(this.Grids, start, end, width);
        }

        /// <summary> Rasterize a path to the grid. </summary>
        /// <param name="path"> The path to rasterize. </param>
        public void RasterizePath(Path path)
        {
            foreach (Edge e in path.Edges)
            {
                RasterizerUtil.RasterizeBox(this.Grids, e.Start, e.End, e.Width);
                if (e.EndRoom == null)
                    RasterizerUtil.RasterizeCircle(this.Grids, e.End, e.Width / 2f);
            }
        }

        /// <summary> Rasterize a path to the grid. </summary>
        /// <param name="path"> The path to rasterize. </param>
        public void RasterizePath(MulitFloorPath path)
        {
            RasterizerUtil.RasterizeBox(this.Grids, path.Room1.OriginalPosition, path.Room2.OriginalPosition, path.Width);
        }

        /// <summary> Marks squares for keeping. </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="width"></param>
        public void MarkForKeeping(Vector3 start, Vector3 end, float width)
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
                    s.Mark();
                    lerp2 += change2;
                }

                lerp += change;
            }
        }

        public void RaiseMesh(Vector3 start, Vector3 end, float width, Vector3 startHeight, Vector3 endHeight)
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
                    int r = this.Grids.GetRow(cell);
                    int c = this.Grids.GetCol(cell);
                    Square s = this.Grids.Get(r, c).GetSquare(cell);
                    s.SetZHeight(startHeight, endHeight, this.Vertices[r,c]);
                    lerp2 += change2;
                }

                lerp += change;
            }
        }

        /// <summary> Generates the mesh for each MeshGrid. </summary>
        public void GenerateMesh(bool onlyMarked)
        {
            for(int r = 0; r < this.Grids.Rows; r++)
            {
                for(int c = 0; c < this.Grids.Cols; c++)
                {
                    MeshGrid mesh = this.Grids.Get(r, c);
                    for (int sr = 0; sr < mesh.Rows; sr++)
                    {
                        for (int sc = 0; sc < mesh.Cols; sc++)
                        {
                            if (mesh.Squares[sr, sc].Filled && (!onlyMarked || mesh.Squares[sr,sc].Marked))
                            {
                                List<int> triangles = mesh.Squares[sr, sc].GetTriangles(this.Vertices[r, c], this.invertTriangles);
                                foreach (int i in triangles)
                                    this.Triangles[r, c].Add(i);
                            }
                        }
                    }
                }
            }
        }

        /// <summary> Extrudes the mesh for each MeshGrid. </summary>
        public void ExtrudeMesh(bool onlyMarked)
        {
            for (int r = 0; r < this.Grids.Rows; r++)
            {
                for (int c = 0; c < this.Grids.Cols; c++)
                {
                    MeshGrid dupe = this.Grids.Get(r,c).Duplicate(-1);
                    for (int sr = 0; sr < dupe.Rows; sr++)
                    {
                        for (int sc = 0; sc < dupe.Cols; sc++)
                        {
                            if (dupe.Squares[sr, sc].Filled && (!onlyMarked || dupe.Squares[sr, sc].Marked))
                            {
                                // Ceiling triangles are inverted from floor
                                List<int> triangles = dupe.Squares[sr, sc].GetTriangles(this.Vertices[r, c], !this.invertTriangles);
                                foreach (int i in triangles)
                                    this.Triangles[r, c].Add(i);

                                RasterizerUtil.AddWalls(
                                    this.Grids,
                                    this.Grids.Get(r, c),
                                    dupe,
                                    this.Vertices,
                                    this.Triangles,
                                    this.invertTriangles,
                                    r,
                                    c,
                                    sr,
                                    sc);
                            }
                        }
                    }
                }
            }
        }
    }
}
