namespace GraphicsResearch.Generation.Meshes
{
    using UnityEngine;
    using Grids;

    public class Rasterizer : MonoBehaviour
    {
        public Square[,,,] InitialzeGrid(RasterizationGrid rast)
        {
            Vector2Int sectors = rast.SectorDimension;
            Vector2Int grids = rast.GridDimension;
            Square[,,,] squares = new Square[sectors.x, sectors.y, grids.x, grids.y];
            for (int sr = 0; sr < sectors.x; sr++)
            {
                for (int sc = 0; sc < sectors.y; sc++)
                {
                    Vector2Int sector = new Vector2Int(sr, sc);
                    for (int gr = 0; gr < grids.x; gr++)
                    {
                        for (int gc = 0; gc < grids.y; gc++)
                        {
                            Vector3 pos = rast.GridCenter(sector, new Vector2Int(gr, gc));
                            Vector2 size = rast.GridSize;
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

            return squares;
        }

        public void RasterizeCircle(Square[,,,] squares, RasterizationGrid rast, Vector3 center, float radius)
        {
            Vector2 topLeft = new Vector2(center.x - radius, center.y + radius);
            Vector2 bottomRight = new Vector2(center.x + radius, center.y - radius);
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
                            squares[sr, sc, gr, gc].FillCircle(center, radius);
                        }
                    }
                }
            }
        }

        public void RasterizeBox(Square[,,,] squares, RasterizationGrid rast, Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br)
        {
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
                            squares[sr, sc, gr, gc].FillBox(tl, tr, bl, br);
                        }
                    }
                }
            }
        }
    }
}
