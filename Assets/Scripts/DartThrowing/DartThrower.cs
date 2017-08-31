namespace GraphicsResearch.DartThrowing
{
    using System.Collections.Generic;
    using UnityEngine;

    public class DartThrower : MonoBehaviour
    {
        [SerializeField]
        private Transform center;
        [SerializeField]
        private Vector2 dimensions;
        [SerializeField]
        private int numberOfDarts;
        [SerializeField]
        private int numberOfAttempts;
        [SerializeField]
        private float circleRadius;
        [SerializeField]
        private CircleDart circleTemplet;

        private float upBound;
        private float downBound;
        private float leftBound;
        private float rightBound;
        private List<CircleDart> circles;

        private void Start()
        {
            this.upBound = center.position.y + dimensions.y / 2;
            this.downBound = center.position.y - dimensions.y / 2;
            this.leftBound = center.position.x - dimensions.x / 2;
            this.rightBound = center.position.x + dimensions.x / 2;
            this.circles = new List<CircleDart>();

            for(int i = 0; i < numberOfDarts; i++)
            {
                ThrowDart();
            }
        }

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.C))
            {
                ThrowDart();
            }
        }

        private void ThrowDart()
        {
            CircleDart circle = Instantiate<CircleDart>(this.circleTemplet);
            circle.transform.localScale = Vector3.one * this.circleRadius * 2f;
            circle.Radius = this.circleRadius;
            int currentAttempt = 0;
            while(currentAttempt < this.numberOfAttempts)
            {
                float x = Random.Range(leftBound, rightBound);
                float y = Random.Range(downBound, upBound);
                circle.transform.position = new Vector2(x, y);
                bool overlaps = false;
                foreach(CircleDart cd in this.circles)
                {
                    if(cd.IsOverlapping(circle))
                    {
                        overlaps = true;
                    }
                }

                if(!overlaps)
                {
                    this.circles.Add(circle);
                    return;
                }

                currentAttempt++;
            }

            Debug.Log("Unable to place dart after " + currentAttempt + " attempts.");
            Destroy(circle.gameObject);
        }
    }
}
