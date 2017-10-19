namespace GraphicsResearch.PathPlacement
{
    using UnityEngine;
    using RoomPlacement;

    public class Edge
    {
        public Vector3 Start { get; private set; }

        public Vector3 End { get; private set; }

        public Room StartRoom { get; private set; }

        public Room EndRoom { get; private set; }

        public float Weight { get; private set; }

        public float Width { get; private set; }

        public Edge(Vector3 start, Vector3 end, Room startRoom, Room endRoom, float weight, float width)
        {
            this.Start = start;
            this.End = end;
            this.StartRoom = startRoom;
            this.EndRoom = endRoom;
            this.Weight = weight;
            this.Width = width;
        }

        public void Draw()
        {
            Vector3 start = this.Start;
            Vector3 end = this.End;
            Vector3 es = Vector3.Normalize(start - end);
            Vector3 left = new Vector3(-es.y, es.x, es.z);
            Gizmos.DrawLine(start + left * this.Width / 2f, end + left * this.Width / 2f);
            Gizmos.DrawLine(start - left * this.Width / 2f, end - left * this.Width / 2f);
            if(this.EndRoom == null)
                Gizmos.DrawWireSphere(this.End, this.Width / 2f);
        }
    }
}
