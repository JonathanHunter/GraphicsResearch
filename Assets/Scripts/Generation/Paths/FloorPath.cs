namespace GraphicsResearch.Generation.Paths
{
    public class FloorPath : Path
    {
        public Edge[] Edges { get; set; }

        public override void Draw(float widthScale)
        {
            foreach (Edge edge in this.Edges)
            {
                edge.Draw(widthScale);
            }
        }
    }
}
