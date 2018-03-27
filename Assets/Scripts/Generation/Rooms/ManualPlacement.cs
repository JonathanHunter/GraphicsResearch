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
                {
                    rooms.Add(MakeCopy(r));
                }
            }
            else
            {
                i = 0;
                foreach (Room r in this.rooms2)
                    rooms.Add(MakeCopy(r));
            }
            yield return null;

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
        }

        private Room MakeCopy(Room r)
        {
            if (r is CircleRoom)
            {
                CircleRoom circle = (CircleRoom)Instantiate(r);
                circle.Init();
                circle.transform.position = r.transform.position;
                circle.OriginalPosition = circle.transform.position;
                circle.transform.localScale = r.transform.localScale;
                circle.Radius = r.transform.localScale.x / 2f;
                r.gameObject.SetActive(false);
                return circle;
            }
            else
            {
                RectangleRoom rect = (RectangleRoom)Instantiate(r);
                rect.transform.position = r.transform.position;
                rect.OriginalPosition = rect.transform.position;
                rect.transform.rotation = r.transform.rotation;
                rect.transform.localScale = r.transform.localScale;
                rect.Dimentions = rect.transform.localScale;
                r.gameObject.SetActive(false);
                return rect;
            }
        }
    }
}
