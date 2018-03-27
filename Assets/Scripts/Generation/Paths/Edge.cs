namespace GraphicsResearch.Generation.Paths
{
    using UnityEngine;

    public class Edge
    {
        public Vector3 Start { get; set; }
        public Vector3 End { get; set; }
        public float Distance { get; set; }
        public float Width { get; set; }
        public bool TailEdge { get; set; }
        
        public void Draw(float widthScale)
        {
            Vector3 start = this.Start;
            Vector3 end = this.End;
            Vector3 es = Vector3.Normalize(start - end);
            Vector3 left = new Vector3(-es.y, es.x, es.z);
            Gizmos.DrawLine(start + left * this.Width * widthScale / 2f, end + left * this.Width * widthScale / 2f);
            Gizmos.DrawLine(start - left * this.Width * widthScale / 2f, end - left * this.Width * widthScale / 2f);
            if (!this.TailEdge)
                Gizmos.DrawWireSphere(this.End, this.Width * widthScale / 2f);
        }
    }
}
