namespace GraphicsResearch.RoomPlacement
{
    using System.Collections;
    using UnityEngine;
    using Util;

    public class JitteredSampling : RoomManager
    {
        [SerializeField]
        private Vector2 gridCount = Vector2.zero;
        [SerializeField]
        private int numberOfAttempts = 100;
        [SerializeField]
        private bool showGrid = false;

        public Grid2D<GameObject> Samples { get; private set; }

        private Vector2 boxSize;

        private void OnDrawGizmos()
        {
            if(this.showGrid && this.Samples != null)
            {
                for(int r = 0; r < this.gridCount.x; r++)
                {
                    for(int c = 0; c < this.gridCount.y; c++)
                    {
                        Vector2 boxCenter = this.Samples.GetPos(r, c);
                        Gizmos.DrawWireCube(boxCenter, this.boxSize);
                    }
                }
            }
        }

        public override void PlaceRoom(bool isCircle)
        {
            Debug.Log("Can't place extra rooms with jittered sampling. ");
        }

        protected override void LocalInit()
        {
            this.boxSize = new Vector2(this.dimensions.x / this.gridCount.x, this.dimensions.y / this.gridCount.y);
            this.Samples = new Grid2D<GameObject>((int)this.gridCount.x, (int)this.gridCount.y, this.topLeft, this.boxSize);
        }

        protected override void LocalPlaceRooms()
        {
            for (int r = 0; r < this.gridCount.x; r++)
            {
                for (int c = 0; c < this.gridCount.y; c++)
                {
                    Sample(r, c);
                }
            }
        }

        protected override IEnumerator LocalPlaceRoomsAsync()
        {
            for (int r = 0; r < this.gridCount.x; r++)
            {
                for (int c = 0; c < this.gridCount.y; c++)
                {
                    Sample(r, c);
                }

                yield return null;
            }
        }

        protected override void LocalClear()
        {
            this.boxSize = new Vector2(this.dimensions.x / this.gridCount.x, this.dimensions.y / this.gridCount.y);
            this.Samples = new Grid2D<GameObject>((int)this.gridCount.x, (int)this.gridCount.y, this.topLeft, this.boxSize);
        }

        private void Sample(int r, int c)
        {
            Vector2 boxCenter = this.Samples.GetPos(r, c);
            Vector2 verticalBounds = new Vector2(boxCenter.y - this.boxSize.y / 2f, boxCenter.y + this.boxSize.y / 2f);
            Vector2 horizontalBounds = new Vector2(boxCenter.x - this.boxSize.x / 2f, boxCenter.x + this.boxSize.x / 2f);
            if (this.onlyCircles)
                this.Samples.Set(r, c, RayCastDart(true, this.numberOfAttempts, verticalBounds, horizontalBounds));
            else
                this.Samples.Set(r, c, RayCastDart(Random.Range(0f, 1f) < .5f, this.numberOfAttempts, verticalBounds, horizontalBounds));
        }
    }
}
