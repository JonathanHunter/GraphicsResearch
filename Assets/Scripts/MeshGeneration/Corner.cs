namespace GraphicsResearch.MeshGeneration
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Corner
    {
        /// <summary> The row and col of this corner in the grid. </summary>
        public Vector2Int Index { get; private set; }
        /// <summary> The index for this corner in the vertices table. </summary>
        public int VertexIndex { get; private set; }
        /// <summary> The 3D position of this corner. </summary>
        public Vector3 Position { get; private set; }

        public Corner(Vector2Int index, Vector3 position)
        {
            this.Index = index;
            this.Position = position;
            this.VertexIndex = -1;
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
