namespace GraphicsResearch.MeshGeneration
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Corner
    {
        /// <summary> The row and col of this corner in the grid. </summary>
        public Vector2 Index { get; private set; }
        /// <summary> The index for this corner in the vertices table. </summary>
        public int VertexIndex { get; private set; }
        /// <summary> The 3D position of this corner. </summary>
        public Vector3 Position { get; private set; }
        /// <summary> True if this corner is attached to a square with something in it. </summary>
        public bool Filled { get; internal set; }

        public Corner(Vector2 index, Vector3 position)
        {
            this.Index = index;
            this.Position = position;
            this.VertexIndex = -1;
            this.Filled = false;
        }

        public void Fill(List<Vector3> vertices)
        {
            this.Filled = true;
            if (this.VertexIndex == -1)
            {
                this.VertexIndex = vertices.Count;
                vertices.Add(this.Position);
            }
        }
    }
}
