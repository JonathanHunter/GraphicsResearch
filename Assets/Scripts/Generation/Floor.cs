namespace GraphicsResearch.Generation
{
    using System.Collections.Generic;
    using UnityEngine;
    using Grids;
    using Meshes;
    using Paths;
    using Rooms;

    public class Floor : MonoBehaviour
    {
        [SerializeField]
        private bool drawSamplingGrid;

        [SerializeField]
        private bool drawRasterizationGrid;

        [SerializeField]
        private Vector2Int rasterizationGridFocus;

        public Transform roomParent;

        public SamplingGrid samplingGrid;

        public RasterizationGrid rasterizationGrid;

        /// <summary> The list of rooms currently placed. </summary>
        public List<Room> Rooms { get; set; }

        /// <summary> List of the paths between rooms. </summary>
        public List<FloorPath> Paths { get; set; }

        public Square[,,,] Squares { get; set; }

        public float WidthScale { get { return this.transform.localScale.x; } }

        private void OnDrawGizmos()
        {
            if (this.drawSamplingGrid)
            {
                this.samplingGrid.Draw(this);
                if (this.Paths != null)
                {
                    foreach (FloorPath p in this.Paths)
                        p.Draw(1, true);
                }
            }

            if(this.drawRasterizationGrid)
            {
                this.rasterizationGrid.Draw(this, rasterizationGridFocus);
            }
        }
    }
}
