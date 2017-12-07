namespace GraphicsResearch.MeshGeneration
{
    using System.Collections.Generic;
    using UnityEngine;
    using Util;

    public class MeshGrid
    {
        public Square[,] Squares { get; private set; }
        public Vector3 TopLeft { get { return this.topLeft; } }

        private int numRows;
        private int numCols;
        private Vector3 topLeft;
        private Vector2 squareDim;

        public MeshGrid(int rows, int cols, Vector3 topLeft, Vector2 squareDim)
        {
            topLeft = topLeft + new Vector3(squareDim.x / 2f, -squareDim.y / 2f, 0f);
            this.numRows = rows;
            this.numCols = cols;
            this.topLeft = topLeft;
            this.squareDim = squareDim;
            this.Squares = new Square[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    this.Squares[r, c] = new Square(new Vector2Int(r, c), GridUtil.GetPos(r, c, topLeft, squareDim), squareDim);

                    if (c == 0)
                    {
                        this.Squares[r, c].TopLeft = new Corner(new Vector3(this.Squares[r, c].Center.x - squareDim.x / 2f, this.Squares[r, c].Center.y + squareDim.y / 2f, topLeft.z));
                        this.Squares[r, c].TopRight = new Corner(new Vector3(this.Squares[r, c].Center.x + squareDim.x / 2f, this.Squares[r, c].Center.y + squareDim.y / 2f, topLeft.z));
                        this.Squares[r, c].Top = new Corner(Vector3.zero);
                    }
                    else
                    {
                        this.Squares[r, c].TopLeft = this.Squares[r, c - 1].BottomLeft;
                        this.Squares[r, c].TopRight = this.Squares[r, c - 1].BottomRight;
                        this.Squares[r, c].Top = this.Squares[r, c - 1].Bottom;
                    }

                    if (r == 0)
                    {
                        this.Squares[r, c].BottomLeft = new Corner(new Vector3(this.Squares[r, c].Center.x - squareDim.x / 2f, this.Squares[r, c].Center.y - squareDim.y / 2f, topLeft.z));
                        this.Squares[r, c].Left = new Corner(Vector3.zero);
                    }
                    else
                    {
                        this.Squares[r, c].BottomLeft = this.Squares[r - 1, c].BottomRight;
                        this.Squares[r, c].Left = this.Squares[r - 1, c].Right;
                    }

                    this.Squares[r, c].BottomRight = new Corner(new Vector3(this.Squares[r, c].Center.x + squareDim.x / 2f, this.Squares[r, c].Center.y - squareDim.y / 2f, topLeft.z));
                    this.Squares[r, c].Right = new Corner(Vector3.zero);
                    this.Squares[r, c].Bottom = new Corner(Vector3.zero);
                }
            }
        }
        
        public Square GetSquare(Vector3 position)
        {
            int r = GridUtil.GetRow(position, this.topLeft, this.squareDim, this.numRows);
            int c = GridUtil.GetCol(position, this.topLeft, this.squareDim, this.numCols);
            return this.Squares[r, c];
        }

        public MeshGrid Duplicate(int zOffset)
        {
            MeshGrid dupe = new MeshGrid(this.numRows, this.numCols, new Vector3(this.topLeft.x, this.topLeft.y, this.topLeft.z + zOffset), this.squareDim);

            for (int r = 0; r < this.numRows; r++)
            {
                for (int c = 0; c < this.numCols; c++)
                {
                    if (this.Squares[r, c].Filled)
                    {
                        dupe.Squares[r, c].Fill();
                        this.Squares[r, c].CopyIntersections(dupe.Squares[r, c], this.topLeft.z + zOffset);
                    }
                }
            }

            return dupe;
        }
    }
}
