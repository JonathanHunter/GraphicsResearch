namespace GraphicsResearch.Generation.Paths
{
    using UnityEngine;
    using UnityEditor;

    public class LayerPath : Path
    {
        public float Gradient { get; set; }

        public bool IsIntersectingLine(Vector3 begin, Vector3 end)
        {
            Vector3 tl, tr, bl, br;
            GenerationUtility.BoxBounds(this.Start.Position, this.End.Position, this.Width, out tl, out tr, out bl, out br);

            if (GenerationUtility.PointInBox(begin, tl, tr, bl, br) || GenerationUtility.PointInBox(end, tl, tr, bl, br))
                return true;

            return GenerationUtility.CheckBoxLineIntersection(begin, end, tl, tr, bl, br);
        }

        public bool IsIntersectingCircle(Vector3 center, float radius)
        {
            Vector3 tl, tr, bl, br;
            GenerationUtility.BoxBounds(this.Start.Position, this.End.Position, this.Width, out tl, out tr, out bl, out br);
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

        public override void Draw(float widthScale)
        {
            Vector3 start = this.Start.WorldPosition;
            Vector3 end = this.End.WorldPosition;
            Vector3 es = Vector3.Normalize(start - end);
            Vector3 left = new Vector3(-es.y, es.x, es.z);
            Gizmos.DrawLine(start + left * this.Width * widthScale / 2f, end + left * this.Width * widthScale / 2f);
            Gizmos.DrawLine(start - left * this.Width * widthScale / 2f, end - left * this.Width * widthScale / 2f);
            Handles.Label(Vector3.Lerp(start, end, .5f), "Gradient: " + this.Gradient + "\nDistance: " + this.Distance);
        }
    }
}
