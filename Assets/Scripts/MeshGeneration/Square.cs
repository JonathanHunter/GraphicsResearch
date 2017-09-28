namespace GraphicsResearch.MeshGeneration
{
    using UnityEngine;

    public class Square
    {
        /// <summary> The row and col of this square in the grid. </summary>
        public Vector2 Index { get; private set; }
        /// <summary> The 3D position of the center of this square. </summary>
        public Vector3 Center { get; private set; }
        /// <summary> The dimensions of this square. </summary>
        public Vector2 Size { get; private set; }
        /// <summary> True if this square has something in it. </summary>
        public bool Filled { get; private set; }

        public Vector2 TopLeft { get { return this.Index; } }
        public Vector2 TopRight { get { return new Vector2(this.Index.x, this.Index.y + 1); } }
        public Vector2 BottomLeft { get { return new Vector2(this.Index.x + 1, this.Index.y); } }
        public Vector2 BottomRight { get { return new Vector2(this.Index.x + 1, this.Index.y + 1); } }

        public Square(Vector2 index, Vector3 position, Vector2 dimensions)
        {
            this.Index = index;
            this.Center = position;
            this.Size = dimensions;
            this.Filled = false;
        }

        public void Fill()
        {
            this.Filled = true;
        }
    }
}
