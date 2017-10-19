namespace GraphicsResearch.PathPlacement
{
    using System.Collections.Generic;
    using RoomPlacement;

    public class Path
    {
        public Edge[] Edges { get; private set; }

        public Room StartRoom { get; private set; }

        public Room EndRoom { get; private set; }

        public float Weight { get; private set; }

        public Path(List<Edge> edges)
        {
            this.Edges = edges.ToArray();
            float weight = 0;
            foreach (Edge e in edges)
                weight += e.Weight;

            this.Weight = weight;
            this.StartRoom = edges[0].StartRoom;
            this.EndRoom = edges[edges.Count - 1].EndRoom;
        }

        public void Draw()
        {
            foreach (Edge edge in this.Edges)
            {
                edge.Draw();
            }
        }
    }
}
