namespace GraphicsResearch.MeshGeneration.MeshObjects
{
    using System.Collections.Generic;
    using UnityEngine;

    public interface IMeshObject 
    {
        /// <summary> Opens the mesh up at the specified points. </summary>
        /// <param name="openingCorners"> The corners of the mesh to open up at. </param>
        void RemoveWallAt(List<Vector3> openingCorners);
    }
}
