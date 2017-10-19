namespace GraphicsResearch.RoomPlacement
{
    using UnityEngine;

    public class RectangleRoom : Room
    {
        [SerializeField]
        private Vector2 dimentions;

        public Vector2 Dimentions { get { return this.dimentions; } internal set { this.dimentions = value; } }
    }
}
