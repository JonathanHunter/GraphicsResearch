namespace GraphicsResearch.Generation.Rooms
{
    using UnityEngine;

    public abstract class Room : MonoBehaviour
    {
        public Vector3 Position { get { return this.transform.localPosition; } }

        public Vector3 WorldPosition { get { return this.transform.position; } }

        public Vector3 OriginalPosition { get; set; }

        public abstract bool IsIntersectingLine(Vector3 begin, Vector3 end);
        public abstract bool IsIntersectingCircle(Vector3 center, float radius);
        public abstract bool IsIntersectingBox(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br);
    }
}
