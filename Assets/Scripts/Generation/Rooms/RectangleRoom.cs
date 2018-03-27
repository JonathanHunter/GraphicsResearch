namespace GraphicsResearch.Generation.Rooms
{
    using UnityEngine;

    public class RectangleRoom : Room
    {
        [SerializeField]
        private Vector2 dimentions;

        public Vector2 Dimentions { get { return this.dimentions; } internal set { this.dimentions = value; } }

        public bool draw;

        private void OnDrawGizmos()
        {
            if(draw)
            {
                Vector3 s = this.WorldPosition + this.transform.up * this.Dimentions.y / 2f;
                Vector3 e = this.WorldPosition - this.transform.up * this.Dimentions.y / 2f;
                Gizmos.DrawLine(s, e);
                //Vector3 s, e;
                //float width;
                //s = this.WorldPosition + this.transform.TransformDirection(this.transform.forward) * this.Dimentions.y / 2f;
                //e = this.WorldPosition - this.transform.TransformDirection(this.transform.forward) * this.Dimentions.y / 2f;
                //width = this.Dimentions.x;
                //Vector3 es = Vector3.Normalize(s - e);
                //Vector3 left = new Vector3(-es.y, es.x, es.z);
                //Vector3 tl = s + left * width / 2f;
                //Vector3 tr = s - left * width / 2f;
                //Vector3 bl = e + left * width / 2f;
                //Vector3 br = e - left * width / 2f;

                //Gizmos.DrawSphere(tl, .1f);
                //Gizmos.DrawSphere(tr, .1f);
                //Gizmos.DrawSphere(bl, .1f);
                //Gizmos.DrawSphere(br, .1f);
            }
        }
    }
}
