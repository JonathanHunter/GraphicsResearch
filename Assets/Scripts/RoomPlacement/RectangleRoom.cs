namespace GraphicsResearch.RoomPlacement
{
    using UnityEngine;

    public class RectangleRoom : MonoBehaviour
    {
        [SerializeField]
        private Vector2 dimentions;

        public Vector2 Dimentions { get { return this.dimentions; } internal set { this.dimentions = value; } }

        public Vector3 OriginalPosition { get; set; }
    }
}
