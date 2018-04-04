namespace GraphicsResearch.Generation
{
    using System.Collections.Generic;
    using UnityEngine;
    using Grids;
    using Meshes;
    using Paths;

    public class MiddleLayer : MonoBehaviour
    {
        [SerializeField]
        private bool drawPaths;

        [SerializeField]
        private bool drawLocal;

        [SerializeField]
        private bool drawRasterizationGrid;

        [SerializeField]
        private Vector2Int rasterizationGridFocus;

        public RasterizationGrid rasterizationGrid;

        public List<LayerPath> Paths { get; set; }

        public Square[,,,] Squares { get; set; }

        public float WidthScale { get { return this.transform.localScale.x; } }

        private void OnDrawGizmos()
        {
            if (this.drawPaths && this.Paths != null)
            {
                foreach (Path p in this.Paths)
                    p.Draw(drawLocal ? 1 : this.WidthScale, this.drawLocal);
            }

            if (this.drawRasterizationGrid)
            {
                this.rasterizationGrid.Draw(this, rasterizationGridFocus);
            }
        }
    }
}
