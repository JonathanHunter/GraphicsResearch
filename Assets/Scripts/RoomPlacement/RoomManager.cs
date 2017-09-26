namespace GraphicsResearch.RoomPlacement
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class RoomManager : MonoBehaviour
    {
        /// <summary> The center of the box where rooms will be placed. </summary>
        [SerializeField]
        protected Transform center = null;
        /// <summary> The dimensions of the box where rooms will be placed. </summary>
        [SerializeField]
        protected Vector2 dimensions = Vector2.zero;
        /// <summary> The templet for spawning a circle room. </summary>
        [SerializeField]
        protected CircleRoom circleTemplet = null;
        /// <summary> The default radius for the circle rooms. </summary>
        [SerializeField]
        protected float circleRadius = .5f;
        /// <summary> The templet for spawning a rectangle room. </summary>
        [SerializeField]
        protected RectangleRoom rectangleTemplet = null;
        /// <summary> The default size for the rectangle rooms. </summary>
        [SerializeField]
        protected Vector2 rectDimensions = Vector2.one;
        /// <summary> Whether or not to rotate room templets during placement. </summary>
        [SerializeField]
        protected bool rotate = false;
        /// <summary> Whether or not to resize room templets during placement. </summary>
        [SerializeField]
        protected bool resize = false;
        /// <summary> The range of values to pick from during room templet resizing. </summary>
        [SerializeField]
        protected Vector2 sizeBound = Vector2.zero;
        /// <summary> Whether or not to only place circles. </summary>
        [SerializeField]
        protected bool onlyCircles;

        /// <summary> The list of circle rooms currently placed. </summary>
        public List<CircleRoom> CircleRooms { get; protected set; }
        /// <summary> The list of rectangle rooms currently placed. </summary>
        public List<RectangleRoom> RectangleRooms { get; protected set; }

        /// <summary> Initializes this room manager. </summary>
        public void Init()
        {
            this.CircleRooms = new List<CircleRoom>();
            this.RectangleRooms = new List<RectangleRoom>();
            LocalInit();
        }

        /// <summary> Clears any previous rooms and places a new set of rooms. </summary>
        public void PlaceRooms()
        {
            Clear();
            LocalPlaceRooms();
        }

        /// <summary> Clears all current rooms. </summary>
        public void Clear()
        {
            foreach (CircleRoom c in this.CircleRooms)
                Destroy(c.gameObject);

            foreach (RectangleRoom r in this.RectangleRooms)
                Destroy(r.gameObject);

            LocalClear();
        }

        /// <summary> Causes all rooms to push each other away. </summary>
        public void Repulse()
        {
            foreach (CircleRoom d in this.CircleRooms)
            {
                Collider[] things = Physics.OverlapSphere(d.transform.position, d.Radius * 2f);
                foreach (Collider c in things)
                {
                    if (c.gameObject != d.gameObject &&
                        (c.gameObject.GetComponent<CircleRoom>() != null || c.gameObject.GetComponent<RectangleRoom>() != null))
                    {
                        Vector3 ray = c.gameObject.transform.position - d.transform.position;
                        c.gameObject.GetComponent<Rigidbody>().AddForce(ray.normalized * (1f / ray.magnitude));
                    }
                }
            }

            foreach (RectangleRoom d in this.RectangleRooms)
            {
                Collider[] things = Physics.OverlapBox(d.transform.position, d.Dimentions * 2f);
                foreach (Collider c in things)
                {
                    if (c.gameObject != d.gameObject &&
                        (c.gameObject.GetComponent<CircleRoom>() != null || c.gameObject.GetComponent<RectangleRoom>() != null))
                    {
                        Vector3 ray = c.gameObject.transform.position - d.transform.position;
                        c.gameObject.GetComponent<Rigidbody>().AddForce(ray.normalized * (1f / ray.magnitude));
                    }
                }
            }
        }

        /// <summary> Causes all rooms to stop moving. </summary>
        public void Halt()
        {
            foreach (CircleRoom c in this.CircleRooms)
            {
                c.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                c.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }

            foreach (RectangleRoom r in this.RectangleRooms)
            {
                r.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                r.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }

        /// <summary> Returns all rooms to their original positions. </summary>
        public void ResetRooms()
        {
            foreach (CircleRoom c in this.CircleRooms)
                c.transform.position = c.OriginalPosition;

            foreach (RectangleRoom r in this.RectangleRooms)
                r.transform.position = r.OriginalPosition;
        }

        /// <summary> Turns all of the room gameobjects on or off. </summary>
        public void ToggleRooms()
        {
            foreach (CircleRoom c in this.CircleRooms)
                c.gameObject.SetActive(!c.gameObject.activeSelf);

            foreach (RectangleRoom r in this.RectangleRooms)
                r.gameObject.SetActive(!r.gameObject.activeSelf);
        }

        /// <summary> Places a new room. </summary>
        public abstract void PlaceRoom(bool isCircle);

        /// <summary> Local handler for initialization. </summary>
        protected abstract void LocalInit();
        /// <summary> Local handler for room placement. </summary>
        protected abstract void LocalPlaceRooms();
        /// <summary> Local handler for room clearing. </summary>
        protected abstract void LocalClear();

        /// <summary> Uses Raycasting to attempt to place a room within the given bounds. </summary>
        /// <param name="isCircle"> Whether to place a rectangle or a circle. </param>
        /// <param name="numberOfAttempts"> The number of attempts to place the room. </param>
        /// <param name="verticalBounds"> The vertical limits in which to try to place a room. </param>
        /// <param name="horizontalBounds"> The horizontal limits in which to try to place a room. </param>
        /// <returns> The placed room if one can be placed. </returns>
        protected GameObject RayCastDart(bool isCircle, int numberOfAttempts, Vector2 verticalBounds, Vector2 horizontalBounds)
        {
            int currentAttempt = 0;
            while (currentAttempt < numberOfAttempts)
            {
                if (isCircle)
                {
                    float x = Random.Range(horizontalBounds.x, horizontalBounds.y);
                    float y = Random.Range(verticalBounds.y, verticalBounds.x);
                    float size = this.resize ? Random.Range(this.sizeBound.x, this.sizeBound.y) : 1f;
                    Collider[] overlaps = Physics.OverlapSphere(new Vector2(x, y), this.circleRadius * size);
                    if (overlaps.Length == 0)
                    {
                        CircleRoom circle = Instantiate<CircleRoom>(this.circleTemplet);
                        circle.transform.position = new Vector2(x, y);
                        circle.OriginalPosition = circle.transform.position;
                        circle.transform.localScale = Vector3.one * this.circleRadius * 2f * size;
                        circle.Radius = this.circleRadius * size;
                        this.CircleRooms.Add(circle);
                        return circle.gameObject;
                    }
                }
                else
                {
                    float x = Random.Range(horizontalBounds.x, horizontalBounds.y);
                    float y = Random.Range(verticalBounds.y, verticalBounds.x);
                    float sizeX = this.resize ? Random.Range(this.sizeBound.x, this.sizeBound.y) : 1f;
                    float sizeY = this.resize ? Random.Range(this.sizeBound.x, this.sizeBound.y) : 1f;
                    Quaternion rotation = this.rotate ? Quaternion.Euler(new Vector3(1, 1, Random.Range(0f, 90f))) : Quaternion.identity;
                    Collider[] overlaps = Physics.OverlapBox(new Vector2(x, y), new Vector3(this.rectDimensions.x * sizeX, this.rectDimensions.y * sizeY, 1) / 2f, rotation);
                    if (overlaps.Length == 0)
                    {
                        RectangleRoom rect = Instantiate<RectangleRoom>(this.rectangleTemplet);
                        rect.transform.position = new Vector2(x, y);
                        rect.OriginalPosition = rect.transform.position;
                        rect.transform.rotation = rotation;
                        rect.transform.localScale = new Vector3(this.rectDimensions.x * sizeX, this.rectDimensions.y * sizeY, 1);
                        rect.Dimentions = this.rectDimensions;
                        this.RectangleRooms.Add(rect);
                        return rect.gameObject;
                    }
                }

                currentAttempt++;
            }

            Debug.Log("Unable to place room after " + currentAttempt + " attempts.");
            return null;
        }
    }
}
