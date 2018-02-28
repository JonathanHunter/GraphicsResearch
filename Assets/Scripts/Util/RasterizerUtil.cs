namespace GraphicsResearch.Util
{
    using System.Collections.Generic;
    using UnityEngine;
    using MeshGeneration;
    using RoomPlacement;

    public class RasterizerUtil
    {
        public static void DrawMesh(Grid2D<MeshGrid> grids, int drawR, int drawC)
        {
            MeshGrid mesh = grids.Get(0, 0);
            Square square = mesh.Squares[0, 0];
            for (int r = 0; r < grids.Rows; r++)
            {
                for (int c = 0; c < grids.Cols; c++)
                {
                    Vector2 boxCenter = grids.GetPos(r, c);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(boxCenter, grids.BoxSize);
                    Gizmos.color = Color.white;
                }
            }

            for (int r = 0; r < grids.Get(drawR, drawC).Rows; r++)
            {
                for (int c = 0; c < grids.Get(drawR, drawC).Cols; c++)
                {
                    MeshGrid m = grids.Get(drawR, drawC);
                    Square s = m.Squares[r, c];

                    if (grids.Get(drawR, drawC) != null && grids.Get(drawR, drawC).Squares[r, c].Filled)
                    {
                        if (grids.Get(drawR, drawC).Squares[r, c].reserved)
                            Gizmos.color = Color.blue;
                        if (grids.Get(drawR, drawC).Squares[r, c].Marked)
                            Gizmos.color = Color.green;

                        Gizmos.DrawCube(s.Center, square.Size / 2f);
                        Gizmos.DrawWireCube(s.Center, square.Size);

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
                        Gizmos.DrawWireCube(s.Center, square.Size);
                }
            }
        }

        public static void GetSquareBounds(RectangleRoom rect, out Vector3 start, out Vector3 end, out float width)
        {
            start = rect.OriginalPosition + rect.transform.up * rect.Dimentions.y / 2f;
            end = rect.OriginalPosition - rect.transform.up * rect.Dimentions.y / 2f;
            width = rect.Dimentions.x;
        }

        public static void RasterizeCircle(Grid2D<MeshGrid> grids, Vector3 center, float radius)
        {
            Vector2 topLeft = new Vector2(center.x - radius, center.y + radius);
            Vector2 bottomRight = new Vector2(center.x + radius, center.y - radius);
            Vector2 size = grids.BoxSize;
            Vector2 gridSize = grids.Get(0, 0).Squares[0,0].Size;
            int gridRowLeft = GridUtil.GetRow(topLeft, grids.TopLeft.position, size, grids.Rows);
            int gridRowRight = GridUtil.GetRow(bottomRight, grids.TopLeft.position, size, grids.Rows);
            int gridColTop = GridUtil.GetCol(topLeft, grids.TopLeft.position, size, grids.Cols);
            int gridColBottom = GridUtil.GetCol(bottomRight, grids.TopLeft.position, size, grids.Cols);
            for (int r = gridRowLeft; r <= gridRowRight; r++)
            {
                for (int c = gridColTop; c <= gridColBottom; c++)
                {
                    MeshGrid mesh = grids.Get(r, c);
                    int r1 = GridUtil.GetRow(topLeft, mesh.TopLeft, gridSize, mesh.Rows);
                    int r2 = GridUtil.GetRow(bottomRight, mesh.TopLeft, gridSize, mesh.Rows);
                    int c1 = GridUtil.GetCol(topLeft, mesh.TopLeft, gridSize, mesh.Cols);
                    int c2 = GridUtil.GetCol(bottomRight, mesh.TopLeft, gridSize, mesh.Cols);
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

        public static void RasterizeBox(Grid2D<MeshGrid> grids, Vector3 start, Vector3 end, float width)
        {
            Vector2 size = grids.BoxSize;
            Vector2 gridSize = grids.Get(0, 0).Squares[0, 0].Size;
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
                    Square s = grids.Get(grids.GetRow(cell), grids.GetCol(cell)).GetSquare(cell);
                    s.FillBox(start, end, width);
                    lerp2 += change2;
                }

                lerp += change;
            }
        }
        
        public static void AddWalls(
            Grid2D<MeshGrid> grids, 
            MeshGrid top, 
            MeshGrid bottom,
            List<Vector3>[,] vertices,
            List<int>[,] triangles,
            bool invertedTriangles,
            int gridRow, 
            int gridCol, 
            int subGridRow, 
            int subGridCol)
        {
            bool left = subGridRow == 0 || !top.Squares[subGridRow - 1, subGridCol].Filled;
            bool right = subGridRow == top.Rows - 1 || !top.Squares[subGridRow + 1, subGridCol].Filled;
            bool up = subGridCol == 0 || !top.Squares[subGridRow, subGridCol - 1].Filled;
            bool down = subGridCol == top.Cols - 1 || !top.Squares[subGridRow, subGridCol + 1].Filled;

            if (subGridRow == 0 && gridRow != 0)
                left = !grids.Get(gridRow - 1, gridCol).Squares[top.Rows - 1, subGridCol].Filled;

            if (subGridRow == top.Rows - 1 && gridRow != grids.Rows - 1)
                right = !grids.Get(gridRow + 1, gridCol).Squares[0, subGridCol].Filled;

            if (subGridCol == 0 && gridCol != 0)
                up = !grids.Get(gridRow, gridCol - 1).Squares[subGridRow, top.Cols - 1].Filled;

            if (subGridCol == top.Cols - 1 && gridCol != grids.Cols - 1)
                down = !grids.Get(gridRow, gridCol + 1).Squares[subGridRow, 0].Filled;

            Square ts = top.Squares[subGridRow, subGridCol];
            Square bs = bottom.Squares[subGridRow, subGridCol];
            List<int> topPoints = new List<int>();
            List<int> bottomPoints = new List<int>();

            if (left)
            {
                List<int> temp = ts.GetWallPoints(Lib.Direction.Left);
                foreach (int i in temp)
                    topPoints.Add(i);

                temp = bs.GetWallPoints(Lib.Direction.Left);
                foreach (int i in temp)
                    bottomPoints.Add(i);
            }
            if (right)
            {
                List<int> temp = ts.GetWallPoints(Lib.Direction.Right);
                foreach (int i in temp)
                    topPoints.Add(i);

                temp = bs.GetWallPoints(Lib.Direction.Right);
                foreach (int i in temp)
                    bottomPoints.Add(i);
            }
            if (up)
            {
                List<int> temp = ts.GetWallPoints(Lib.Direction.Up);
                foreach (int i in temp)
                    topPoints.Add(i);

                temp = bs.GetWallPoints(Lib.Direction.Up);
                foreach (int i in temp)
                    bottomPoints.Add(i);
            }
            if (down)
            {
                List<int> temp = ts.GetWallPoints(Lib.Direction.Down);
                foreach (int i in temp)
                    topPoints.Add(i);

                temp = bs.GetWallPoints(Lib.Direction.Down);
                foreach (int i in temp)
                    bottomPoints.Add(i);
            }
            if (!left && !right && !up && !down)
            {
                if (!ts.TopLeft.Filled || !ts.TopRight.Filled || !ts.BottomLeft.Filled || !ts.BottomRight.Filled)
                {
                    List<int> temp = ts.GetWallPoints(Lib.Direction.Down);
                    foreach (int i in temp)
                        topPoints.Add(i);

                    temp = bs.GetWallPoints(Lib.Direction.Down);
                    foreach (int i in temp)
                        bottomPoints.Add(i);
                }
            }

            // change to add new points to fix shading (make walls their own mesh grid?)
            for (int i = 0; i < topPoints.Count - 1; i += 2)
            {
                Vector3 pos1 = vertices[gridRow, gridCol][bottomPoints[i]];
                int a = vertices[gridRow, gridCol].Count;
                vertices[gridRow, gridCol].Add(pos1);
                Vector3 pos2 = vertices[gridRow, gridCol][bottomPoints[i + 1]];
                int b = vertices[gridRow, gridCol].Count;
                vertices[gridRow, gridCol].Add(pos2);
                Vector3 pos3 = vertices[gridRow, gridCol][topPoints[i + 1]];
                int c = vertices[gridRow, gridCol].Count;
                vertices[gridRow, gridCol].Add(pos3);
                Vector3 pos4 = vertices[gridRow, gridCol][topPoints[i]];
                int d = vertices[gridRow, gridCol].Count;
                vertices[gridRow, gridCol].Add(pos4);
                Lib.AddSquare(triangles[gridRow, gridCol], a, b, c, d, invertedTriangles);
            }
        }
    }
}
