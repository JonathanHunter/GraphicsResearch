namespace GraphicsResearch.PathPlacement
{
    using UnityEngine;
    using RoomPlacement;

    public class Path
    {
        public Vector3 Start { get; private set; }

        public Vector3 End { get; private set; }

        public Room StartRoom { get; private set; }

        public Room EndRoom { get; private set; }

        public float Weight { get; private set; }

        public float Width { get; private set; }

        public Path(Vector3 start, Vector3 end, Room startRoom, Room endRoom, float weight, float width)
        {
            this.Start = start;
            this.End = end;
            this.StartRoom = startRoom;
            this.EndRoom = endRoom;
            this.Weight = weight;
            this.Width = width;
        }
    }
}
