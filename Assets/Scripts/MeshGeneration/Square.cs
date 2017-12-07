namespace GraphicsResearch.MeshGeneration
{
    using UnityEngine;

    public class Square
    {
        public enum State { NoIntersection, tbl, tbr, tlu, tld, tru, trd, blu, bld, bru, brd, lru, lrd }

        /// <summary> The row and col of this square in the grid. </summary>
        public Vector2Int Index { get; private set; }
        /// <summary> The 3D position of the center of this square. </summary>
        public Vector3 Center { get; private set; }
        /// <summary> The dimensions of this square. </summary>
        public Vector2 Size { get; private set; }
        /// <summary> True if this square has something in it. </summary>
        public bool Filled { get; private set; }
        /// <summary> True if this square has multiple shapes overlapping in it. </summary>
        public bool MultiOverlap { get; private set; }
        /// <summary> The current state of this square. </summary>
        public State CurrentState { get; private set; }

        public Vector2Int TopLeft { get { return this.Index; } }
        public Vector2Int TopRight { get { return new Vector2Int(this.Index.x + 1, this.Index.y); } }
        public Vector2Int BottomLeft { get { return new Vector2Int(this.Index.x, this.Index.y + 1); } }
        public Vector2Int BottomRight { get { return new Vector2Int(this.Index.x + 1, this.Index.y + 1); } }
        public Corner Top { get; set; }
        public Corner Bottom { get; set; }
        public Corner Left { get; set; }
        public Corner Right { get; set; }

        public Square(Vector2Int index, Vector3 position, Vector2 dimensions)
        {
            this.Index = index;
            this.Center = position;
            this.Size = dimensions;
            this.Filled = false;
            this.MultiOverlap = false;
        }

        public void Fill()
        {
            if(this.Filled && !this.MultiOverlap)
            {
                this.CurrentState = State.NoIntersection;
                this.MultiOverlap = true;
            }

            this.Filled = true;
        }

        public void SetState(State state)
        {
            if (this.CurrentState == State.NoIntersection && !this.MultiOverlap)
            {
                this.CurrentState = state;
            }
            else if (!this.MultiOverlap)
            {
                this.CurrentState = State.NoIntersection;
                this.MultiOverlap = true;
            }
        }

        public void CopyIntersections(Square s, float z)
        {
            s.Top.SetPosition(new Vector3(this.Top.Position.x, this.Top.Position.y, z));
            s.Left.SetPosition(new Vector3(this.Left.Position.x, this.Left.Position.y, z));
            s.Bottom.SetPosition(new Vector3(this.Bottom.Position.x, this.Bottom.Position.y, z));
            s.Right.SetPosition(new Vector3(this.Right.Position.x, this.Right.Position.y, z));
        }
    }
}
