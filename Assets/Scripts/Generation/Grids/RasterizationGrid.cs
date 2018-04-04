namespace GraphicsResearch.Generation.Grids
{
    using UnityEngine;
    using Meshes;

    public class RasterizationGrid : DataGrid
    {
        [SerializeField]
        private Transform topLeft;

        [SerializeField]
        private Vector2Int sectorDimension;

        [SerializeField]
        private Vector2Int gridDimension;

        [SerializeField]
        private Vector2 gridSize;

        public Vector2Int SectorDimension { get { return this.sectorDimension; } }
        public Vector2Int GridDimension { get { return this.gridDimension; } }
        public Vector2 SectorSize { get { return Vector2.Scale(this.gridSize, this.gridDimension); } }
        public Vector2 GridSize { get { return this.gridSize; } }
        
        public void Draw(Floor f, Vector2Int draw, Vector2Int detail)
        {
            for (int r = 0; r < this.SectorDimension.x; r++)
            {
                for (int c = 0; c < this.SectorDimension.y; c++)
                {
                    Vector2 boxCenter = GetPosition(r, c, this.topLeft.localPosition, this.SectorSize);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(boxCenter, this.SectorSize);
                    Gizmos.color = Color.white;
                }
            }

            Vector3 sectorTopLeft = SectorTopLeft(draw);

            for (int r = 0; r < this.GridDimension.x; r++)
            {
                for (int c = 0; c < this.GridDimension.y; c++)
                {
                    if (f.Squares != null)
                    {
                        if (f.Squares[draw.x, draw.y, r, c].Reserved)
                            Gizmos.color = Color.blue;
                        if (f.Squares[draw.x, draw.y, r, c].Marked)
                            Gizmos.color = Color.green;
                    }

                    Vector3 center = GetPosition(r, c, sectorTopLeft, this.gridSize);
                    Gizmos.DrawWireCube(center, this.gridSize);

                    if (f.Squares != null && f.Squares[draw.x, draw.y, r, c].Filled)
                    {
                        if (r == detail.x && c == detail.y)
                        {
                            Square s = f.Squares[draw.x, draw.y, r, c];
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere(s.Top.Position, GridSize.x / 10f);
                            Gizmos.color = Color.blue;
                            Gizmos.DrawSphere(s.Bottom.Position, GridSize.x / 10f);
                            Gizmos.color = Color.green;
                            Gizmos.DrawSphere(s.Left.Position, GridSize.x / 10f);
                            Gizmos.color = Color.yellow;
                            Gizmos.DrawSphere(s.Right.Position, GridSize.x / 10f);
                            Gizmos.color = Color.black;
                        }

                        Gizmos.DrawCube(center, this.GridSize / 2f);
                    }

                    Gizmos.color = Color.white;
                }
            }
        }

        public void Draw(MiddleLayer l, Vector2Int draw)
        {
            for (int r = 0; r < this.SectorDimension.x; r++)
            {
                for (int c = 0; c < this.SectorDimension.y; c++)
                {
                    Vector2 boxCenter = GetPosition(r, c, this.topLeft.localPosition, this.SectorSize);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(boxCenter, this.SectorSize);
                    Gizmos.color = Color.white;
                }
            }

            Vector3 sectorTopLeft = SectorTopLeft(draw);

            for (int r = 0; r < this.GridDimension.x; r++)
            {
                for (int c = 0; c < this.GridDimension.y; c++)
                {
                    if (l.Squares != null)
                    {
                        if (l.Squares[draw.x, draw.y, r, c].Reserved)
                            Gizmos.color = Color.blue;
                        if (l.Squares[draw.x, draw.y, r, c].Marked)
                            Gizmos.color = Color.green;
                    }

                    Vector3 center = GetPosition(r, c, sectorTopLeft, this.gridSize);
                    Gizmos.DrawWireCube(center, this.gridSize);

                    if (l.Squares != null && l.Squares[draw.x, draw.y, r, c].Filled)
                        Gizmos.DrawCube(center, this.GridSize / 2f);

                    Gizmos.color = Color.white;
                }
            }
        }

        public Vector3 SectorCenter(Vector2Int sector)
        {
            return GetPosition(sector.x, sector.y, this.topLeft.localPosition, this.SectorSize);
        }

        public int SectorRow(Vector3 position)
        {
            return GetRow(position, this.topLeft.localPosition, this.SectorSize, this.SectorDimension.x);
        }

        public int SectorColumn(Vector3 position)
        {
            return GetColumn(position, this.topLeft.localPosition, this.SectorSize, this.SectorDimension.y);
        }

        public Vector3 GridCenter(Vector2Int sector, Vector2Int grid)
        {
            Vector3 sectorTopLeft = SectorTopLeft(sector);
            return GetPosition(grid.x, grid.y, sectorTopLeft, this.GridSize);
        }

        public int GridRow(Vector2Int sector, Vector3 position)
        {
            Vector3 sectorTopLeft = SectorTopLeft(sector);
            return GetRow(position, sectorTopLeft, this.GridSize, this.GridDimension.x);
        }

        public int GridColumn(Vector2Int sector, Vector3 position)
        {
            Vector3 sectorTopLeft = SectorTopLeft(sector);
            return GetColumn(position, sectorTopLeft, this.GridSize, this.GridDimension.y);
        }

        private Vector3 SectorTopLeft(Vector2Int sector)
        {
            return SectorCenter(sector) + 
                new Vector3((this.GridSize.x - this.SectorSize.x) / 2f, (this.SectorSize.y - this.GridSize.y) / 2f);
        }
    }
}
