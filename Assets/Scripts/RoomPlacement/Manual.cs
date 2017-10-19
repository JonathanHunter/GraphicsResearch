namespace GraphicsResearch.RoomPlacement
{
    using UnityEngine;

    public class Manual : RoomManager
    {
        [SerializeField]
        private CircleRoom[] circles;
        [SerializeField]
        private RectangleRoom[] rects;

        public override void PlaceRoom(bool isCircle)
        {
        }

        protected override void LocalClear()
        {
        }
        
        protected override void LocalInit()
        {
        }

        protected override void LocalPlaceRooms()
        {
            foreach (CircleRoom c in this.circles)
            {
                c.OriginalPosition = c.transform.position;
                c.Radius = c.transform.localScale.x;
                this.CircleRooms.Add(c);
            }

            foreach (RectangleRoom r in this.rects)
            {
                r.OriginalPosition = r.transform.position;
                r.Dimentions = new Vector2(r.transform.localScale.x, r.transform.localScale.y);
                this.RectangleRooms.Add(r);
            }
        }
    }
}
