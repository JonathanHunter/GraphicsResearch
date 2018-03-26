namespace GraphicsResearch.Generation.Rooms
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Grids;

    public class JitteredPlacement : RoomGenerator
    {
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
        /// <summary> The number of attempts to try and place each room.</summary>
        [SerializeField]
        private int numberOfAttempts = 100;
        /// <summary> Whether or not to only place circles. </summary>
        [SerializeField]
        protected bool onlyCircles;
        /// <summary> Whether or not to rotate room templets during placement. </summary>
        [SerializeField]
        protected bool rotate = false;
        /// <summary> Whether or not to resize room templets during placement. </summary>
        [SerializeField]
        protected bool resize = false;
        /// <summary> The range of values to pick from during room templet resizing. </summary>
        [SerializeField]
        protected Vector2 sizeBound = Vector2.zero;

        protected override void LocalInit()
        {
        }

        protected override IEnumerator LocalPlaceRoomsAsync(Floor f)
        {
            List<Room> rooms = new List<Room>();
            for (int r = 0; r < f.samplingGrid.Dimension.x; r++)
            {
                for (int c = 0; c < f.samplingGrid.Dimension.y; c++)
                {
                    Room room = Sample(f, r, c);
                    if(room != null)
                        rooms.Add(room);
                }

                yield return null;
            }

            foreach(Room r in rooms)
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

        private Room Sample(Floor f, int r, int c)
        {
            Vector2 boxCenter = f.samplingGrid.GetPos(r, c);
            Vector2 verticalBounds = new Vector2(boxCenter.y - f.samplingGrid.BoxSize.y / 2f, boxCenter.y + f.samplingGrid.BoxSize.y / 2f);
            Vector2 horizontalBounds = new Vector2(boxCenter.x - f.samplingGrid.BoxSize.x / 2f, boxCenter.x + f.samplingGrid.BoxSize.x / 2f);
            return RayCastDart(this.onlyCircles? true : Random.Range(0f, 1f) < .5f, verticalBounds, horizontalBounds);
        }

        /// <summary> Uses Raycasting to attempt to place a room within the given bounds. </summary>
        /// <param name="isCircle"> Whether to place a rectangle or a circle. </param>
        /// <param name="verticalBounds"> The vertical limits in which to try to place a room. </param>
        /// <param name="horizontalBounds"> The horizontal limits in which to try to place a room. </param>
        /// <returns> The placed room if one can be placed. </returns>
        private Room RayCastDart(bool isCircle, Vector2 verticalBounds, Vector2 horizontalBounds)
        {
            int currentAttempt = 0;
            while (currentAttempt < this.numberOfAttempts)
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
                        circle.Init();
                        circle.transform.position = new Vector2(x, y);
                        circle.OriginalPosition = circle.transform.position;
                        circle.transform.localScale = Vector3.one * this.circleRadius * 2f * size;
                        circle.transform.localScale = new Vector3(circle.transform.localScale.x, circle.transform.localScale.y, 1);
                        circle.Radius = this.circleRadius * size;
                        return circle;
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
                        rect.Dimentions = rect.transform.localScale;
                        return rect;
                    }
                }

                currentAttempt++;
            }

            Debug.Log("Unable to place room after " + currentAttempt + " attempts.");
            return null;
        }
    }
}
