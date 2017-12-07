﻿namespace GraphicsResearch.MeshGeneration
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
                        Vector2 boxCenter = s.Center;
                        if (this.Grids.Get(drawR, drawC) != null && this.Grids.Get(drawR, drawC).Squares[r, c].Filled)
                        {
                            //Vector2 tl = m.Corners[s.TopLeft.x, s.TopLeft.y].Position;
                            //Vector2 tr = m.Corners[s.TopRight.x, s.TopRight.y].Position;
                            //Vector2 bl = m.Corners[s.BottomLeft.x, s.BottomLeft.y].Position;
                            //Vector2 br = m.Corners[s.BottomRight.x, s.BottomRight.y].Position;
                            //Vector2 i = CircleLineIntersection(bl, br, new Vector3(-5.07f, 5.65f, 0), .5f);
                            //Debug.Log("bl: " + bl + ", br: " + br + ", intersection: " + i);
                            Gizmos.DrawCube(boxCenter, new Vector2(this.boxSize / 2f, this.boxSize / 2f));
                            Gizmos.DrawWireCube(boxCenter, new Vector2(this.boxSize, this.boxSize));
                            //Gizmos.color = Color.cyan;
                            //Gizmos.DrawSphere(m.Corners[s.BottomRight.x, s.BottomRight.y].Position, .025f);
                            Gizmos.color = Color.red;
                            if (s.Top.Position != Vector3.zero)
                                Gizmos.DrawSphere(s.Top.Position, this.boxSize / 10);
                            Gizmos.color = Color.blue;
                            if (s.Bottom.Position != Vector3.zero)
                                Gizmos.DrawSphere(s.Bottom.Position, this.boxSize / 10);
                            Gizmos.color = Color.green;
                            if (s.Left.Position != Vector3.zero)
                                Gizmos.DrawSphere(s.Left.Position, this.boxSize / 10);
                            Gizmos.color = Color.yellow;
                            if (s.Right.Position != Vector3.zero)
                                Gizmos.DrawSphere(s.Right.Position, this.boxSize / 10);
                            Gizmos.color = Color.white;
                        }
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
                            Square s = mesh.Squares[sr, sc];
                            Vector2 pos = s.Center;
                            Vector2[] intersections = new Vector2[5];
                            Vector2 tl = mesh.Corners[s.TopLeft.x, s.TopLeft.y].Position;
                            Vector2 tr = mesh.Corners[s.TopRight.x, s.TopRight.y].Position;
                            Vector2 bl = mesh.Corners[s.BottomLeft.x, s.BottomLeft.y].Position;
                            Vector2 br = mesh.Corners[s.BottomRight.x, s.BottomRight.y].Position;
                            intersections[0] = CircleLineIntersection(tl, tr, center, radius);
                            intersections[1] = CircleLineIntersection(bl, br, center, radius);
                            intersections[2] = CircleLineIntersection(tl, bl, center, radius);
                            intersections[3] = CircleLineIntersection(tr, br, center, radius);
                            intersections[4] = center;
                            int intersectionCount = 0;
                            if (intersections[0] != Vector2.zero)
                                intersectionCount++;
                            if (intersections[1] != Vector2.zero)
                                intersectionCount++;
                            if (intersections[2] != Vector2.zero)
                                intersectionCount++;
                            if (intersections[3] != Vector2.zero)
                                intersectionCount++;

                            if (Vector2.Distance(pos, center) <= radius ||
                                 intersectionCount > 1)
                            {
                                mesh.Fill(pos);
                                ProcessSquare(mesh, sr, sc, intersections);
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
            List<Square> squares = new List<Square>();
            while (lerp <= 1f + change)
            {
                Vector3 pos = Vector2.Lerp(start - left * width / 2f, end - left * width / 2f, lerp);
                lerp2 = 0;
                while (lerp2 <= 1f + change2)
                {
                    Vector3 cell = Vector2.Lerp(pos, pos + left * width, lerp2);
                    int row = this.Grids.GetRow(cell);
                    int col = this.Grids.GetCol(cell);
                    MeshGrid mesh = this.Grids.Get(row, col);
                    Square s = mesh.GetSquare(cell);
                    if (!squares.Contains(s))
                    {
                        mesh.Fill(cell);
                        Vector2[] intersections = new Vector2[5];
                        Vector2 tl = mesh.Corners[s.TopLeft.x, s.TopLeft.y].Position;
                        Vector2 tr = mesh.Corners[s.TopRight.x, s.TopRight.y].Position;
                        Vector2 bl = mesh.Corners[s.BottomLeft.x, s.BottomLeft.y].Position;
                        Vector2 br = mesh.Corners[s.BottomRight.x, s.BottomRight.y].Position;
                        intersections[0] = BoxLineIntersection(tl, tr, start, end, width, s.Center);
                        intersections[1] = BoxLineIntersection(bl, br, start, end, width, s.Center);
                        intersections[2] = BoxLineIntersection(tl, bl, start, end, width, s.Center);
                        intersections[3] = BoxLineIntersection(tr, br, start, end, width, s.Center);
                        intersections[4] = lerp2 < .5f ? cell + left * change2 : cell - left * change2;//start - (start - end) / 2;
                        int intersectionCount = 0;
                        if (intersections[0] != Vector2.zero)
                            intersectionCount++;
                        if (intersections[1] != Vector2.zero)
                            intersectionCount++;
                        if (intersections[2] != Vector2.zero)
                            intersectionCount++;
                        if (intersections[3] != Vector2.zero)
                            intersectionCount++;
                        if (intersectionCount > 0)
                        {
                            ProcessSquare(mesh, s.Index.x, s.Index.y, intersections);
                            squares.Add(s);
                        }
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
                        CalcualteTriangles(rasterized, r, c, gridRow, gridCol, this.invertTriangles);
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
                            CalcualteTriangles(dupe, r, c, gridRow, gridCol, !this.invertTriangles);
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
            int ttl = top.Corners[ts.TopLeft.x, ts.TopLeft.y].VertexIndex;
            int ttr = top.Corners[ts.TopRight.x, ts.TopRight.y].VertexIndex;
            int tbl = top.Corners[ts.BottomLeft.x, ts.BottomLeft.y].VertexIndex;
            int tbr = top.Corners[ts.BottomRight.x, ts.BottomRight.y].VertexIndex;
            int btl = bottom.Corners[bs.TopLeft.x, bs.TopLeft.y].VertexIndex;
            int btr = bottom.Corners[bs.TopRight.x, bs.TopRight.y].VertexIndex;
            int bbl = bottom.Corners[bs.BottomLeft.x, bs.BottomLeft.y].VertexIndex;
            int bbr = bottom.Corners[bs.BottomRight.x, bs.BottomRight.y].VertexIndex;

            if (ts.CurrentState == Square.State.tbl)
            {
                ttr = ts.Top.VertexIndex;
                tbr = ts.Bottom.VertexIndex;
                btr = bs.Top.VertexIndex;
                bbr = bs.Bottom.VertexIndex;
            }
            else if (ts.CurrentState == Square.State.tbr)
            {
                ttl = ts.Top.VertexIndex;
                tbl = ts.Bottom.VertexIndex;
                btl = bs.Top.VertexIndex;
                bbl = bs.Bottom.VertexIndex;
            }
            else if (ts.CurrentState == Square.State.tlu)
            {
                ttr = ts.Top.VertexIndex;
                tbl = ts.Left.VertexIndex;
                tbr = ts.Left.VertexIndex;
                btr = bs.Top.VertexIndex;
                bbl = bs.Left.VertexIndex;
                bbr = bs.Left.VertexIndex;
            }
            else if (ts.CurrentState == Square.State.tld)
            {
                ttl = ts.Top.VertexIndex;
                tbl = ts.Left.VertexIndex;
                btl = bs.Top.VertexIndex;
                bbl = bs.Left.VertexIndex;
            }
            else if (ts.CurrentState == Square.State.tru)
            {
                ttl = ts.Top.VertexIndex;
                tbl = ts.Right.VertexIndex;
                tbr = ts.Right.VertexIndex;
                btl = bs.Top.VertexIndex;
                bbl = bs.Right.VertexIndex;
                bbr = bs.Right.VertexIndex;
            }
            else if (ts.CurrentState == Square.State.trd)
            {
                ttr = ts.Top.VertexIndex;
                tbr = ts.Right.VertexIndex;
                btr = bs.Top.VertexIndex;
                bbr = bs.Right.VertexIndex;
            }
            else if (ts.CurrentState == Square.State.blu)
            {
                ttl = ts.Left.VertexIndex;
                tbl = ts.Bottom.VertexIndex;
                btl = bs.Left.VertexIndex;
                bbl = bs.Bottom.VertexIndex;
            }
            else if (ts.CurrentState == Square.State.bld)
            {
                ttl = ts.Left.VertexIndex;
                ttr = ts.Left.VertexIndex;
                tbr = ts.Bottom.VertexIndex;
                btl = bs.Left.VertexIndex;
                btr = bs.Left.VertexIndex;
                bbr = bs.Bottom.VertexIndex;
            }
            else if (ts.CurrentState == Square.State.bru)
            {
                ttr = ts.Right.VertexIndex;
                tbr = ts.Bottom.VertexIndex;
                btr = ts.Right.VertexIndex;
                bbr = ts.Bottom.VertexIndex;
            }
            else if (ts.CurrentState == Square.State.brd)
            {
                ttl = ts.Right.VertexIndex;
                ttr = ts.Right.VertexIndex;
                tbl = ts.Bottom.VertexIndex;
                btl = bs.Right.VertexIndex;
                btr = bs.Right.VertexIndex;
                bbl = bs.Bottom.VertexIndex;
            }
            else if (ts.CurrentState == Square.State.lru)
            {
                tbl = ts.Left.VertexIndex;
                tbr = ts.Right.VertexIndex;
                bbl = bs.Left.VertexIndex;
                bbr = bs.Right.VertexIndex;
            }
            else if (ts.CurrentState == Square.State.lrd)
            {
                ttl = ts.Left.VertexIndex;
                ttr = ts.Right.VertexIndex;
                btl = bs.Left.VertexIndex;
                btr = bs.Right.VertexIndex;
            }
            
            if (left)
                AddSquare(this.Triangles[gridRow, gridCol], bbl, btl, ttl, tbl, this.invertTriangles);

            if (right)
                AddSquare(this.Triangles[gridRow, gridCol], btr, bbr, tbr, ttr, this.invertTriangles);

            if (up)
                AddSquare(this.Triangles[gridRow, gridCol], btl, btr, ttr, ttl, this.invertTriangles);

            if (down)
                AddSquare(this.Triangles[gridRow, gridCol], bbr, bbl, tbl, tbr, this.invertTriangles);
        }

        private void AddTriangle(List<int> triangles, int a, int b, int c, bool invert)
        {
            if (invert)
            {
                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(c);
            }
            else
            {
                triangles.Add(c);
                triangles.Add(b);
                triangles.Add(a);
            }
        }

        private void AddSquare(List<int> triangles, int a, int b, int c, int d, bool invert)
        {
            AddTriangle(triangles, a, b, c, invert);
            AddTriangle(triangles, d, a, c, invert);
        }

        private void AddPentagon(List<int> triangles, int a, int b, int c, int d, int e, bool invert)
        {
            AddTriangle(triangles, e, a, b, invert);
            AddTriangle(triangles, e, b, c, invert);
            AddTriangle(triangles, e, c, d, invert);
        }

        private Vector2 CircleLineIntersection(Vector3 begin, Vector3 end, Vector3 center, float radius)
        {
            float x1 = begin.x - center.x;
            float y1 = begin.y - center.y;
            float x2 = end.x - center.x;
            float y2 = end.y - center.y;
            float dx = x2 - x1;
            float dy = y2 - y1;
            float dr = Mathf.Sqrt(dx * dx + dy * dy);
            float D = x1 * y2 - x2 * y1;
            float incedent = radius * radius * dr * dr - D * D;
            if (incedent < 0)
                return Vector2.zero;

            float rx1 = (D * dy + Mathf.Sign(dy) * dx * Mathf.Sqrt(incedent)) / (dr * dr) + center.x;
            float ry1 = (-D * dx + Mathf.Abs(dy) * Mathf.Sqrt(incedent)) / (dr * dr) + center.y;
            float rx2 = (D * dy - Mathf.Sign(dy) * dx * Mathf.Sqrt(incedent)) / (dr * dr) + center.x;
            float ry2 = (-D * dx - Mathf.Abs(dy) * Mathf.Sqrt(incedent)) / (dr * dr) + center.y;

            if ((rx1 >= begin.x && rx1 <= end.x && ry1 >= begin.y && ry1 <= end.y) ||
                (rx1 >= end.x && rx1 <= begin.x && ry1 >= end.y && ry1 <= begin.y))
                return new Vector2(rx1, ry1);

            if ((rx2 >= begin.x && rx2 <= end.x && ry2 >= begin.y && ry2 <= end.y) ||
                (rx2 >= end.x && rx2 <= begin.x && ry2 >= end.y && ry2 <= begin.y))
                return new Vector2(rx2, ry2);

            if ((rx1 >= begin.x - this.boxSize / 10f && rx1 <= end.x + this.boxSize / 10f && ry1 >= begin.y - this.boxSize / 10f && ry1 <= end.y + this.boxSize / 10f) ||
                (rx1 >= end.x - this.boxSize / 10f && rx1 <= begin.x + this.boxSize / 10f && ry1 >= end.y - this.boxSize / 10f && ry1 <= begin.y + this.boxSize / 10f))
                return new Vector2(rx1, ry1);

            if ((rx2 >= begin.x - this.boxSize / 10f && rx2 <= end.x + this.boxSize / 10f && ry2 >= begin.y - this.boxSize / 10f && ry2 <= end.y + this.boxSize / 10f) ||
                (rx2 >= end.x - this.boxSize / 10f && rx2 <= begin.x + this.boxSize / 10f && ry2 >= end.y - this.boxSize / 10f && ry2 <= begin.y + this.boxSize / 10f))
                return new Vector2(rx2, ry2);

            return Vector2.zero;
        }

        private Vector2 BoxLineIntersection(Vector3 p1, Vector3 p2, Vector3 start, Vector3 end, float width, Vector3 center)
        {
            Vector3 es = Vector3.Normalize(start - end);
            Vector3 left = new Vector3(-es.y, es.x, es.z);
            Vector3 tl = start + left * width / 2f;
            Vector3 tr = start - left * width / 2f;
            Vector3 bl = end + left * width / 2f;
            Vector3 br = end - left * width / 2f;
            
            Vector2 t = LineLineIntersection(p1, p2, tl, tr);
            if (t != Vector2.zero)
                return t;

            Vector2 b = LineLineIntersection(p1, p2, bl, br);
            if (b != Vector2.zero)
                return b;

            Vector2 l = LineLineIntersection(p1, p2, tl, bl);
            if (l != Vector2.zero)
                return l;

            Vector2 r = LineLineIntersection(p1, p2, tr, br);
            if (r != Vector2.zero)
                return r;

            return Vector2.zero;
        }

        private Vector2 LineLineIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            float x = ((p1.x * p2.y - p1.y * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.y - p3.y * p4.x)) / ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));
            float y = ((p1.x * p2.y - p1.y * p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x * p4.y - p3.y * p4.x)) / ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));

            if (((x >= p1.x && x <= p2.x && y >= p1.y && y <= p2.y) ||
                (x >= p2.x && x <= p1.x && y >= p2.y && y <= p1.y)) &&
                ((x >= p3.x && x <= p4.x && y >= p3.y && y <= p4.y) ||
                (x >= p4.x && x <= p3.x && y >= p4.y && y <= p3.y)))
                return new Vector2(x, y);

            if (((x >= p1.x - this.boxSize / 10f && x <= p2.x + this.boxSize / 10f && y >= p1.y - this.boxSize / 10f && y <= p2.y + this.boxSize / 10f) ||
                (x >= p2.x - this.boxSize / 10f && x <= p1.x + this.boxSize / 10f && y >= p2.y - this.boxSize / 10f && y <= p1.y + this.boxSize / 10f)) &&
                ((x >= p3.x - this.boxSize / 10f && x <= p4.x + this.boxSize / 10f && y >= p3.y - this.boxSize / 10f && y <= p4.y + this.boxSize / 10f) ||
                (x >= p4.x - this.boxSize / 10f && x <= p3.x + this.boxSize / 10f && y >= p4.y - this.boxSize / 10f && y <= p3.y + this.boxSize / 10f)))
                return new Vector2(x, y);

            return Vector2.zero;
        }

        private void CalcualteTriangles(MeshGrid mesh, int row, int col, int gridRow, int gridCol, bool inverted)
        {
            Square square = mesh.Squares[row, col];
            if (square.CurrentState == Square.State.NoIntersection)
            {
                int tl = mesh.Corners[(int)square.TopLeft.x, (int)square.TopLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int tr = mesh.Corners[(int)square.TopRight.x, (int)square.TopRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int bl = mesh.Corners[(int)square.BottomLeft.x, (int)square.BottomLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int br = mesh.Corners[(int)square.BottomRight.x, (int)square.BottomRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                AddSquare(this.Triangles[gridRow, gridCol], br, bl, tl, tr, inverted);
            }
            else if (square.CurrentState == Square.State.tbr)
            {
                int t = square.Top.AssignVertex(this.Vertices[gridRow, gridCol]);
                int b = square.Bottom.AssignVertex(this.Vertices[gridRow, gridCol]);
                int tr = mesh.Corners[(int)square.TopRight.x, (int)square.TopRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int br = mesh.Corners[(int)square.BottomRight.x, (int)square.BottomRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                AddSquare(this.Triangles[gridRow, gridCol], br, b, t, tr, inverted);
            }
            else if (square.CurrentState == Square.State.tbl)
            {
                int t = square.Top.AssignVertex(this.Vertices[gridRow, gridCol]);
                int b = square.Bottom.AssignVertex(this.Vertices[gridRow, gridCol]);
                int tl = mesh.Corners[(int)square.TopLeft.x, (int)square.TopLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int bl = mesh.Corners[(int)square.BottomLeft.x, (int)square.BottomLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                AddSquare(this.Triangles[gridRow, gridCol], b, bl, tl, t, inverted);
            }
            else if (square.CurrentState == Square.State.tld)
            {
                int t = square.Top.AssignVertex(this.Vertices[gridRow, gridCol]);
                int l = square.Left.AssignVertex(this.Vertices[gridRow, gridCol]);
                int tr = mesh.Corners[(int)square.TopRight.x, (int)square.TopRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int bl = mesh.Corners[(int)square.BottomLeft.x, (int)square.BottomLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int br = mesh.Corners[(int)square.BottomRight.x, (int)square.BottomRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                AddPentagon(this.Triangles[gridRow, gridCol], bl, l, t, tr, br, inverted);
            }
            else if (square.CurrentState == Square.State.tlu)
            {
                int tl = mesh.Corners[(int)square.TopLeft.x, (int)square.TopLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int t = square.Top.AssignVertex(this.Vertices[gridRow, gridCol]);
                int l = square.Left.AssignVertex(this.Vertices[gridRow, gridCol]);
                AddTriangle(this.Triangles[gridRow, gridCol], l, tl, t, inverted);
            }
            else if (square.CurrentState == Square.State.tru)
            {
                int tr = mesh.Corners[(int)square.TopRight.x, (int)square.TopRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int t = square.Top.AssignVertex(this.Vertices[gridRow, gridCol]);
                int r = square.Right.AssignVertex(this.Vertices[gridRow, gridCol]);
                AddTriangle(this.Triangles[gridRow, gridCol], t, tr, r, inverted);
            }
            else if (square.CurrentState == Square.State.trd)
            {
                int t = square.Top.AssignVertex(this.Vertices[gridRow, gridCol]);
                int r = square.Right.AssignVertex(this.Vertices[gridRow, gridCol]);
                int tl = mesh.Corners[(int)square.TopLeft.x, (int)square.TopLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int bl = mesh.Corners[(int)square.BottomLeft.x, (int)square.BottomLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int br = mesh.Corners[(int)square.BottomRight.x, (int)square.BottomRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                AddPentagon(this.Triangles[gridRow, gridCol], tl, t, r, br, bl, inverted);
            }
            else if (square.CurrentState == Square.State.blu)
            {
                int b = square.Bottom.AssignVertex(this.Vertices[gridRow, gridCol]);
                int l = square.Left.AssignVertex(this.Vertices[gridRow, gridCol]);
                int tl = mesh.Corners[(int)square.TopLeft.x, (int)square.TopLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int tr = mesh.Corners[(int)square.TopRight.x, (int)square.TopRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int br = mesh.Corners[(int)square.BottomRight.x, (int)square.BottomRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                AddPentagon(this.Triangles[gridRow, gridCol], br, b, l, tl, tr, inverted);
            }
            else if (square.CurrentState == Square.State.bld)
            {
                int bl = mesh.Corners[(int)square.BottomLeft.x, (int)square.BottomLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int b = square.Bottom.AssignVertex(this.Vertices[gridRow, gridCol]);
                int l = square.Left.AssignVertex(this.Vertices[gridRow, gridCol]);
                AddTriangle(this.Triangles[gridRow, gridCol], b, bl, l, inverted);
            }
            else if (square.CurrentState == Square.State.brd)
            {
                int br = mesh.Corners[(int)square.BottomRight.x, (int)square.BottomRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int b = square.Bottom.AssignVertex(this.Vertices[gridRow, gridCol]);
                int r = square.Right.AssignVertex(this.Vertices[gridRow, gridCol]);
                AddTriangle(this.Triangles[gridRow, gridCol], r, br, b, inverted);
            }
            else if (square.CurrentState == Square.State.bru)
            {
                int b = square.Bottom.AssignVertex(this.Vertices[gridRow, gridCol]);
                int r = square.Right.AssignVertex(this.Vertices[gridRow, gridCol]);
                int tl = mesh.Corners[(int)square.TopLeft.x, (int)square.TopLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int tr = mesh.Corners[(int)square.TopRight.x, (int)square.TopRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int bl = mesh.Corners[(int)square.BottomLeft.x, (int)square.BottomLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                AddPentagon(this.Triangles[gridRow, gridCol], tr, r, b, bl, tl, inverted);
            }
            else if (square.CurrentState == Square.State.lru)
            {
                int l = square.Left.AssignVertex(this.Vertices[gridRow, gridCol]);
                int r = square.Right.AssignVertex(this.Vertices[gridRow, gridCol]);
                int tl = mesh.Corners[(int)square.TopLeft.x, (int)square.TopLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int tr = mesh.Corners[(int)square.TopRight.x, (int)square.TopRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                AddSquare(this.Triangles[gridRow, gridCol], r, l, tl, tr, inverted);
            }
            else if (square.CurrentState == Square.State.lrd)
            {
                int l = square.Left.AssignVertex(this.Vertices[gridRow, gridCol]);
                int r = square.Right.AssignVertex(this.Vertices[gridRow, gridCol]);
                int bl = mesh.Corners[(int)square.BottomLeft.x, (int)square.BottomLeft.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                int br = mesh.Corners[(int)square.BottomRight.x, (int)square.BottomRight.y].AssignVertex(this.Vertices[gridRow, gridCol]);
                AddSquare(this.Triangles[gridRow, gridCol], br, bl, l, r, inverted);
            }
        }

        private void ProcessSquare(MeshGrid mesh, int r, int c, Vector2[] intersections)
        {
            Square square = mesh.Squares[r, c];
            if(intersections[0] != Vector2.zero)
                square.Top.SetPosition(intersections[0]);
            if (intersections[1] != Vector2.zero)
                square.Bottom.SetPosition(intersections[1]);
            if (intersections[2] != Vector2.zero)
                square.Left.SetPosition(intersections[2]);
            if (intersections[3] != Vector2.zero)
                square.Right.SetPosition(intersections[3]);

            if (intersections[0] != Vector2.zero && intersections[1] != Vector2.zero)
            {
                if (square.Center.x < intersections[4].x)
                    square.SetState(Square.State.tbr);
                else
                    square.SetState(Square.State.tbl);
            }
            else if (intersections[0] != Vector2.zero && intersections[2] != Vector2.zero)
            {
                if (square.Center.y < intersections[4].y)
                    square.SetState(Square.State.tlu);
                else
                    square.SetState(Square.State.tld);
            }
            else if (intersections[0] != Vector2.zero && intersections[3] != Vector2.zero)
            {
                if (square.Center.y < intersections[4].y)
                    square.SetState(Square.State.tru);
                else
                    square.SetState(Square.State.trd);
            }
            else if (intersections[1] != Vector2.zero && intersections[2] != Vector2.zero)
            {
                if (square.Center.y < intersections[4].y)
                    square.SetState(Square.State.blu);
                else
                    square.SetState(Square.State.bld);
            }
            else if (intersections[1] != Vector2.zero && intersections[3] != Vector2.zero)
            {
                if (square.Center.y < intersections[4].y)
                    square.SetState(Square.State.bru);
                else
                    square.SetState(Square.State.brd);
            }
            else if (intersections[2] != Vector2.zero && intersections[3] != Vector2.zero)
            {
                if (square.Center.y < intersections[4].y)
                    square.SetState(Square.State.lru);
                else
                    square.SetState(Square.State.lrd);
            }
        }
    }
}
