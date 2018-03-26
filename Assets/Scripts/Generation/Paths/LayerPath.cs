namespace GraphicsResearch.Generation.Paths
{
    using UnityEngine;
    using UnityEditor;

    public class LayerPath : Path
    {
        public float Gradient { get; set; }

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
