namespace GraphicsResearch.Generation.Rooms
{
    using UnityEngine;

    public class Room : MonoBehaviour
    {
        public Vector3 Position { get { return this.transform.localPosition; } }

        public Vector3 WorldPosition { get { return this.transform.position; } }

        public Vector3 OriginalPosition { get; set; }
    }
}
