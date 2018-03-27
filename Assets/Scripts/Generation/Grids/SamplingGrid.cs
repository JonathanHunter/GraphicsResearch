namespace GraphicsResearch.Generation.Grids
{
    using UnityEngine;
    using Rooms;

    public class SamplingGrid : DataGrid
    {
        [SerializeField]
        private Transform topLeft;

        [SerializeField]
        private Vector2Int dimension;

        [SerializeField]
        private Vector2 boxSize;

        [SerializeField]
        private bool showRooms;
        
        public Vector2Int Dimension { get { return this.dimension; } }
        public Vector2 BoxSize { get { return this.boxSize; } }

        public void Draw(Floor f)
        {
            for (int r = 0; r < this.dimension.x; r++)
            {
                for (int c = 0; c < this.dimension.y; c++)
                { 
                    Gizmos.DrawWireCube(this.GetPos(r, c), this.boxSize);
                }
            }

            if (this.showRooms)
            {
                foreach (Room r in f.Rooms)
                {
                    if (r is CircleRoom)
                        Gizmos.DrawWireSphere(r.Position, ((CircleRoom)r).Radius);
                    else
                    {
                        RectangleRoom rect = (RectangleRoom)r;
                        Vector3 s = rect.Position + rect.transform.parent.InverseTransformDirection(rect.transform.up) * rect.Dimentions.y / 2f;
                        Vector3 e = rect.Position - rect.transform.parent.InverseTransformDirection(rect.transform.up) * rect.Dimentions.y / 2f;
                        Gizmos.DrawLine(s, e);
                        Gizmos.matrix = Matrix4x4.TRS(r.Position, r.transform.localRotation, r.transform.localScale);
                        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                        Gizmos.matrix = Matrix4x4.identity;
                    }
                }
            }
        }

        public Vector2 GetPos(int r, int c)
        {
            return DataGrid.GetPosition(r, c, this.topLeft.localPosition, this.boxSize);
        }
    }
}
