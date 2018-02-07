namespace GraphicsResearch.MeshGeneration
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using MeshObjects;
    using PathPlacement;
    using RoomPlacement;
    using Util;

    public class ObjectMesh : MeshManager
    {
        [SerializeField]
        private Circle circleTemplet;

        private List<Circle> circles;

        protected override void LocalInit()
        {
            this.circles = new List<MeshObjects.Circle>();
        }

        protected override void LocalReserveGridSquares(RoomManager rooms, PathManager paths)
        {
        }

        protected override void LocalCalculateMesh(RoomManager rooms, PathManager paths)
        {
        }

        protected override IEnumerator LocalCalculateMeshAsync(RoomManager rooms, PathManager paths)
        {
            foreach(CircleRoom r in rooms.CircleRooms)
            {
                Circle c = Instantiate(this.circleTemplet);
                c.transform.position = r.transform.position;
                c.Create(r.Radius, 1f, true);
                this.circles.Add(c);
                yield return null;
            }

            yield return null;
        }

        protected override void LocalCreateMesh()
        {

        }

        protected override IEnumerator LocalCreateMeshAsync()
        {
            yield return null;
        }

        protected override void LocalClear()
        {
            foreach (Circle c in this.circles)
                Destroy(c.gameObject);

            this.circles.Clear();
        }
    }
}
