namespace GraphicsResearch.Generation.Meshes
{
    using System.Collections.Generic;

    public class MeshUtility 
    {
        public static void AddTriangle(List<int> triangles, int a, int b, int c, bool invert)
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

        public static void AddSquare(List<int> triangles, int a, int b, int c, int d, bool invert)
        {
            AddTriangle(triangles, a, b, c, invert);
            AddTriangle(triangles, d, a, c, invert);
        }

        public static void AddPentagon(List<int> triangles, int a, int b, int c, int d, int e, bool invert)
        {
            AddTriangle(triangles, e, a, b, invert);
            AddTriangle(triangles, e, b, c, invert);
            AddTriangle(triangles, e, c, d, invert);
        }
    }
}
