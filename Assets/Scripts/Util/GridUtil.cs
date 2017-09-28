namespace GraphicsResearch.Util
{
    using UnityEngine;

    public class GridUtil
    {
        public static int GetRow(Vector2 position, Vector3 topLeft, Vector2 boxSize, int numRows)
        {
            Vector2 pos = topLeft;
            float dist = position.x - pos.x + boxSize.x / 2f;
            int row = (int)(dist / boxSize.x);
            if (row < 0)
                return 0;
            else if (row >= numRows)
                return numRows - 1;
            else
                return row;
        }

        public static int GetCol(Vector2 position, Vector3 topLeft, Vector2 boxSize, int numCols)
        {
            Vector2 pos = topLeft;
            float dist = pos.y - position.y + boxSize.y / 2f;
            int col = (int)(dist / boxSize.y);
            if (col < 0)
                return 0;
            else if (col >= numCols)
                return numCols - 1;
            else
                return col;
        }

        public static Vector3 GetPos(int r, int c, Vector3 topLeft, Vector2 boxSize)
        {
            return topLeft + new Vector3(boxSize.x * r, -boxSize.y * c, 0);
        }
    }
}
