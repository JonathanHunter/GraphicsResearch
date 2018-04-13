namespace GraphicsResearch.MeshGeneration
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TunnelAndPlace : MonoBehaviour
    {
        public Room roomTemplet;
        public Tunnel tunnelTemplet;
        public Transform topLeft;
        public Transform bottomRight;
        public int roomLimit;

        private enum Direction { North, South, East, West }

        private List<Tunnel> paths;
        private float top;
        private float bottom;
        private float left;
        private float right;
        private int limit;

        private void Start()
        {
            this.paths = new List<Tunnel>();
            this.top = this.topLeft.position.y;
            this.bottom = this.bottomRight.position.y;
            this.left = this.topLeft.position.x;
            this.right = this.bottomRight.position.x;
            this.limit = 1;

            StartCoroutine(Generate());
        }

        private IEnumerator Generate()
        {
            Vector2 pos = new Vector2(Random.Range(left + 1, right - 1), Random.Range(bottom + 1, top - 1));
            Room room = PlaceRoom(pos, Direction.North, null);
            Tunnel tunnel = PlaceTunnel(room);
            this.paths.Add(tunnel);
            while (this.paths.Count > 0 && this.limit < this.roomLimit)
            {
                tunnel = this.paths[0];
                room = PlaceRoom(tunnel.end.position, GetDirection(tunnel), tunnel);
                if (room == null)
                {
                    Destroy(tunnel.gameObject);
                }
                else
                {
                    this.limit++;
                    Tunnel t = PlaceTunnel(room);
                    if (t != null)
                        this.paths.Add(t);

                    if (Random.Range(0f, 1f) < .7f)
                    {
                        t = PlaceTunnel(room);
                        if (t != null)
                            this.paths.Add(t);
                    }
                }

                this.paths.Remove(tunnel);
                yield return null;
            }

            foreach (Tunnel t in this.paths)
                Destroy(t.gameObject);
        }

        private Room PlaceRoom(Vector2 pos, Direction dir, Tunnel t)
        {
            if (dir == Direction.North)
                pos += Vector2.down;
            if (dir == Direction.South)
                pos += Vector2.up;
            if (dir == Direction.East)
                pos += Vector2.left;
            if (dir == Direction.West)
                pos += Vector2.right;

            Collider[] overlaps = Physics.OverlapBox(pos, Vector3.one);
            int count = overlaps.Length;
            if (t != null)
            {
                foreach (Collider c in overlaps)
                {
                    if (c.gameObject == t.gameObject)
                        count--;
                }
            }

            if (count == 0 && WithinBounds(pos))
            {
                Room room = Instantiate<Room>(this.roomTemplet);
                room.transform.position = pos;
                return room;
            }
            else
                return null;

        }
        
        private Tunnel PlaceTunnel(Room room)
        {
            Vector2 north = room.north.transform.position + Vector3.up;
            Vector2 south = room.south.transform.position + Vector3.down;
            Vector2 east = room.east.transform.position + Vector3.right;
            Vector2 west = room.west.transform.position + Vector3.left;
            Debug.Log(north + ", " + south + ", " + east + ", " + west);

            List<Direction> dir = new List<Direction> { Direction.North, Direction.South, Direction.East, Direction.West };
            Shuffle(dir);
            foreach(Direction d in dir)
            {
                Debug.Log(d);
                Vector2 pos = Vector2.zero;
                int rot = 0;
                Vector3 size = Vector3.zero;
                if (d == Direction.North)
                {
                    pos = north;
                    rot = 90;
                    size = new Vector3(.5f, 1f, .5f);
                }
                if (d == Direction.South)
                {
                    pos = south;
                    rot = 270;
                    size = new Vector3(.5f, 1f, .5f);
                }
                if (d == Direction.East)
                {
                    pos = east;
                    rot = 0;
                    size = new Vector3(1f, .5f, .5f);
                }
                if (d == Direction.West)
                {
                    pos = west;
                    rot = 180;
                    size = new Vector3(1f, .5f, .5f);
                }


                Collider[] overlaps = Physics.OverlapBox(pos, size);
                int count = overlaps.Length;
                foreach(Collider c in overlaps)
                {
                    if (c.gameObject == room.gameObject)
                        count--;
                    else
                        Debug.Log(c.gameObject.name + " " + c.transform.position);
                }
                Debug.Log(count + " " + WithinBounds(pos));
                if (count == 0 && WithinBounds(pos))
                {
                    Tunnel t = Instantiate<Tunnel>(this.tunnelTemplet);
                    t.transform.position = pos;
                    t.transform.rotation = Quaternion.Euler(0, 0, rot);
                    return t;
                }
            }

            return null;
        }

        private Direction GetDirection(Tunnel tunnel)
        {
            if (tunnel.transform.rotation.eulerAngles.z == 0)
                return Direction.West;
            if (tunnel.transform.rotation.eulerAngles.z == 90)
                return Direction.South;
            if (tunnel.transform.rotation.eulerAngles.z == 180)
                return Direction.East;
            return Direction.North;
        }

        private bool WithinBounds(Vector2 pos)
        {
            return pos.x > this.left && pos.x < this.right &&
                pos.y > this.bottom && pos.y < this.top;
        }

        private void Shuffle(List<Direction> dir)
        {
            for(int i = 0; i < dir.Count; i++)
            {
                int rand = Random.Range(0, dir.Count);
                Direction temp = dir[i];
                dir[i] = dir[rand];
                dir[rand] = temp;
            }
        }
    }
}
