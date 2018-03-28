namespace GraphicsResearch.Generation.Meshes
{
    using System.Collections;
    using UnityEngine;
    using Paths;
    using Rooms;

    public class FloorRasterizer : Rasterizer
    {
        public IEnumerator RasterizeFloor(Floor f)
        {
            Vector2Int sectors = f.rasterizationGrid.SectorDimension;
            Vector2Int grids = f.rasterizationGrid.GridDimension;
            Square[,,,] squares = new Square[sectors.x, sectors.y, grids.x, grids.y];
            for(int sr = 0; sr < sectors.x; sr++)
            {
                for(int sc = 0; sc < sectors.y; sc++)
                {
                    Vector2Int sector = new Vector2Int(sr, sc);
                    for(int gr = 0; gr < grids.x; gr++)
                    {
                        for(int gc = 0; gc < grids.y; gc++)
                        {
                            Vector3 pos = f.rasterizationGrid.GridCenter(sector, new Vector2Int(gr, gc));
                            Vector2 size = f.rasterizationGrid.GridSize;
                            Square square = new Square(pos, size);
                            squares[sr, sc, gr, gc] = square;

                            if (gc == 0)
                            {
                                square.TopLeft = new Corner(new Vector3(pos.x - size.x / 2f, pos.y + size.y / 2f));
                                square.TopRight = new Corner(new Vector3(pos.x + size.x / 2f, pos.y + size.y / 2f));
                                square.Top = new Corner(Vector3.zero);
                            }
                            else
                            {
                                square.TopLeft = squares[sr, sc, gr, gc - 1].BottomLeft;
                                square.TopRight = squares[sr, sc, gr, gc - 1].BottomRight;
                                square.Top = squares[sr, sc, gr, gc - 1].Bottom;
                            }

                            if (gr == 0)
                            {
                                square.BottomLeft = new Corner(new Vector3(pos.x - size.x / 2f, pos.y - size.y / 2f));
                                square.Left = new Corner(Vector3.zero);
                            }
                            else
                            {
                                square.BottomLeft = squares[sr, sc, gr - 1, gc].BottomRight;
                                square.Left = squares[sr, sc, gr - 1, gc].Right;
                            }

                            square.BottomRight = new Corner(new Vector3(pos.x + size.x / 2f, pos.y - size.y / 2f));
                            square.Right = new Corner(Vector3.zero);
                            square.Bottom = new Corner(Vector3.zero);
                        }
                    }
                }
            }
            yield return null;

            foreach (Room room in f.Rooms)
            {
                if(room is CircleRoom)
                {
                    CircleRoom circle = (CircleRoom)room;
                    RasterizeCircle(squares, f.rasterizationGrid, circle.Position, circle.Radius);
                }
                else
                {
                    RectangleRoom rect = (RectangleRoom)room;
                    Vector3 tl, tr, bl, br;
                    rect.BoxBounds(out tl, out tr, out bl, out br);
                    RasterizeBox(squares, f.rasterizationGrid, tl, tr, bl, br);
                }
            }
            yield return null;

            foreach(FloorPath p in f.Paths)
            {
                foreach (Edge e in p.Edges)
                {
                    Vector3 tl, tr, bl, br;
                    GenerationUtility.BoxBounds(e.Start, e.End, e.Width, out tl, out tr, out bl, out br);
                    RasterizeBox(squares, f.rasterizationGrid, tl, tr, bl, br);
                    if (!e.TailEdge)
                        RasterizeCircle(squares, f.rasterizationGrid, e.End, e.Width / 2f);
                }
            }

            f.Squares = squares;
        }
    }
}
