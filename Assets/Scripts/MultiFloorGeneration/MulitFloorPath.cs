namespace GraphicsResearch.MultiFloorGeneration
{
    using UnityEngine;
    using RoomPlacement;

    public class MulitFloorPath
    {
        public Room Room1 { get; private set; }
        public Room Room2 { get; private set; }
        public float Gradient { get; private set; }
        public float Distance { get; private set; }
        public float Width { get { return this.width; } }

        private float width;

        public MulitFloorPath(Room r1, Room r2, float gradient, float distance)
        {
            this.Room1 = r1;
            this.Room2 = r2;
            this.Gradient = gradient;
            this.Distance = distance;
            this.width = 0.5f;
        }

        public void Draw()
        {
            Vector3 start = this.Room1.transform.position;
            Vector3 end = this.Room2.transform.position;
            Vector3 es = Vector3.Normalize(start - end);
            Vector3 left = new Vector3(-es.y, es.x, es.z);
            Gizmos.DrawLine(start + left * this.width / 2f, end + left * this.width / 2f);
            Gizmos.DrawLine(start - left * this.width / 2f, end - left * this.width / 2f);
        }
    }
}
