namespace GraphicsResearch.RoomPlacement
{ 
    using UnityEngine;

    public class JitteredSampling : RoomManager
    {
        [SerializeField]
        private Vector2 gridCount = Vector2.zero;
        [SerializeField]
        private int numberOfAttempts = 100;
        [SerializeField]
        private bool showGrid = false;

        public GameObject[,] Samples { get; private set; }

        private Vector2 boxSize;
        private Vector2 topLeft;
                
        private void OnDrawGizmos()
        {
            if(this.showGrid)
            {
                for(int r = 0; r < this.gridCount.x; r++)
                {
                    for(int c = 0; c < this.gridCount.y; c++)
                    {
                        Vector2 boxCenter = this.topLeft + new Vector2(this.boxSize.x * r, this.boxSize.y * c);
                        Gizmos.DrawWireCube(boxCenter, boxSize);
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
            this.Samples = new GameObject[(int)gridCount.x, (int)gridCount.y];
            this.boxSize = new Vector2(this.dimensions.x / this.gridCount.x, this.dimensions.y / this.gridCount.y);
            this.topLeft = new Vector2(this.center.position.x, this.center.position.y) - this.dimensions / 2f + this.boxSize / 2f;
        }

        protected override void LocalPlaceRooms()
        {
            for (int r = 0; r < this.gridCount.x; r++)
            {
                for (int c = 0; c < this.gridCount.y; c++)
                {
                    Vector2 boxCenter = this.topLeft + new Vector2(this.boxSize.x * r, this.boxSize.y * c);
                    Vector2 verticalBounds = new Vector2(boxCenter.y - this.boxSize.y / 2f, boxCenter.y + this.boxSize.y / 2f);
                    Vector2 horizontalBounds = new Vector2(boxCenter.x - this.boxSize.x / 2f, boxCenter.x + this.boxSize.x / 2f);
                    if (this.onlyCircles)
                        this.Samples[r, c] = RayCastDart(true, this.numberOfAttempts, verticalBounds, horizontalBounds);
                    else
                        this.Samples[r, c] = RayCastDart(Random.Range(0f, 1f) < .5f, this.numberOfAttempts, verticalBounds, horizontalBounds);
                }
            }
        }

        protected override void LocalClear()
        {
            this.Samples = new GameObject[(int)gridCount.x, (int)gridCount.y];
        }
    }
}
