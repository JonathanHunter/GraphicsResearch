namespace GraphicsResearch.RoomPlacement
{
    using UnityEngine;

    public class PoissonDiskSampling : RoomManager
    {
        [SerializeField]
        private int numberOfDarts = 100;
        [SerializeField]
        private int numberOfAttempts = 100;

        private Vector2 globalVerticalBounds;
        private Vector2 globalHorizontalBounds;

        public override void PlaceRoom(bool isCircle)
        {
            RayCastDart(isCircle, this.numberOfAttempts, this.globalVerticalBounds, this.globalHorizontalBounds);
        }

        protected override void LocalInit()
        {
            this.globalVerticalBounds = new Vector2(this.topLeft.position.y - this.dimensions.y, this.topLeft.position.y);
            this.globalHorizontalBounds = new Vector2(this.topLeft.position.x, this.topLeft.position.x + this.dimensions.x);
        }

        protected override void LocalPlaceRooms()
        {
            for (int i = 0; i < numberOfDarts; i++)
            {
                if (this.onlyCircles)
                    RayCastDart(true, this.numberOfAttempts, this.globalVerticalBounds, this.globalHorizontalBounds);
                else
                    RayCastDart(Random.Range(0f, 1f) < .5f, this.numberOfAttempts, this.globalVerticalBounds, this.globalHorizontalBounds);
            }
        }

        protected override void LocalClear()
        {
        }
    }
}
