namespace GraphicsResearch.Util
{
    using UnityEngine;

    public class Grid2D<T>
    {
        public Transform TopLeft { get { return this.topLeft; } }
        public Vector2 BoxSize { get { return this.boxSize; } }
        public int Rows { get { return this.numRows; } }
        public int Cols { get { return this.numCols; } }

        private T[,] grid;
        private Transform topLeft;
        private Vector2 boxSize;
        private int numRows;
        private int numCols;

        public Grid2D(int rows, int cols, Transform topLeft, Vector2 boxSize)
        {
            this.grid = new T[rows, cols];
            this.topLeft = topLeft;
            this.boxSize = boxSize;
            this.numRows = rows;
            this.numCols = cols;
        }

        public T Get(int r, int c)
        {
            return this.grid[r, c];
        }

        public void Set(int r, int c, T value)
        {
            this.grid[r, c] = value;
        }

        public int GetRow(Vector2 position)
        {
            return GridUtil.GetRow(position, this.topLeft.position, this.boxSize, this.numRows);
        }

        public int GetCol(Vector2 position)
        {
            return GridUtil.GetCol(position, this.topLeft.position, this.boxSize, this.numCols);
        }

        public Vector2 GetPos(int r, int c)
        {
            return GridUtil.GetPos(r, c, this.topLeft.position, this.boxSize);
        }
    }
}
