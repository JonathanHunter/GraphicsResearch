namespace GraphicsResearch.RoomPlacement
{
    using UnityEngine;

    public class CircleRoom : Room
    {
        [SerializeField]
        private float radius;

        public float Radius { get { return this.radius; } internal set { this.radius = value; } }
    }
}
