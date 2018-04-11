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
            Square[,,,] squares = f.Squares;

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

            if (f.Paths != null)
            {
                foreach (FloorPath p in f.Paths)
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
            }

            f.Squares = squares;
        }
    }
}
