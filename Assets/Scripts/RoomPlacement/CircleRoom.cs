namespace GraphicsResearch.RoomPlacement
{
    using UnityEngine;

    public class CircleRoom : MonoBehaviour
    {
        [SerializeField]
        private float radius;

        public float Radius { get { return this.radius; } internal set { this.radius = value; } }

        public Vector3 OriginalPosition { get; set; }
    }
}
