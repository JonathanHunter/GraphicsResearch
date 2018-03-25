namespace GraphicsResearch.Generation.Meshes
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Corner
    {
        /// <summary> The index for this corner in the vertices table. </summary>
        public int VertexIndex { get; private set; }
        /// <summary> The 3D position of this corner. </summary>
        public Vector3 Position { get; private set; }
        /// <summary> Whether or not this corner is in a shape. </summary>
        public bool Filled { get; set; }

        public Corner(Vector3 position)
        {
            this.Position = position;
            this.VertexIndex = -1;
            this.Filled = false;
        }

        public void SetPosition(Vector3 pos, List<Vector3> vertices = null)
        {
            this.Position = pos;
            if (this.VertexIndex != -1 && vertices != null)
                vertices[this.VertexIndex] = pos;
        }

        public int AssignVertex(List<Vector3> vertices)
        {
            if (this.VertexIndex == -1)
            {
                this.VertexIndex = vertices.Count;
                vertices.Add(this.Position);
            }

            return this.VertexIndex;
        }
    }
}
