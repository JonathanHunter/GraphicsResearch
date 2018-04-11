namespace GraphicsResearch.Generation.Meshes
{
    using System.Collections;
    using UnityEngine;
    using Grids;
    using Paths;
    using Rooms;
    using System;

    public class LayerRasterizer : Rasterizer
    {
        public IEnumerator RasterizeLayer(MiddleLayer l, Floor upper, Floor lower)
        {
            Square[,,,] squares = l.Squares;
            Vector3 tl, tr, bl, br;
            foreach (LayerPath p in l.Paths)
            {
                if (p.Start is CircleRoom)
                {
                    CircleRoom circle = (CircleRoom)p.Start;
                    RasterizeCircle(squares, l.rasterizationGrid, circle.Position, circle.Radius);
                }
                else
                {
                    RectangleRoom rect = (RectangleRoom)p.Start;
                    rect.BoxBounds(out tl, out tr, out bl, out br);
                    RasterizeBox(squares, l.rasterizationGrid, tl, tr, bl, br);
                }

                if (p.End is CircleRoom)
                {
                    CircleRoom circle = (CircleRoom)p.End;
                    RasterizeCircle(squares, l.rasterizationGrid, circle.Position, circle.Radius);
                }
                else
                {
                    RectangleRoom rect = (RectangleRoom)p.End;
                    rect.BoxBounds(out tl, out tr, out bl, out br);
                    RasterizeBox(squares, l.rasterizationGrid, tl, tr, bl, br);
                }

                GenerationUtility.BoxBounds(p.Start.Position, p.End.Position, p.Width, out tl, out tr, out bl, out br);
                RasterizeBox(squares, l.rasterizationGrid, tl, tr, bl, br);

                Vector3 start = GetRoomEdgePoint(p.Start, p);
                Vector3 end = GetRoomEdgePoint(p.End, p);
                MarkForKeeping(squares, l.rasterizationGrid, start, end, p.Width);
                ReserveGridSquares(upper, start, Vector3.Lerp(start, end, .02f), p.Width);
                ReserveGridSquares(lower, Vector3.Lerp(start, end, .98f), end, p.Width);
                Vector3 startHeight = l.transform.InverseTransformPoint(p.Start.WorldPosition);
                Vector3 endHeight = l.transform.InverseTransformPoint(p.End.WorldPosition);
                RaiseMesh(squares, l.rasterizationGrid, start, end, p.Width, startHeight, endHeight);
                yield return null;
            }

            l.Squares = squares;
        }

        private Vector3 GetRoomEdgePoint(Room room, LayerPath p)
        {
            if (room is CircleRoom)
            {
                CircleRoom c = (CircleRoom)room;
                return GenerationUtility.CircleLineIntersection(p.Start.Position, p.End.Position, c.Position, c.Radius);
            }
            else
            {
                Vector3 tl, tr, bl, br;
                RectangleRoom rect = (RectangleRoom)room;
                rect.BoxBounds(out tl, out tr, out bl, out br);
                return GenerationUtility.BoxLineIntersection(p.Start.Position, p.End.Position, tl, tr, bl, br);
            }
        }

        /// <summary> Marks squares for keeping. </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="width"></param>
        private void MarkForKeeping(Square[,,,] squares, RasterizationGrid rast, Vector3 start, Vector3 end, float width)
        {
            Vector3 tl, tr, bl, br;
            GenerationUtility.BoxBounds(start, end, width, out tl, out tr, out bl, out br);
            float left = Mathf.Min(tl.x, tr.x, bl.x, br.x);
            float right = Mathf.Max(tl.x, tr.x, bl.x, br.x);
            float up = Mathf.Max(tl.y, tr.y, bl.y, br.y);
            float down = Mathf.Min(tl.y, tr.y, bl.y, br.y);
            Vector2 topLeft = new Vector2(left, up);
            Vector2 bottomRight = new Vector2(right, down);
            int sectorTop = rast.SectorColumn(topLeft);
            int sectorBottom = rast.SectorColumn(bottomRight);
            int sectorLeft = rast.SectorRow(topLeft);
            int sectorRight = rast.SectorRow(bottomRight);
            for (int sr = sectorLeft; sr <= sectorRight; sr++)
            {
                for (int sc = sectorTop; sc <= sectorBottom; sc++)
                {
                    Vector2Int sector = new Vector2Int(sr, sc);
                    int gridTop = rast.GridColumn(sector, topLeft);
                    int gridBottom = rast.GridColumn(sector, bottomRight);
                    int gridLeft = rast.GridRow(sector, topLeft);
                    int gridRight = rast.GridRow(sector, bottomRight);
                    for (int gr = gridLeft; gr <= gridRight; gr++)
                    {
                        for (int gc = gridTop; gc <= gridBottom; gc++)
                        {
                            bool a = GenerationUtility.PointInBox(squares[sr, sc, gr, gc].TopLeft.Position, tl, tr, bl, br);
                            bool b = GenerationUtility.PointInBox(squares[sr, sc, gr, gc].TopRight.Position, tl, tr, bl, br);
                            bool c = GenerationUtility.PointInBox(squares[sr, sc, gr, gc].BottomLeft.Position, tl, tr, bl, br);
                            bool d = GenerationUtility.PointInBox(squares[sr, sc, gr, gc].BottomRight.Position, tl, tr, bl, br);
                            if(a || b || c || d)
                                squares[sr, sc, gr, gc].Marked = true;
                        }
                    }
                }
            }
        }

        private void ReserveGridSquares(Floor floor, Vector3 start, Vector3 end, float width)
        {
            RasterizationGrid rast = floor.rasterizationGrid;
            Square[,,,] squares = floor.Squares;
            Vector3 tl, tr, bl, br;
            GenerationUtility.BoxBounds(start, end, width, out tl, out tr, out bl, out br);
            float left = Mathf.Min(tl.x, tr.x, bl.x, br.x);
            float right = Mathf.Max(tl.x, tr.x, bl.x, br.x);
            float up = Mathf.Max(tl.y, tr.y, bl.y, br.y);
            float down = Mathf.Min(tl.y, tr.y, bl.y, br.y);
            Vector2 topLeft = new Vector2(left, up);
            Vector2 bottomRight = new Vector2(right, down);
            int sectorTop = rast.SectorColumn(topLeft);
            int sectorBottom = rast.SectorColumn(bottomRight);
            int sectorLeft = rast.SectorRow(topLeft);
            int sectorRight = rast.SectorRow(bottomRight);
            for (int sr = sectorLeft; sr <= sectorRight; sr++)
            {
                for (int sc = sectorTop; sc <= sectorBottom; sc++)
                {
                    Vector2Int sector = new Vector2Int(sr, sc);
                    int gridTop = rast.GridColumn(sector, topLeft);
                    int gridBottom = rast.GridColumn(sector, bottomRight);
                    int gridLeft = rast.GridRow(sector, topLeft);
                    int gridRight = rast.GridRow(sector, bottomRight);
                    for (int gr = gridLeft; gr <= gridRight; gr++)
                    {
                        for (int gc = gridTop; gc <= gridBottom; gc++)
                        {
                            bool a = GenerationUtility.PointInBox(squares[sr, sc, gr, gc].TopLeft.Position, tl, tr, bl, br);
                            bool b = GenerationUtility.PointInBox(squares[sr, sc, gr, gc].TopRight.Position, tl, tr, bl, br);
                            bool c = GenerationUtility.PointInBox(squares[sr, sc, gr, gc].BottomLeft.Position, tl, tr, bl, br);
                            bool d = GenerationUtility.PointInBox(squares[sr, sc, gr, gc].BottomRight.Position, tl, tr, bl, br);
                            if (a || b || c || d)
                                squares[sr, sc, gr, gc].Reserved = true;
                        }
                    }
                }
            }
        }

        private void RaiseMesh(Square[,,,] squares, RasterizationGrid rast, Vector3 start, Vector3 end, float width, Vector3 startHeight, Vector3 endHeight)
        {
            Vector3 es = Vector3.Normalize(start - end);
            Vector3 left = new Vector3(-es.y, es.x, es.z);
            float change = rast.GridSize.x / Vector2.Distance(start, end) / 4f;
            float change2 = rast.GridSize.x / width / 4f;
            float lerp = 0;
            float lerp2 = 0;
            while (lerp <= 1f + change)
            {
                Vector3 pos = Vector2.Lerp(start - left * width / 2f, end - left * width / 2f, lerp);
                lerp2 = 0;
                while (lerp2 <= 1f + change2)
                {
                    Vector3 cell = Vector2.Lerp(pos, pos + left * width, lerp2);
                    int sr = rast.SectorRow(cell);
                    int sc = rast.SectorColumn(cell);
                    Vector2Int sector = new Vector2Int(sr, sc);
                    int gr = rast.GridRow(sector, cell);
                    int gc = rast.GridColumn(sector, cell);
                    squares[sr, sc, gr, gc].SetZHeight(startHeight, endHeight);
                    lerp2 += change2;
                }

                lerp += change;
            }
        }
    }
}
