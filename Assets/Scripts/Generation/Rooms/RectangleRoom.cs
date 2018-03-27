namespace GraphicsResearch.Generation.Rooms
{
    using UnityEngine;

    public class RectangleRoom : Room
    {
        [SerializeField]
        private Vector2 dimentions;

        [SerializeField]
        private bool draw;

        public Vector2 Dimentions { get { return this.dimentions; } internal set { this.dimentions = value; } }
        
        public void ImplicitBounds(out Vector3 start, out Vector3 end, out Vector3 left)
        {
            start = this.Position + this.transform.parent.InverseTransformDirection(this.transform.up) * this.Dimentions.y / 2f;
            end = this.Position - this.transform.parent.InverseTransformDirection(this.transform.up) * this.Dimentions.y / 2f;
            Vector3 es = Vector3.Normalize(start - end);
            left = new Vector3(-es.y, es.x, es.z);
        }

        public void BoxBounds(out Vector3 tl, out Vector3 tr, out Vector3 bl, out Vector3 br)
        {
            Vector3 s, e, left;
            ImplicitBounds(out s, out e, out left);
            tl = s + left * this.Dimentions.x / 2f;
            tr = s - left * this.Dimentions.x / 2f;
            bl = e + left * this.Dimentions.x / 2f;
            br = e - left * this.Dimentions.x / 2f;
        }

        public override bool IsIntersectingLine(Vector3 begin, Vector3 end)
        {
            Vector3 tl, tr, bl, br;
            BoxBounds(out tl, out tr, out bl, out br);
            if (GenerationUtility.PointInBox(begin, tl, tr, bl, br) || GenerationUtility.PointInBox(end, tl, tr, bl, br))
                return true;

            return GenerationUtility.CheckBoxLineIntersection(begin, end, tl, tr, bl, br);
        }

        public override bool IsIntersectingCircle(Vector3 center, float radius)
        {
            Vector3 tl, tr, bl, br;
            BoxBounds(out tl, out tr, out bl, out br);
            if (GenerationUtility.PointInCircle(tl, center, radius))
                return true;

            if (GenerationUtility.PointInCircle(tr, center, radius))
                return true;

            if (GenerationUtility.PointInCircle(bl, center, radius))
                return true;

            if (GenerationUtility.PointInCircle(br, center, radius))
                return true;

            if (GenerationUtility.CheckCircleLineIntersection(tl, tr, center, radius))
                return true;

            if (GenerationUtility.CheckCircleLineIntersection(bl, br, center, radius))
                return true;

            if (GenerationUtility.CheckCircleLineIntersection(tl, bl, center, radius))
                return true;

            if (GenerationUtility.CheckCircleLineIntersection(tr, br, center, radius))
                return true;

            return GenerationUtility.PointInBox(center, tl, tr, bl, br);
        }

        public override bool IsIntersectingBox(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br)
        {
            Vector3 ourTl, ourTr, ourBl, ourBr;
            BoxBounds(out ourTl, out ourTr, out ourBl, out ourBr);
            if (GenerationUtility.PointInBox(ourTl, tl, tr, bl , br))
                return true;

            if (GenerationUtility.PointInBox(ourTr, tl, tr, bl, br))
                return true;

            if (GenerationUtility.PointInBox(ourBl, tl, tr, bl, br))
                return true;

            if (GenerationUtility.PointInBox(ourBr, tl, tr, bl, br))
                return true;

            if (GenerationUtility.PointInBox(tl, ourTl, ourTr, ourBl, ourBr))
                return true;

            if (GenerationUtility.PointInBox(tr, ourTl, ourTr, ourBl, ourBr))
                return true;

            if (GenerationUtility.PointInBox(bl, ourTl, ourTr, ourBl, ourBr))
                return true;

            if (GenerationUtility.PointInBox(br, ourTl, ourTr, ourBl, ourBr))
                return true;

            return false;
        }

        private void OnDrawGizmos()
        {
            if(this.draw)
            {
                Vector3 tl, tr, bl, br;
                BoxBounds(out tl, out tr, out bl, out br);
                Gizmos.DrawSphere(tl, .2f);
                Gizmos.DrawSphere(tr, .2f);
                Gizmos.DrawSphere(bl, .2f);
                Gizmos.DrawSphere(br, .2f);
            }
        }
    }
}
