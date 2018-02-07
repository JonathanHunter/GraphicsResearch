namespace GraphicsResearch.RoomPlacement
{
    using UnityEngine;

    public class CircleRoom : Room
    {
        [SerializeField]
        private float radius;
        [SerializeField]
        private MeshGeneration.MeshObjects.Circle circle;

        public float Radius { get { return this.radius; } internal set { this.radius = value; } }

        public void Init()
        {
            this.circle.Create(.5f, .5f, true);
        }
    }
}
