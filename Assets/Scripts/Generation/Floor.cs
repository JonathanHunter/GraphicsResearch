namespace GraphicsResearch.Generation
{
    using System.Collections.Generic;
    using UnityEngine;
    using Grids;
    using Rooms;

    public class Floor : MonoBehaviour
    {
        public Transform roomParent;

        public SamplingGrid samplingGrid;

        /// <summary> The list of rooms currently placed. </summary>
        public List<Room> Rooms { get; set; }


    }
}
