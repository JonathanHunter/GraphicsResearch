namespace GraphicsResearch.Generation
{
    using System.Collections.Generic;
    using UnityEngine;
    using Rooms;

    public class Floor : MonoBehaviour
    {
        public Transform roomParent;

        /// <summary> The list of rooms currently placed. </summary>
        public List<Room> Rooms { get; set; }


    }
}
