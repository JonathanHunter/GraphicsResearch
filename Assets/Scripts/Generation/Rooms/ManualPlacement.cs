namespace GraphicsResearch.Generation.Rooms
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ManualPlacement : RoomGenerator
    {
        [SerializeField]
        public Room[] rooms1;

        [SerializeField]
        public Room[] rooms2;

        private int i;

        protected override void LocalInit()
        {
            i = 0;
        }

        protected override IEnumerator LocalPlaceRoomsAsync(Floor f)
        {
            List<Room> rooms = new List<Room>();
            if (i == 0)
            {
                i++;
                foreach (Room r in this.rooms1)
                    rooms.Add(r);
            }
            else
            {
                i = 0;
                foreach (Room r in this.rooms1)
                    rooms.Add(r);
            }

            foreach (Room r in rooms)
            {
                Vector3 pos = r.transform.position;
                Quaternion rot = r.transform.rotation;
                Vector3 size = r.transform.localScale;
                r.transform.SetParent(f.roomParent);
                r.transform.localPosition = pos;
                r.transform.localRotation = rot;
                r.transform.localScale = size;
                r.GetComponent<Collider>().enabled = false;
            }

            f.Rooms = rooms;
            return null;
        }
    }
}
