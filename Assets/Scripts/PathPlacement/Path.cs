namespace GraphicsResearch.PathPlacement
{
    using UnityEngine;

    public class Path
    {
        public GameObject Start { get; private set; }

        public GameObject End { get; private set; }

        public float Weight { get; private set; }

        public float Width { get; private set; }

        public Path(GameObject start, GameObject end, float weight, float width)
        {
            this.Start = start;
            this.End = end;
            this.Weight = weight;
            this.Width = width;
        }
    }
}
