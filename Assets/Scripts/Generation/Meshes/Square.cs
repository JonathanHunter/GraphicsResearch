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
        public bool Marked { get; private set; }
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
    }
}
