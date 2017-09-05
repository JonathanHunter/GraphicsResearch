namespace GraphicsResearch.MST
{
    using UnityEngine;

    public class GraphEdge
    {
        public GameObject Start { get; private set; }

        public GameObject End { get; private set; }

        public float Weight { get; private set; }

        public GraphEdge(GameObject start, GameObject end, float weight)
        {
            this.Start = start;
            this.End = end;
            this.Weight = weight;
        }
    }
}
