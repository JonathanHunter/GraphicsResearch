namespace GraphicsResearch.Generation.Meshes
{
    using System.Collections.Generic;

    public class MeshUtility 
    {
        /// <summary> Adds the verts in the correct order to make a triangle. </summary>
        /// <param name="triangles"> The list to add to. </param>
        /// <param name="a"> Bottom Right. </param>
        /// <param name="b"> Bottom Left. </param>
        /// <param name="c"> Top Left. </param>
        /// <param name="invert"> Whether or not to reverse the order. </param>
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

        /// <summary> Adds the verts in the correct order to make a square. </summary>
        /// <param name="triangles"> The list to add to. </param>
        /// <param name="a"> Bottom Right. </param>
        /// <param name="b"> Bottom Left. </param>
        /// <param name="c"> Top Left. </param>
        /// <param name="d"> Top Right. </param>
        /// <param name="invert"> Whether or not to reverse the order. </param>
        public static void AddSquare(List<int> triangles, int a, int b, int c, int d, bool invert)
        {
            AddTriangle(triangles, a, b, c, invert);
            AddTriangle(triangles, d, a, c, invert);
        }

        /// <summary> Adds the verts in the correct order to make a pentagon. </summary>
        /// <param name="triangles"> The list to add to. </param>
        /// <param name="a"> Bottom Right. </param>
        /// <param name="b"> Bottom Left. </param>
        /// <param name="c"> Left. </param>
        /// <param name="d"> Top. </param>
        /// <param name="e"> Top Right. </param>
        /// <param name="invert"> Whether or not to reverse the order. </param>
        public static void AddPentagon(List<int> triangles, int a, int b, int c, int d, int e, bool invert)
        {
            AddTriangle(triangles, a, b, c, invert);
            AddTriangle(triangles, a, c, d, invert);
            AddTriangle(triangles, a, d, e, invert);
        }
    }
}
