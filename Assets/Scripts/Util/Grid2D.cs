namespace GraphicsResearch.Util
{
    using UnityEngine;

    public class Grid2D<T>
    {
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
            Vector2 pos = this.topLeft.position;
            float dist = position.x - pos.x + this.boxSize.x / 2f;
            int row = (int)(dist / this.boxSize.x);
            if (row < 0)
                return 0;
            else if (row >= this.numRows)
                return this.numRows - 1;
            else
                return row;
        }

        public int GetCol(Vector2 position)
        {
            Vector2 pos = this.topLeft.position;
            float dist = pos.y - position.y + this.boxSize.y / 2f;
            int col = (int)(dist / this.boxSize.y);
            if (col < 0)
                return 0;
            else if (col >= this.numCols)
                return this.numCols - 1;
            else
                return col;
        }

        public Vector2 GetPos(int r, int c)
        {
            Vector2 pos = this.topLeft.position;
            return pos + new Vector2(this.boxSize.x * r, -this.boxSize.y * c);
        }
    }
}
