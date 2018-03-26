namespace GraphicsResearch.Generation
{
    using System.Collections.Generic;
    using UnityEngine;
    using Paths;

    public class MiddleLayer : MonoBehaviour
    {
        public bool drawPaths;

        public List<LayerPath> Paths { get; set; }

        public float WidthScale { get { return this.transform.localScale.x; } }

        private void OnDrawGizmos()
        {
            if (this.drawPaths && this.Paths != null)
            {
                foreach (Path p in this.Paths)
                    p.Draw(this.WidthScale);
            }
        }
    }
}
