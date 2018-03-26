namespace GraphicsResearch.Generation.Grids
{
    using UnityEngine;

    public class SamplingGrid : Grid
    {
        [SerializeField]
        private Transform topLeft;

        [SerializeField]
        private Vector2Int dimension;

        [SerializeField]
        private Vector2 boxSize;
        
        public Vector2Int Dimension { get { return this.dimension; } }
        public Vector2 BoxSize { get { return this.boxSize; } }

        protected override void Draw()
        {
            for (int r = 0; r < this.dimension.x; r++)
            {
                for (int c = 0; c < this.dimension.y; c++)
                {
                    Gizmos.DrawWireCube(this.GetPos(r, c), this.boxSize);
                }
            }
        }

        public Vector2 GetPos(int r, int c)
        {
            return Grid.GetPosition(r, c, this.topLeft.localPosition, this.boxSize);
        }
    }
}
