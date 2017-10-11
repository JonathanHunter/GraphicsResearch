namespace GraphicsResearch.MeshGeneration
{
    using System.Collections.Generic;
    using UnityEngine;
    using Util;

    public class MeshGrid
    {
        public Square[,] Squares { get; private set; }
        public Corner[,] Corners { get; private set; }
        public Vector3 TopLeft { get { return this.topLeft; } }

        private int numRows;
        private int numCols;
        private Vector3 topLeft;
        private Vector2 squareDim;

        public MeshGrid(int rows, int cols, Vector3 topLeft, Vector2 squareDim)
        {
            this.numRows = rows;
            this.numCols = cols;
            this.topLeft = topLeft;
            this.squareDim = squareDim;
            this.Squares = new Square[rows, cols];
            this.Corners = new Corner[rows + 1, cols + 1];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    this.Squares[r, c] = new Square(new Vector2(r, c), GridUtil.GetPos(r, c, topLeft, squareDim), squareDim);
                }
            }

            Vector3 topLeftCorner = new Vector3(topLeft.x - squareDim.x / 2f, topLeft.y + squareDim.y / 2f, topLeft.z);
            for (int r = 0; r < rows + 1; r++)
            {
                for (int c = 0; c < cols + 1; c++)
                {
                    this.Corners[r, c] = new Corner(new Vector2(r, c), GridUtil.GetPos(r, c, topLeftCorner, squareDim));
                }
            }
        }

        public void Fill(Vector3 position, List<Vector3> vertices)
        {
            int r = GridUtil.GetRow(position, this.topLeft, this.squareDim, this.numRows);
            int c = GridUtil.GetCol(position, this.topLeft, this.squareDim, this.numCols);
            Square square = this.Squares[r, c];
            square.Fill();
            this.Corners[(int)square.TopLeft.x, (int)square.TopLeft.y].Fill(vertices);
            this.Corners[(int)square.TopRight.x, (int)square.TopRight.y].Fill(vertices);
            this.Corners[(int)square.BottomLeft.x, (int)square.BottomLeft.y].Fill(vertices);
            this.Corners[(int)square.BottomRight.x, (int)square.BottomRight.y].Fill(vertices);
        }

        public MeshGrid Duplicate(int zOffset, List<Vector3> vertices)
        {
            MeshGrid dupe = new MeshGrid(this.numRows, this.numCols, new Vector3(this.topLeft.x, this.topLeft.y, this.topLeft.z + zOffset), this.squareDim);

            for (int r = 0; r < this.numRows; r++)
            {
                for (int c = 0; c < this.numCols; c++)
                {
                    if (this.Squares[r, c].Filled)
                        dupe.Squares[r, c].Fill();
                }
            }

            for (int r = 0; r < this.numRows + 1; r++)
            {
                for (int c = 0; c < this.numCols + 1; c++)
                {
                    if (this.Corners[r, c].Filled)
                        dupe.Corners[r, c].Fill(vertices);
                }
            }

            return dupe;
        }
    }
}
