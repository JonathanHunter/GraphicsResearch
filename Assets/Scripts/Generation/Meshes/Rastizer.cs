namespace GraphicsResearch.Generation.Meshes
{
    using UnityEngine;
    using Grids;

    public class Rasterizer : MonoBehaviour
    {
        public static void RasterizeCircle(Square[,,,] squares, RasterizationGrid rast, Vector3 center, float radius)
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

        public static void RasterizeBox(Square[,,,] squares, RasterizationGrid rast, Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br)
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
