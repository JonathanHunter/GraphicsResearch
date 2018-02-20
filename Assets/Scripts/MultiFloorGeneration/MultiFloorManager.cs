namespace GraphicsResearch.MultiFloorGeneration
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using RoomPlacement;
    using System;

    public class MultiFloorManager : MonoBehaviour
    {
        public Vector2 gradientRange;
        public Vector2 distanceRange;
        public bool drawPaths;
        public bool drawHandles;

        private List<MulitFloorPath> paths;

        public void Init()
        {
            paths = new List<MulitFloorPath>();
        }

        private void OnDrawGizmos()
        {
            if (this.paths != null)
            {
                foreach (MulitFloorPath p in this.paths)
                {
                    if(this.drawPaths)
                        p.Draw();
                    if(this.drawHandles)
                        Handles.Label(Vector3.Lerp(p.Room1.transform.position, p.Room2.transform.position, .5f), 
                            "Gradient: " + p.Gradient + "\nDistance: " + p.Distance);
                }
            }
        }

        public void FindRoomPairs(RoomManager rooms1, RoomManager rooms2)
        {
            foreach (Room r1 in rooms1.Rooms)
            {
                foreach(Room r2 in rooms2.Rooms)
                {
                    CompareRoom(r1, r2);
                }
            }
        }

        public IEnumerator FindRoomPairsAsync(RoomManager rooms1, RoomManager rooms2)
        {
            foreach (Room r1 in rooms1.Rooms)
            {
                foreach (Room r2 in rooms2.Rooms)
                {
                    CompareRoom(r1, r2);
                }

                yield return null;
            }

            yield return null;
        }

        private void CompareRoom(Room r1, Room r2)
        {
            float gradient = GetGradient(r1, r2);
            float distance = GetDistance(r1, r2);
            Vector3 dir = Vector3.Normalize(r1.transform.position - r2.transform.position);
            RaycastHit[] hits = Physics.RaycastAll(r1.transform.position, dir, distance);
            bool blocked = false;
            foreach (RaycastHit rh in hits)
            {
                if (rh.collider.gameObject != r1 && rh.collider.gameObject != r2)
                    blocked = true;
            }

            if (!blocked)
            {
                if (gradient >= this.gradientRange.x && gradient <= this.gradientRange.y &&
                    distance >= this.distanceRange.x && distance <= this.distanceRange.y)
                {
                    MulitFloorPath p = new MulitFloorPath(r1, r2, gradient, distance);
                    this.paths.Add(p);
                }
            }
        }

        private float GetGradient(Room r1, Room r2)
        {
            Vector3 refPoint = new Vector3(r1.transform.position.x, r2.transform.position.y, r1.transform.position.z);
            Vector3 r21 = Vector3.Normalize(r1.transform.position - r2.transform.position);
            Vector3 r2ref = Vector3.Normalize(refPoint - r2.transform.position);
            return Vector3.Angle(r2ref, r21);
        }

        private float GetDistance(Room r1, Room r2)
        {
            return Vector3.Distance(r1.transform.position, r2.transform.position);
        }
    }
}
