namespace GraphicsResearch.Generation.Meshes
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Square
    {
        /// <summary> The 3D position of the center of this square. </summary>
        public Vector3 Center { get; private set; }
        /// <summary> The dimensions of this square. </summary>
        public Vector2 Size { get; private set; }
        /// <summary> True if this square has something in it. </summary>
        public bool Filled { get { return this.filled || this.Reserved; } set { this.filled = value; } }
        /// <summary> True if this square was marked. </summary>
        public bool Marked { get; set; }
        /// <summary> True if this square was reserved. </summary>
        public bool Reserved { get; set; }

        public Corner TopLeft { get; set; }
        public Corner TopRight { get; set; }
        public Corner BottomLeft { get; set; }
        public Corner BottomRight { get; set; }
        public Corner Top { get; set; }
        public Corner Bottom { get; set; }
        public Corner Left { get; set; }
        public Corner Right { get; set; }

        private bool filled;

        public Square(Vector3 position, Vector2 dimensions)
        {
            this.Center = position;
            this.Size = dimensions;
            this.Filled = false;
            this.Marked = false;
            this.Reserved = false;
        }

        public void FillCircle(Vector3 center, float radius)
        {
            if (this.Reserved)
                return; // don't fill reserved squares

            bool topLeft = GenerationUtility.PointInCircle(this.TopLeft.Position, center, radius);
            bool topRight = GenerationUtility.PointInCircle(this.TopRight.Position, center, radius);
            bool bottomLeft = GenerationUtility.PointInCircle(this.BottomLeft.Position, center, radius);
            bool bottomRight = GenerationUtility.PointInCircle(this.BottomRight.Position, center, radius);

            if (topLeft)
                this.TopLeft.Filled = true;
            if (topRight)
                this.TopRight.Filled = true;
            if (bottomLeft)
                this.BottomLeft.Filled = true;
            if (bottomRight)
                this.BottomRight.Filled = true;

            if (topLeft || topRight || bottomLeft || bottomRight)
                this.Filled = true;

            if (!this.Filled)
                return; // square is empty so work is done

            if (GenerationUtility.CheckCircleLineIntersection(this.TopLeft.Position, this.TopRight.Position, center, radius))
            {
                this.Top.Filled = true;
                this.Top.SetPosition(
                    GenerationUtility.CircleLineIntersection(this.TopLeft.Position, this.TopRight.Position, center, radius));
            }

            if (GenerationUtility.CheckCircleLineIntersection(this.BottomLeft.Position, this.BottomRight.Position, center, radius))
            {
                this.Bottom.Filled = true;
                this.Bottom.SetPosition(
                    GenerationUtility.CircleLineIntersection(this.BottomLeft.Position, this.BottomRight.Position, center, radius));
            }

            if (GenerationUtility.CheckCircleLineIntersection(this.TopLeft.Position, this.BottomLeft.Position, center, radius))
            {
                this.Left.Filled = true;
                this.Left.SetPosition(
                    GenerationUtility.CircleLineIntersection(this.TopLeft.Position, this.BottomLeft.Position, center, radius));
            }

            if (GenerationUtility.CheckCircleLineIntersection(this.TopRight.Position, this.BottomRight.Position, center, radius))
            {
                this.Right.Filled = true;
                this.Right.SetPosition(
                    GenerationUtility.CircleLineIntersection(this.TopRight.Position, this.BottomRight.Position, center, radius));
            }
        }

        public void FillBox(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br)
        {
            if (this.Reserved)
                return;
            
            bool topLeft = GenerationUtility.PointInBox(this.TopLeft.Position, tl, tr, bl, br);
            bool topRight = GenerationUtility.PointInBox(this.TopRight.Position, tl, tr, bl, br);
            bool bottomLeft = GenerationUtility.PointInBox(this.BottomLeft.Position, tl, tr, bl, br);
            bool bottomRight = GenerationUtility.PointInBox(this.BottomRight.Position, tl, tr, bl, br);

            if (topLeft)
                this.TopLeft.Filled = true;
            if (topRight)
                this.TopRight.Filled = true;
            if (bottomLeft)
                this.BottomLeft.Filled = true;
            if (bottomRight)
                this.BottomRight.Filled = true;

            if (topLeft || topRight || bottomLeft || bottomRight)
                this.Filled = true;

            if (!this.Filled)
                return; // square is empty so work is done

            if (GenerationUtility.CheckBoxLineIntersection(this.TopLeft.Position, this.TopRight.Position, tl, tr, bl, br))
            {
                this.Top.Filled = true;
                this.Top.SetPosition(
                    GenerationUtility.BoxLineIntersection(this.TopLeft.Position, this.TopRight.Position, tl, tr, bl, br));
            }

            if (GenerationUtility.CheckBoxLineIntersection(this.BottomLeft.Position, this.BottomRight.Position, tl, tr, bl, br))
            {
                this.Bottom.Filled = true;
                this.Bottom.SetPosition(
                    GenerationUtility.BoxLineIntersection(this.BottomLeft.Position, this.BottomRight.Position, tl, tr, bl, br));
            }

            if (GenerationUtility.CheckBoxLineIntersection(this.TopLeft.Position, this.BottomLeft.Position, tl, tr, bl, br))
            {
                this.Left.Filled = true;
                this.Left.SetPosition(
                    GenerationUtility.BoxLineIntersection(this.TopLeft.Position, this.BottomLeft.Position, tl, tr, bl, br));
            }

            if (GenerationUtility.CheckBoxLineIntersection(this.TopRight.Position, this.BottomRight.Position, tl, tr, bl, br))
            {
                this.Right.Filled = true;
                this.Right.SetPosition(
                    GenerationUtility.BoxLineIntersection(this.TopRight.Position, this.BottomRight.Position, tl, tr, bl, br));
            }
        }
        
        public void SetZHeight(Vector3 startHeight, Vector3 endHeight)
        {
            TopLeft.SetZ(Vector3.Lerp(startHeight, endHeight, GenerationUtility.PercentBetween(startHeight, endHeight, TopLeft.Position)).z);
            TopRight.SetZ(Vector3.Lerp(startHeight, endHeight, GenerationUtility.PercentBetween(startHeight, endHeight, TopRight.Position)).z);
            BottomLeft.SetZ(Vector3.Lerp(startHeight, endHeight, GenerationUtility.PercentBetween(startHeight, endHeight, BottomLeft.Position)).z);
            BottomRight.SetZ(Vector3.Lerp(startHeight, endHeight, GenerationUtility.PercentBetween(startHeight, endHeight, BottomRight.Position)).z);
            Top.SetZ(Vector3.Lerp(startHeight, endHeight, GenerationUtility.PercentBetween(startHeight, endHeight, Top.Position)).z);
            Left.SetZ(Vector3.Lerp(startHeight, endHeight, GenerationUtility.PercentBetween(startHeight, endHeight, Left.Position)).z);
            Right.SetZ(Vector3.Lerp(startHeight, endHeight, GenerationUtility.PercentBetween(startHeight, endHeight, Right.Position)).z);
            Bottom.SetZ(Vector3.Lerp(startHeight, endHeight, GenerationUtility.PercentBetween(startHeight, endHeight, Bottom.Position)).z);
        }

        public List<int> GetTriangles(List<Vector3> vertices, bool inverted)
        {
            List<int> ret = new List<int>();
            if (this.Reserved)
                return ret;

            // 0000 -> tl, tr, bl, br
            int state = 0;
            state = this.TopLeft.Filled ? state + 8 : state;
            state = this.TopRight.Filled ? state + 4 : state;
            state = this.BottomLeft.Filled ? state + 2 : state;
            state = this.BottomRight.Filled ? state + 1 : state;

            #region Marching Squares states
            // 0000
            if (state == 0)
            {
            }
            // 0001
            else if (state == 1)
            {
                int r = this.Right.AssignVertex(vertices);
                int br = this.BottomRight.AssignVertex(vertices);
                int b = this.Bottom.AssignVertex(vertices);
                MeshUtility.AddTriangle(ret, r, br, b, inverted);
            }
            // 0010
            else if (state == 2)
            {
                int b = this.Bottom.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                int l = this.Left.AssignVertex(vertices);
                MeshUtility.AddTriangle(ret, b, bl, l, inverted);
            }
            // 0011
            else if (state == 3)
            {
                int br = this.BottomRight.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                int l = this.Left.AssignVertex(vertices);
                int r = this.Right.AssignVertex(vertices);
                MeshUtility.AddSquare(ret, br, bl, l, r, inverted);
            }
            // 0100
            else if (state == 4)
            {
                int t = this.Top.AssignVertex(vertices);
                int tr = this.TopRight.AssignVertex(vertices);
                int r = this.Right.AssignVertex(vertices);
                MeshUtility.AddTriangle(ret, t, tr, r, inverted);
            }
            // 0101
            else if (state == 5)
            {
                int br = this.BottomRight.AssignVertex(vertices);
                int b = this.Bottom.AssignVertex(vertices);
                int t = this.Top.AssignVertex(vertices);
                int tr = this.TopRight.AssignVertex(vertices);
                MeshUtility.AddSquare(ret, br, b, t, tr, inverted);
            }
            // 0110
            else if (state == 6)
            {
                int t = this.Top.AssignVertex(vertices);
                int tr = this.TopRight.AssignVertex(vertices);
                int r = this.Right.AssignVertex(vertices);
                MeshUtility.AddTriangle(ret, t, tr, r, inverted);

                int b = this.Bottom.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                int l = this.Left.AssignVertex(vertices);
                MeshUtility.AddTriangle(ret, b, bl, l, inverted);
            }
            // 0111
            else if (state == 7)
            {
                int br = this.BottomRight.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                int l = this.Left.AssignVertex(vertices);
                int t = this.Top.AssignVertex(vertices);
                int tr = this.TopRight.AssignVertex(vertices);
                MeshUtility.AddPentagon(ret, br, bl, l, t, tr, inverted);
            }
            // 1000
            else if (state == 8)
            {
                int l = this.Left.AssignVertex(vertices);
                int tl = this.TopLeft.AssignVertex(vertices);
                int t = this.Top.AssignVertex(vertices);
                MeshUtility.AddTriangle(ret, l, tl, t, inverted);
            }
            // 1001
            else if (state == 9)
            {
                int l = this.Left.AssignVertex(vertices);
                int tl = this.TopLeft.AssignVertex(vertices);
                int t = this.Top.AssignVertex(vertices);
                MeshUtility.AddTriangle(ret, l, tl, t, inverted);

                int r = this.Left.AssignVertex(vertices);
                int br = this.TopLeft.AssignVertex(vertices);
                int b = this.Top.AssignVertex(vertices);
                MeshUtility.AddTriangle(ret, r, br, b, inverted);
            }
            // 1010
            else if (state == 10)
            {
                int b = this.Bottom.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                int tl = this.TopLeft.AssignVertex(vertices);
                int t = this.Top.AssignVertex(vertices);
                MeshUtility.AddSquare(ret, b, bl, tl, t, inverted);
            }
            // 1011
            else if (state == 11)
            {
                int bl = this.BottomLeft.AssignVertex(vertices);
                int tl = this.TopLeft.AssignVertex(vertices);
                int t = this.Top.AssignVertex(vertices);
                int r = this.Right.AssignVertex(vertices);
                int br = this.BottomRight.AssignVertex(vertices);
                MeshUtility.AddPentagon(ret, bl, tl, t, r, br, inverted);
            }
            // 1100
            else if (state == 12)
            {
                int r = this.Right.AssignVertex(vertices);
                int l = this.Left.AssignVertex(vertices);
                int tl = this.TopLeft.AssignVertex(vertices);
                int tr = this.TopRight.AssignVertex(vertices);
                MeshUtility.AddSquare(ret, r, l, tl, tr, inverted);
            }
            // 1101
            else if (state == 13)
            {
                int tr = this.TopRight.AssignVertex(vertices);
                int br = this.BottomRight.AssignVertex(vertices);
                int b = this.Bottom.AssignVertex(vertices);
                int l = this.Left.AssignVertex(vertices);
                int tl = this.TopLeft.AssignVertex(vertices);
                MeshUtility.AddPentagon(ret, tr, br, b, l, tl, inverted);
            }
            // 1110
            else if (state == 14)
            {
                int tl = this.TopLeft.AssignVertex(vertices);
                int tr = this.TopRight.AssignVertex(vertices);
                int r = this.Right.AssignVertex(vertices);
                int b = this.Bottom.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                MeshUtility.AddPentagon(ret, tl, tr, r, b, bl, inverted);
            }
            // 1111
            else if (state == 15)
            {
                int br = this.BottomRight.AssignVertex(vertices);
                int bl = this.BottomLeft.AssignVertex(vertices);
                int tl = this.TopLeft.AssignVertex(vertices);
                int tr = this.TopRight.AssignVertex(vertices);
                MeshUtility.AddSquare(ret, br, bl, tl, tr, inverted);
            }
            #endregion

            return ret;
        }

        public List<int> GetWallPoints(bool[,] neighbors)
        {
            List<int> ret = new List<int>();
            if (this.Reserved)
                return ret;

            // 0000 -> tl, tr, bl, br
            int state = 0;
            state = this.TopLeft.Filled ? state + 8 : state;
            state = this.TopRight.Filled ? state + 4 : state;
            state = this.BottomLeft.Filled ? state + 2 : state;
            state = this.BottomRight.Filled ? state + 1 : state;

            // 0000
            if (state == 0)
            {
            }
            // 0001
            else if (state == 1)
            {
                int b = this.Bottom.VertexIndex;
                int r = this.Right.VertexIndex;
                int br = this.BottomRight.VertexIndex;
                ret.Add(b);
                ret.Add(r);

                if(!neighbors[2,1])
                {
                    ret.Add(r);
                    ret.Add(br);
                }

                if(!neighbors[1,2])
                {
                    ret.Add(br);
                    ret.Add(b);
                }
            }
            // 0010
            else if (state == 2)
            {
                int l = this.Left.VertexIndex;
                int b = this.Bottom.VertexIndex;
                int bl = this.BottomLeft.VertexIndex;
                ret.Add(l);
                ret.Add(b);

                if(!neighbors[0,1])
                {
                    ret.Add(bl);
                    ret.Add(l);
                }

                if(!neighbors[1,2])
                {
                    ret.Add(b);
                    ret.Add(bl);
                }
            }
            // 0011
            else if (state == 3)
            {
                int l = this.Left.VertexIndex;
                int r = this.Right.VertexIndex;
                int br = this.BottomRight.VertexIndex;
                int bl = this.BottomLeft.VertexIndex;
                ret.Add(l);
                ret.Add(r);
                
                if(!neighbors[0,1])
                {
                    ret.Add(bl);
                    ret.Add(l);
                }

                if(!neighbors[1,2])
                {
                    ret.Add(br);
                    ret.Add(bl);
                }

                if(!neighbors[2,1])
                {
                    ret.Add(r);
                    ret.Add(br);
                }
            }
            // 0100
            else if (state == 4)
            {
                int r = this.Right.VertexIndex;
                int t = this.Top.VertexIndex;
                int tr = this.TopRight.VertexIndex;
                ret.Add(r);
                ret.Add(t);

                if(!neighbors[1,0])
                {
                    ret.Add(t);
                    ret.Add(tr);
                }

                if(!neighbors[2,1])
                {
                    ret.Add(tr);
                    ret.Add(r);
                }
            }
            // 0101
            else if (state == 5)
            {
                int b = this.Bottom.VertexIndex;
                int t = this.Top.VertexIndex;
                int tr = this.TopRight.VertexIndex;
                int br = this.BottomRight.VertexIndex;
                ret.Add(b);
                ret.Add(t);

                if(!neighbors[1,0])
                {
                    ret.Add(t);
                    ret.Add(tr);
                }

                if(!neighbors[2,1])
                {
                    ret.Add(tr);
                    ret.Add(br);
                }

                if(!neighbors[1,2])
                {
                    ret.Add(br);
                    ret.Add(b);
                }
            }
            // 0110 
            else if (state == 6)
            {
                int r = this.Right.VertexIndex;
                int t = this.Top.VertexIndex;
                int l = this.Left.VertexIndex;
                int b = this.Bottom.VertexIndex;
                int tr = this.TopRight.VertexIndex;
                int bl = this.BottomLeft.VertexIndex;
                ret.Add(r);
                ret.Add(t);

                ret.Add(l);
                ret.Add(b);
                if(!neighbors[1,0])
                {
                    ret.Add(t);
                    ret.Add(tr);
                }

                if(!neighbors[2,1])
                {
                    ret.Add(tr);
                    ret.Add(r);
                }

                if(!neighbors[1,2])
                {
                    ret.Add(b);
                    ret.Add(bl);
                }

                if(!neighbors[0,1])
                {
                    ret.Add(bl);
                    ret.Add(l);
                }
            }
            // 0111
            else if (state == 7)
            {
                int l = this.Left.VertexIndex;
                int t = this.Top.VertexIndex;
                int tr = this.TopRight.VertexIndex;
                int br = this.BottomRight.VertexIndex;
                int bl = this.BottomLeft.VertexIndex;
                ret.Add(l);
                ret.Add(t);

                if(!neighbors[1,0])
                {
                    ret.Add(t);
                    ret.Add(tr);
                }

                if(!neighbors[2,1])
                {
                    ret.Add(tr);
                    ret.Add(br);
                }

                if(!neighbors[1,2])
                {
                    ret.Add(br);
                    ret.Add(bl);
                }

                if(!neighbors[0,1])
                {
                    ret.Add(bl);
                    ret.Add(l);
                }
            }
            // 1000
            else if (state == 8)
            {
                int t = this.Top.VertexIndex;
                int l = this.Left.VertexIndex;
                int tl = this.TopLeft.VertexIndex;
                ret.Add(t);
                ret.Add(l);

                if(!neighbors[1,0])
                {
                    ret.Add(tl);
                    ret.Add(t);
                }

                if(!neighbors[0,1])
                {
                    ret.Add(l);
                    ret.Add(tl);
                }
            }
            // 1001
            else if (state == 9)
            {
                int t = this.Top.VertexIndex;
                int l = this.Left.VertexIndex;
                int b = this.Bottom.VertexIndex;
                int r = this.Right.VertexIndex;
                int tl = this.TopLeft.VertexIndex;
                int br = this.BottomRight.VertexIndex;
                ret.Add(t);
                ret.Add(l);

                ret.Add(b);
                ret.Add(r);

                if(!neighbors[1,0])
                {
                    ret.Add(tl);
                    ret.Add(t);
                }

                if(!neighbors[2,1])
                {
                    ret.Add(r);
                    ret.Add(br);
                }

                if(!neighbors[1,2])
                {
                    ret.Add(br);
                    ret.Add(b);
                }

                if(!neighbors[0,1])
                {
                    ret.Add(l);
                    ret.Add(tl);
                }
            }
            // 1010
            else if (state == 10)
            {
                int t = this.Top.VertexIndex;
                int b = this.Bottom.VertexIndex;
                int bl = this.BottomLeft.VertexIndex;
                int tl = this.TopLeft.VertexIndex;
                ret.Add(t);
                ret.Add(b);

                if(!neighbors[1,0])
                {
                    ret.Add(tl);
                    ret.Add(t);
                }

                if(!neighbors[1,2])
                {
                    ret.Add(b);
                    ret.Add(bl);
                }

                if(!neighbors[0,1])
                {
                    ret.Add(bl);
                    ret.Add(tl);
                }
            }
            // 1011
            else if (state == 11)
            {
                int t = this.Top.VertexIndex;
                int r = this.Right.VertexIndex;
                int br = this.BottomRight.VertexIndex;
                int bl = this.BottomLeft.VertexIndex;
                int tl = this.TopLeft.VertexIndex;
                ret.Add(t);
                ret.Add(r);

                if(!neighbors[1,0])
                {
                    ret.Add(tl);
                    ret.Add(t);
                }

                if(!neighbors[2,1])
                {
                    ret.Add(r);
                    ret.Add(br);
                }

                if(!neighbors[1,2])
                {
                    ret.Add(br);
                    ret.Add(bl);
                }

                if(!neighbors[0,1])
                {
                    ret.Add(bl);
                    ret.Add(tl);
                }
            }
            // 1100
            //else if (state == 12)
            //{
            //    int l = this.Left.VertexIndex;
            //    int r = this.Right.VertexIndex;
            //    ret.Add(r);
            //    ret.Add(l);
            //}
            //// 1101
            //else if (state == 13)
            //{
            //    int b = this.Bottom.VertexIndex;
            //    if (direction == Lib.Direction.Down)
            //    {
            //        int br = this.BottomRight.VertexIndex;
            //        ret.Add(br);
            //        ret.Add(b);
            //    }

            //    int l = this.Left.VertexIndex;
            //    ret.Add(b);
            //    ret.Add(l);
            //}
            //// 1110
            //else if (state == 14)
            //{
            //    int b = this.Bottom.VertexIndex;
            //    if (direction == Lib.Direction.Down)
            //    {
            //        int bl = this.BottomLeft.VertexIndex;
            //        ret.Add(bl);
            //        ret.Add(b);
            //    }

            //    int r = this.Right.VertexIndex;
            //    ret.Add(r);
            //    ret.Add(b);
            //}
            //// 1111
            //else if (state == 15)
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

            return ret;
        }
    }
}
