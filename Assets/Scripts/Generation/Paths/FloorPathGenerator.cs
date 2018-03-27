namespace GraphicsResearch.Generation.Paths
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Rooms;

    public class FloorPathGenerator : MonoBehaviour
    {
        /// <summary> The amoutn of extra edges to include in the path generation. </summary>
        [SerializeField]
        [Range(0f, 1f)]
        protected float edgeAmount;
        [SerializeField]
        private float pathWidth = .5f;
        [SerializeField]
        private int maxOffsetAttempts = 10;
        [SerializeField]
        private int maxRecursionDepth = 6;
        [SerializeField]
        private int maxFancyPathsPerRoom = 3;

        /// <summary> Finds all the allowed floor paths for the given floor. </summary>
        /// <param name="floor"> The floor. </param>
        /// <param name="upperLayer"> The layer above this floor. </param>
        /// <param name="upperLayer"> The layer below this floor. </param>
        public IEnumerator FindFloorPathsAsync(Floor floor, MiddleLayer upperLayer, MiddleLayer lowerLayer)
        {
            List<FloorPath> basePaths = new List<FloorPath>();
            List<Room> objects = new List<Room>();

            foreach (Room r1 in floor.Rooms)
            {
                int fancyPaths = 0;
                foreach (Room r2 in floor.Rooms)
                {
                    TryAddPath(r1, r2, basePaths, ref fancyPaths);
                }

                objects.Add(r1);
                yield return null;
            }

            Kruskals(basePaths, objects);
            yield return null;

            List<FloorPath> finalPaths = new List<FloorPath>();
            foreach (Path e in this.Paths)
                finalPaths.Add(e);

            int numToAdd = (int)(this.ExtraPaths.Count * this.edgeAmount);
            for (int i = 0; i < numToAdd; i++)
                finalPaths.Add(this.ExtraPaths[i]);

            floor.Paths = finalPaths;
        }
    }
}
