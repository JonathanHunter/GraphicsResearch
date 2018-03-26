namespace GraphicsResearch.Generation.Paths
{
    using Rooms;

    public abstract class Path
    {
        public Room Start { get; set; }
        public Room End { get; set; }

        public float Distance { get; set; }
        public float Width { get; set; }

        public abstract void Draw();
    }
}
