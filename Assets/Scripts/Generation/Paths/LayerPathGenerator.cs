namespace GraphicsResearch.Generation.Paths
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Rooms;

    public class LayerPathGenerator : MonoBehaviour
    {
        /// <summary> The range of allowed gradients. </summary>
        [SerializeField]
        private Vector2 gradientRange;

        /// <summary> The range of allowed distances. </summary>
        [SerializeField]
        private Vector2 distanceRange;

        /// <summary> The number of paths to keep. </summary>
        [SerializeField]
        private int pathsToKeep;

        /// <summary> The path width for layer paths. </summary>
        [SerializeField]
        private float layerPathWidth;

        /// <summary> Finds all the allowed middle layer paths for the given floors. </summary>
        /// <param name="floor1"> The top floor. </param>
        /// <param name="floor2"> The bottom floor. </param>
        /// <param name="layer"> The layer to hold the paths. </param>
        public IEnumerator FindMiddleLayerPathsAsync(Floor floor1, Floor floor2, MiddleLayer layer)
        {
            foreach (Room r in floor1.Rooms)
                r.GetComponent<Collider>().enabled = true;

            foreach (Room r in floor2.Rooms)
                r.GetComponent<Collider>().enabled = true;

            List<LayerPath> paths = new List<LayerPath>();
            foreach (Room r1 in floor1.Rooms)
            {
                foreach (Room r2 in floor2.Rooms)
                {
                    LayerPath p = CompareRoom(r1, r2, floor1, floor2, this.layerPathWidth * layer.WidthScale, paths);
                    if (p != null)
                        paths.Add(p);
                }

                yield return null;
            }

            List<LayerPath> finalPaths = new List<LayerPath>();
            int i = 0;
            while(i < this.pathsToKeep && i < paths.Count)
            {
                LayerPath path = paths[Random.Range(0, paths.Count - 1)];
                bool blocked = false;
                foreach (LayerPath p in finalPaths)
                {
                    if (GenerationUtility.CheckLineLineIntersection(
                        path.Start.Position,
                        path.End.Position,
                        p.Start.Position,
                        p.End.Position))
                    {
                        blocked = true;
                    }
                }

                if(!blocked)
                {
                    i++;
                    finalPaths.Add(path);
                }
            }

            layer.Paths = finalPaths;

            foreach (Room r in floor1.Rooms)
                r.GetComponent<Collider>().enabled = false;

            foreach (Room r in floor2.Rooms)
                r.GetComponent<Collider>().enabled = false;
        }

        private LayerPath CompareRoom(Room r1, Room r2, Floor floor1, Floor floor2, float pathWidth, List<LayerPath> paths)
        {
            float gradient = GetGradient(r1, r2);
            float distance = GetDistance(r1, r2);

            Vector3 r12 = r2.WorldPosition- r1.WorldPosition;
            Vector3 left = new Vector3(-r12.y, r12.x, r12.z).normalized;
            RaycastHit[] hits = Physics.RaycastAll(r1.transform.position + left * pathWidth / 2f, r12.normalized, r12.magnitude);
            int hitCount = hits.Length;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == r1.gameObject || hit.collider.gameObject == r2.gameObject)
                    hitCount--;
            }

            hits = Physics.RaycastAll(r1.transform.position - left * pathWidth / 2f, r12.normalized, r12.magnitude);
            hitCount += hits.Length;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == r1.gameObject || hit.collider.gameObject == r2.gameObject)
                    hitCount--;
            }

            if (gradient >= this.gradientRange.x && gradient <= this.gradientRange.y &&
                distance >= this.distanceRange.x && distance <= this.distanceRange.y &&
                hitCount == 0)
            {
                LayerPath p = new LayerPath
                {
                    Start = r1,
                    End = r2,
                    Distance = distance,
                    Width = this.layerPathWidth,
                    Gradient = gradient
                };
                
                return p;
            }

            return null;
        }

        private float GetGradient(Room r1, Room r2)
        {
            Vector3 refPoint = new Vector3(r1.WorldPosition.x, r2.WorldPosition.y, r1.WorldPosition.z);
            Vector3 r21 = Vector3.Normalize(r1.WorldPosition - r2.WorldPosition);
            Vector3 r2ref = Vector3.Normalize(refPoint - r2.WorldPosition);
            return Vector3.Angle(r2ref, r21);
        }

        private float GetDistance(Room r1, Room r2)
        {
            return Vector3.Distance(r1.WorldPosition, r2.WorldPosition);
        }
    }
}
