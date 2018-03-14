namespace GraphicsResearch.MeshGeneration
{
    using System.Collections.Generic;
    using UnityEngine;
    using Util;

    public class Square
    {
        /// <summary> The row and col of this square in the grid. </summary>
        public Vector2Int Index { get; private set; }
        /// <summary> The 3D position of the center of this square. </summary>
        public Vector3 Center { get; private set; }
        /// <summary> The dimensions of this square. </summary>
        public Vector2 Size { get; private set; }
        /// <summary> True if this square has something in it. </summary>
        public bool Filled { get { return this.filled || this.reserved; } private set { this.filled = value; } }
        /// <summary> True if this square was marked. </summary>
        public bool Marked { get { return this.marked; } }

        public bool reserved;

        public Corner TopLeft { get; set; }
        public Corner TopRight { get; set; }
        public Corner BottomLeft { get; set; }
        public Corner BottomRight { get; set; }
        public Corner Top { get; set; }
        public Corner Bottom { get; set; }
        public Corner Left { get; set; }
        public Corner Right { get; set; }

        private bool filled;
        private bool marked;

        public Square(Vector2Int index, Vector3 position, Vector2 dimensions)
        {
            this.Index = index;
            this.Center = position;
            this.Size = dimensions;
            this.Filled = false;
            this.marked = false;
            this.reserved = false;
        }

        public void Fill()
        {
            this.Filled = true;
        }

        public void Mark()
        {
            this.marked = true;
        }

        public void FillCircle(Vector3 center, float radius)
        {
            if (this.reserved)
                return;

            bool topLeft = Lib.PointInCircle(this.TopLeft.Position, center, radius);
            bool topRight = Lib.PointInCircle(this.TopRight.Position, center, radius);
            bool bottomLeft = Lib.PointInCircle(this.BottomLeft.Position, center, radius);
            bool bottomRight = Lib.PointInCircle(this.BottomRight.Position, center, radius);

            if (topLeft)
                this.TopLeft.Filled = true;
            if (topRight)
                this.TopRight.Filled = true;
            if (bottomLeft)
                this.BottomLeft.Filled = true;
            if (bottomRight)
                this.BottomRight.Filled = true;

            if (this.TopLeft.Filled || this.TopRight.Filled || this.BottomLeft.Filled || this.BottomRight.Filled)
                this.Filled = true;

            if (topLeft != topRight)
            {
                this.Top.Filled = true;
                this.Top.SetPosition(Lib.CircleLineIntersection(this.TopLeft.Position, this.TopRight.Position, center, radius));
            }

            if (bottomLeft != bottomRight)
            {
                this.Bottom.Filled = true;
                this.Bottom.SetPosition(Lib.CircleLineIntersection(this.BottomLeft.Position, this.BottomRight.Position, center, radius));
            }

            if (topLeft != bottomLeft)
            {
                this.Left.Filled = true;
                this.Left.SetPosition(Lib.CircleLineIntersection(this.TopLeft.Position, this.BottomLeft.Position, center, radius));
            }

            if (topRight != bottomRight)
            {
                this.Right.Filled = true;
                this.Right.SetPosition(Lib.CircleLineIntersection(this.TopRight.Position, this.BottomRight.Position, center, radius));
            }
        }

        public void FillBox(Vector3 start, Vector3 end, float width)
        {
            if (this.reserved)
                return;

            Vector3 es = Vector3.Normalize(start - end);
            Vector3 left = new Vector3(-es.y, es.x, es.z);
            Vector3 tl = start + left * width / 2f;
            Vector3 tr = start - left * width / 2f;
            Vector3 bl = end + left * width / 2f;
            Vector3 br = end - left * width / 2f;

            bool topLeft = Lib.PointInBox(this.TopLeft.Position, tl, tr, bl, br);
            bool topRight = Lib.PointInBox(this.TopRight.Position, tl, tr, bl, br);
            bool bottomLeft = Lib.PointInBox(this.BottomLeft.Position, tl, tr, bl, br);
            bool bottomRight = Lib.PointInBox(this.BottomRight.Position, tl, tr, bl, br);

            if (topLeft)
                this.TopLeft.Filled = true;
            if (topRight)
                this.TopRight.Filled = true;
            if (bottomLeft)
                this.BottomLeft.Filled = true;
            if (bottomRight)
                this.BottomRight.Filled = true;

            if (this.TopLeft.Filled || this.TopRight.Filled || this.BottomLeft.Filled || this.BottomRight.Filled)
                this.Filled = true;

            if (topLeft != topRight)
            {
                this.Top.Filled = true;
                this.Top.SetPosition(Lib.BoxLineIntersection(this.TopLeft.Position, this.TopRight.Position, tl, tr, bl, br));
            }

            if (bottomLeft!= bottomRight)
            {
                this.Bottom.Filled = true;
                this.Bottom.SetPosition(Lib.BoxLineIntersection(this.BottomLeft.Position, this.BottomRight.Position, tl, tr, bl, br));
            }

            if (topLeft != bottomLeft)
            {
                this.Left.Filled = true;
                this.Left.SetPosition(Lib.BoxLineIntersection(this.TopLeft.Position, this.BottomLeft.Position, tl, tr, bl, br));
            }

            if (topRight != bottomRight)
            {
                this.Right.Filled = true;
                this.Right.SetPosition(Lib.BoxLineIntersection(this.TopRight.Position, this.BottomRight.Position, tl, tr, bl, br));
            }
        }

        public List<int> GetTriangles(List<Vector3> vertices, bool inverted)
        {
            List<int> ret = new List<int>();
            if (this.reserved)
                return ret;

            // 0000 -> tl, tr, bl, br
            int state = 0;
            state = this.TopLeft.Filled ? state + 8 : state;
            state = this.TopRight.Filled ? state + 4 : state;
            state = this.BottomLeft.Filled ? state + 2 : state;
            state = this.BottomRight.Filled ? state + 1 : state;

            //if (state != 0)
            //{
            //    int tl = this.TopLeft.AssignVertex(vertices);
            //    int tr = this.TopRight.AssignVertex(vertices);
            //    int bl = this.BottomLeft.AssignVertex(vertices);
            //    int br = this.BottomRight.AssignVertex(vertices);
            //    Lib.AddSquare(ret, br, bl, tl, tr, inverted);
            //}

            //return ret;


            // 0000
            if (state == 0)
            {
            }
            // 0001
            else if (state == 1)
            {
                int br = this.BottomRight.AssignVertex(vertices);
                int b = this.Bottom.AssignVertex(vertices);
                int r = this.Right.AssignVertex(vertices);
                Lib.AddTriangle(ret, r, br, b, inverted);
            }
            // 0010
            else if (state == 2)
            {
                int bl = this.BottomLeft.AssignVertex(vertices);
                int b = this.Bottom.AssignVertex(vertices);
                int l = this.Left.AssignVertex(vertices);
                Lib.AddTriangle(ret, b, bl, l, inverted);
            }
            // 0011
            else if (state == 3)
            {
                int bl = this.BottomLeft.AssignVertex(vertices);
                int br = this.BottomRight.AssignVertex(vertices);
                int l = this.Left.AssignVertex(vertices);
                int r = this.Right.AssignVertex(vertices);
                Lib.AddSquare(ret, br, bl, l, r, inverted);
            }
            // 0100
            else if (state == 4)
            {
                int tr = this.TopRight.AssignVertex(vertices);
                int t = this.Top.AssignVertex(vertices);
                int r = this.Right.AssignVertex(vertices);
                Lib.AddTriangle(ret, t, tr, r, inverted);
            }
            // 0101
            else if (state == 5)
            {
                int tr = this.TopRight.AssignVertex(vertices);
                int br = this.BottomRight.AssignVertex(vertices);
                int t = this.Top.AssignVertex(vertices);
                int b = this.Bottom.AssignVertex(vertices);
                Lib.AddSquare(ret, br, b, t, tr, inverted);
            }
            // 0110 (ignored and filled as square)
            else if (state == 6)
            {
                int tl = this.TopLeft.AssignVertex(vertices);
                int tr = this.TopRight.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                int br = this.BottomRight.AssignVertex(vertices);
                Lib.AddSquare(ret, br, bl, tl, tr, inverted);
            }
            // 0111
            else if (state == 7)
            {
                int tr = this.TopRight.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                int br = this.BottomRight.AssignVertex(vertices);
                int t = this.Top.AssignVertex(vertices);
                int l = this.Left.AssignVertex(vertices);
                Lib.AddPentagon(ret, bl, l, t, tr, br, inverted);
            }
            // 1000
            else if (state == 8)
            {
                int tl = this.TopLeft.AssignVertex(vertices);
                int t = this.Top.AssignVertex(vertices);
                int l = this.Left.AssignVertex(vertices);
                Lib.AddTriangle(ret, l, tl, t, inverted);
            }
            // 1001 (ignored and filled as square)
            else if (state == 9)
            {
                int tl = this.TopLeft.AssignVertex(vertices);
                int tr = this.TopRight.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                int br = this.BottomRight.AssignVertex(vertices);
                Lib.AddSquare(ret, br, bl, tl, tr, inverted);
            }
            // 1010
            else if (state == 10)
            {
                int tl = this.TopLeft.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                int t = this.Top.AssignVertex(vertices);
                int b = this.Bottom.AssignVertex(vertices);
                Lib.AddSquare(ret, b, bl, tl, t, inverted);
            }
            // 1011
            else if (state == 11)
            {
                int tl = this.TopLeft.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                int br = this.BottomRight.AssignVertex(vertices);
                int t = this.Top.AssignVertex(vertices);
                int r = this.Right.AssignVertex(vertices);
                Lib.AddPentagon(ret, tl, t, r, br, bl, inverted);
            }
            // 1100
            else if (state == 12)
            {
                int tl = this.TopLeft.AssignVertex(vertices);
                int tr = this.TopRight.AssignVertex(vertices);
                int l = this.Left.AssignVertex(vertices);
                int r = this.Right.AssignVertex(vertices);
                Lib.AddSquare(ret, r, l, tl, tr, inverted);
            }
            // 1101
            else if (state == 13)
            {
                int tl = this.TopLeft.AssignVertex(vertices);
                int tr = this.TopRight.AssignVertex(vertices);
                int br = this.BottomRight.AssignVertex(vertices);
                int b = this.Bottom.AssignVertex(vertices);
                int l = this.Left.AssignVertex(vertices);
                Lib.AddPentagon(ret, br, b, l, tl, tr, inverted);
            }
            // 1110
            else if (state == 14)
            {
                int tl = this.TopLeft.AssignVertex(vertices);
                int tr = this.TopRight.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                int b = this.Bottom.AssignVertex(vertices);
                int r = this.Right.AssignVertex(vertices);
                Lib.AddPentagon(ret, tr, r, b, bl, tl, inverted);
            }
            // 1111
            else if (state == 15)
            {
                int tl = this.TopLeft.AssignVertex(vertices);
                int tr = this.TopRight.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                int br = this.BottomRight.AssignVertex(vertices);
                Lib.AddSquare(ret, br, bl, tl, tr, inverted);
            }

            return ret;
        }

        public List<int> GetWallPoints(Lib.Direction direction)
        {
            List<int> ret = new List<int>();
            if (this.reserved)
                return ret;

            // 0000 -> tl, tr, bl, br
            int state = 0;            
            state = this.TopLeft.Filled ? state + 8 : state;
            state = this.TopRight.Filled ? state + 4 : state;
            state = this.BottomLeft.Filled ? state + 2 : state;
            state = this.BottomRight.Filled ? state + 1 : state;

            //if (state != 0)
            //{
            //    int tl = this.TopLeft.VertexIndex;
            //    int tr = this.TopRight.VertexIndex;
            //    int bl = this.BottomLeft.VertexIndex;
            //    int br = this.BottomRight.VertexIndex;
            //    if (direction == Lib.Direction.Up)
            //    {
            //        ret.Add(tl);
            //        ret.Add(tr);
            //    }
            //    else if (direction == Lib.Direction.Down)
            //    {
            //        ret.Add(br);
            //        ret.Add(bl);
            //    }
            //    else if (direction == Lib.Direction.Left)
            //    {
            //        ret.Add(bl);
            //        ret.Add(tl);
            //    }
            //    else if (direction == Lib.Direction.Right)
            //    {
            //        ret.Add(tr);
            //        ret.Add(br);
            //    }
            //}

            //return ret;


            // 0000
            if (state == 0)
            {
            }
            // 0001
            else if (state == 1)
            {
                int br = this.BottomRight.VertexIndex;
                int b = this.Bottom.VertexIndex;
                int r = this.Right.VertexIndex;
                ret.Add(b);
                ret.Add(r);
            }
            // 0010
            else if (state == 2)
            {
                int bl = this.BottomLeft.VertexIndex;
                int b = this.Bottom.VertexIndex;
                int l = this.Left.VertexIndex;
                ret.Add(l);
                ret.Add(b);
            }
            // 0011
            else if (state == 3)
            {
                int bl = this.BottomLeft.VertexIndex;
                int br = this.BottomRight.VertexIndex;
                int l = this.Left.VertexIndex;
                int r = this.Right.VertexIndex;
                ret.Add(l);
                ret.Add(r);
            }
            // 0100
            else if (state == 4)
            {
                int tr = this.TopRight.VertexIndex;
                int t = this.Top.VertexIndex;
                int r = this.Right.VertexIndex;
                ret.Add(r);
                ret.Add(t);
            }
            // 0101
            else if (state == 5)
            {
                int tr = this.TopRight.VertexIndex;
                int br = this.BottomRight.VertexIndex;
                int t = this.Top.VertexIndex;
                int b = this.Bottom.VertexIndex;
                ret.Add(b);
                ret.Add(t);
            }
            // 0110 (ignored and filled as square)
            else if (state == 6)
            {
                int tl = this.TopLeft.VertexIndex;
                int tr = this.TopRight.VertexIndex;
                int bl = this.BottomLeft.VertexIndex;
                int br = this.BottomRight.VertexIndex;
                if (direction == Lib.Direction.Up)
                {
                    ret.Add(tl);
                    ret.Add(tr);
                }
                else if (direction == Lib.Direction.Down)
                {
                    ret.Add(br);
                    ret.Add(bl);
                }
                else if (direction == Lib.Direction.Left)
                {
                    ret.Add(bl);
                    ret.Add(tl);
                }
                else if (direction == Lib.Direction.Right)
                {
                    ret.Add(tr);
                    ret.Add(br);
                }
            }
            // 0111
            else if (state == 7)
            {
                int t = this.Top.VertexIndex;
                if (direction == Lib.Direction.Up)
                {
                    int tr = this.TopRight.VertexIndex;
                    ret.Add(tr);
                    ret.Add(t);
                }

                int l = this.Left.VertexIndex;
                ret.Add(l);
                ret.Add(t);
            }
            // 1000
            else if (state == 8)
            {
                int t = this.Top.VertexIndex;
                int l = this.Left.VertexIndex;
                ret.Add(t);
                ret.Add(l);
            }
            // 1001 (ignored and filled as square)
            else if (state == 9)
            {
                int tl = this.TopLeft.VertexIndex;
                int tr = this.TopRight.VertexIndex;
                int bl = this.BottomLeft.VertexIndex;
                int br = this.BottomRight.VertexIndex;
                if (direction == Lib.Direction.Up)
                {
                    ret.Add(tl);
                    ret.Add(tr);
                }
                else if (direction == Lib.Direction.Down)
                {
                    ret.Add(br);
                    ret.Add(bl);
                }
                else if (direction == Lib.Direction.Left)
                {
                    ret.Add(bl);
                    ret.Add(tl);
                }
                else if (direction == Lib.Direction.Right)
                {
                    ret.Add(tr);
                    ret.Add(br);
                }
            }
            // 1010
            else if (state == 10)
            {
                int tl = this.TopLeft.VertexIndex;
                int bl = this.BottomLeft.VertexIndex;
                int t = this.Top.VertexIndex;
                int b = this.Bottom.VertexIndex;
                ret.Add(t);
                ret.Add(b);
            }
            // 1011
            else if (state == 11)
            {
                int t = this.Top.VertexIndex;
                if (direction == Lib.Direction.Up)
                {
                    int tl = this.TopLeft.VertexIndex;
                    ret.Add(tl);
                    ret.Add(t);
                }

                int r = this.Right.VertexIndex;
                ret.Add(t);
                ret.Add(r);
            }
            // 1100
            else if (state == 12)
            {
                int l = this.Left.VertexIndex;
                int r = this.Right.VertexIndex;
                ret.Add(r);
                ret.Add(l);
            }
            // 1101
            else if (state == 13)
            {
                int b = this.Bottom.VertexIndex;
                if (direction == Lib.Direction.Down)
                {
                    int br = this.BottomRight.VertexIndex;
                    ret.Add(br);
                    ret.Add(b);
                }

                int l = this.Left.VertexIndex;
                ret.Add(b);
                ret.Add(l);
            }
            // 1110
            else if (state == 14)
            {
                int b = this.Bottom.VertexIndex;
                if (direction == Lib.Direction.Down)
                {
                    int bl = this.BottomLeft.VertexIndex;
                    ret.Add(bl);
                    ret.Add(b);
                }

                int r = this.Right.VertexIndex;
                ret.Add(r);
                ret.Add(b);
            }
            // 1111
            else if (state == 15)
            {
                int tl = this.TopLeft.VertexIndex;
                int tr = this.TopRight.VertexIndex;
                int bl = this.BottomLeft.VertexIndex;
                int br = this.BottomRight.VertexIndex;
                if (direction == Lib.Direction.Up)
                {
                    ret.Add(tl);
                    ret.Add(tr);
                }
                else if (direction == Lib.Direction.Down)
                {
                    ret.Add(br);
                    ret.Add(bl);
                }
                else if (direction == Lib.Direction.Left)
                {
                    ret.Add(bl);
                    ret.Add(tl);
                }
                else if (direction == Lib.Direction.Right)
                {
                    ret.Add(tr);
                    ret.Add(br);
                }
            }

            return ret;
        }

        public void CopyIntersections(Square s, float z)
        {
            s.TopLeft.SetPosition(new Vector3(this.TopLeft.Position.x, this.TopLeft.Position.y, this.TopLeft.Position.z + z));
            s.TopRight.SetPosition(new Vector3(this.TopRight.Position.x, this.TopRight.Position.y, this.TopRight.Position.z + z));
            s.BottomLeft.SetPosition(new Vector3(this.BottomLeft.Position.x, this.BottomLeft.Position.y, this.BottomLeft.Position.z + z));
            s.BottomRight.SetPosition(new Vector3(this.BottomRight.Position.x, this.BottomRight.Position.y, this.BottomRight.Position.z + z));
            s.Top.SetPosition(new Vector3(this.Top.Position.x, this.Top.Position.y, this.Top.Position.z + z));
            s.Left.SetPosition(new Vector3(this.Left.Position.x, this.Left.Position.y, this.Left.Position.z + z));
            s.Bottom.SetPosition(new Vector3(this.Bottom.Position.x, this.Bottom.Position.y, this.Bottom.Position.z + z));
            s.Right.SetPosition(new Vector3(this.Right.Position.x, this.Right.Position.y, this.Right.Position.z + z));

            s.TopLeft.Filled = this.TopLeft.Filled;
            s.TopRight.Filled = this.TopRight.Filled;
            s.BottomLeft.Filled = this.BottomLeft.Filled;
            s.BottomRight.Filled = this.BottomRight.Filled;
            s.Top.Filled = this.Top.Filled;
            s.Left.Filled = this.Left.Filled;
            s.Bottom.Filled = this.Bottom.Filled;
            s.Right.Filled = this.Right.Filled;

            s.filled = this.filled;
            s.marked = this.marked;
            s.reserved = this.reserved;
        }
        
        public void SetZHeight(Vector3 startHeight, Vector3 endHeight, List<Vector3> vertices)
        {
            TopLeft.SetZ(Vector3.Lerp(startHeight, endHeight, Lib.PercentBetween(startHeight, endHeight, TopLeft.Position)).z, vertices);
            TopRight.SetZ(Vector3.Lerp(startHeight, endHeight, Lib.PercentBetween(startHeight, endHeight, TopRight.Position)).z, vertices);
            BottomLeft.SetZ(Vector3.Lerp(startHeight, endHeight, Lib.PercentBetween(startHeight, endHeight, BottomLeft.Position)).z, vertices);
            BottomRight.SetZ(Vector3.Lerp(startHeight, endHeight, Lib.PercentBetween(startHeight, endHeight, BottomRight.Position)).z, vertices);
            Top.SetZ(Vector3.Lerp(startHeight, endHeight, Lib.PercentBetween(startHeight, endHeight, Top.Position)).z, vertices);
            Left.SetZ(Vector3.Lerp(startHeight, endHeight, Lib.PercentBetween(startHeight, endHeight, Left.Position)).z, vertices);
            Right.SetZ(Vector3.Lerp(startHeight, endHeight, Lib.PercentBetween(startHeight, endHeight, Right.Position)).z, vertices);
            Bottom.SetZ(Vector3.Lerp(startHeight, endHeight, Lib.PercentBetween(startHeight, endHeight, Bottom.Position)).z, vertices);
        }
    }
}
