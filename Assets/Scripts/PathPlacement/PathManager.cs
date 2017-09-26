namespace GraphicsResearch.PathPlacement
{
    using System.Collections.Generic;
    using UnityEngine;
    using RoomPlacement;

    public abstract class PathManager : MonoBehaviour
    {
        /// <summary> The amoutn of extra edges to include in the path generation. </summary>
        [SerializeField]
        [Range(0f, 1f)]
        protected float edgeAmount;

        /// <summary> The minimum edges generated inorder to connect the rooms. </summary>
        public List<Path> Edges { get; protected set; }
        /// <summary> Extra edges created during path generation. </summary>
        public List<Path> ExtraEdges { get; protected set; }

        /// <summary> Initializes this path manager. </summary>
        public void Init()
        {
            this.Edges = new List<Path>();
            this.ExtraEdges = new List<Path>();
            LocalInit();
        }

        /// <summary> Places paths between the given rooms. </summary>
        /// <param name="rooms"> The rooms to place paths between. </param>
        public void PlacePaths(RoomManager rooms)
        {
            Clear();
            LocalPlacePaths(rooms);
        }

        /// <summary> Clears all of the current paths. </summary>
        public void Clear()
        {
            this.Edges.Clear();
            this.ExtraEdges.Clear();
            LocalClear();
        }

        /// <summary> Gets all of the current paths. </summary>
        /// <returns> The minum path edges plus the request percentage of extra edges. </returns>
        public List<Path> GetPaths()
        {
            List<Path> paths = new List<Path>();
            foreach (Path e in this.Edges)
                paths.Add(e);

            int numToAdd = (int)(this.ExtraEdges.Count * this.edgeAmount);
            for (int i = 0; i < numToAdd; i++)
                paths.Add(this.ExtraEdges[i]);

            return paths;
        }

        /// <summary> Randomizes the extra path edges. </summary>
        public void RandomizeExtraEdges()
        {
            if (this.ExtraEdges != null)
            {
                int n = this.ExtraEdges.Count;
                while (n > 1)
                {
                    n--;
                    int k = Random.Range(0, n + 1);
                    Path edge = this.ExtraEdges[k];
                    this.ExtraEdges[k] = this.ExtraEdges[n];
                    this.ExtraEdges[n] = edge;
                }
            }
        }

        /// <summary> Sorts the extra path edges. </summary>
        public void SortExtraEdges()
        {
            this.ExtraEdges.Sort((x, y) => x.Weight.CompareTo(y.Weight));
        }

        /// <summary> Local handler for initialization. </summary>
        protected abstract void LocalInit();
        /// <summary> Local handler for path placement. </summary>
        protected abstract void LocalPlacePaths(RoomManager rooms);
        /// <summary> Local handler for path clearing. </summary>
        protected abstract void LocalClear();
    }
}
