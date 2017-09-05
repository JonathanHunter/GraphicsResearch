namespace GraphicsResearch.DartThrowing
{
    using System.Collections.Generic;
    using UnityEngine;

    public class DartThrower : MonoBehaviour
    {
        [SerializeField]
        private Transform center = null;
        [SerializeField]
        private Vector2 dimensions = Vector2.zero;
        [SerializeField]
        private int numberOfDarts = 0;
        [SerializeField]
        private int numberOfAttempts = 0;
        [SerializeField]
        private float circleRadius = 0;
        [SerializeField]
        private CircleDart circleTemplet = null;
        [SerializeField]
        private Vector2 rectDimensions = Vector2.zero;
        [SerializeField]
        private RectangleDart rectTemplet = null;
        [SerializeField]
        private bool rotate = false;
        [SerializeField]
        private bool resize = false;
        [SerializeField]
        private Vector2 sizeBound = Vector2.zero;
        [SerializeField]
        private bool onlyCircles;
        
        public List<CircleDart> Circles { get; private set; }

        private float upBound;
        private float downBound;
        private float leftBound;
        private float rightBound;
        private List<RectangleDart> rects;

        private void Start()
        {
            this.upBound = center.position.y + dimensions.y / 2;
            this.downBound = center.position.y - dimensions.y / 2;
            this.leftBound = center.position.x - dimensions.x / 2;
            this.rightBound = center.position.x + dimensions.x / 2;
            this.Circles = new List<CircleDart>();
            this.rects = new List<RectangleDart>();

            for(int i = 0; i < numberOfDarts; i++)
            {
                if(this.onlyCircles)
                    RayCastDart(true);
                else
                    RayCastDart(Random.Range(0f, 1f) < .5f);
            }
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.O))
            {
                ThrowDart();
            }
            else if (Input.GetKeyUp(KeyCode.C))
            {
                RayCastDart(true);
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                RayCastDart(false);
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
                foreach(CircleDart cd in this.Circles)
                {
                    if(cd.IsOverlapping(circle))
                    {
                        overlaps = true;
                    }
                }

                if(!overlaps)
                {
                    this.Circles.Add(circle);
                    return;
                }

                currentAttempt++;
            }

            Debug.Log("Unable to place dart after " + currentAttempt + " attempts.");
            Destroy(circle.gameObject);
        }

        private void RayCastDart(bool isCircle)
        {
            if(isCircle)
            {
                int currentAttempt = 0;
                while (currentAttempt < this.numberOfAttempts)
                {
                    float x = Random.Range(leftBound, rightBound);
                    float y = Random.Range(downBound, upBound);
                    float size = this.resize? Random.Range(this.sizeBound.x, this.sizeBound.y) : 1f;
                    Collider[] overlaps = Physics.OverlapSphere(new Vector2(x, y), this.circleRadius * size);
                    if (overlaps.Length == 0)
                    {
                        CircleDart circle = Instantiate<CircleDart>(this.circleTemplet);
                        circle.transform.position = new Vector2(x, y);
                        circle.transform.localScale = Vector3.one * this.circleRadius * 2f * size;
                        circle.Radius = this.circleRadius * size;
                        this.Circles.Add(circle);
                        return;
                    }

                    currentAttempt++;
                }

                Debug.Log("Unable to place dart after " + currentAttempt + " attempts.");
            }
            else
            {
                int currentAttempt = 0;
                while (currentAttempt < this.numberOfAttempts)
                {
                    float x = Random.Range(leftBound, rightBound);
                    float y = Random.Range(downBound, upBound);
                    float sizeX = this.resize ? Random.Range(this.sizeBound.x, this.sizeBound.y) : 1f;
                    float sizeY = this.resize ? Random.Range(this.sizeBound.x, this.sizeBound.y) : 1f;
                    Quaternion rotation = this.rotate ? Quaternion.Euler(new Vector3(1, 1, Random.Range(0f, 90f))) : Quaternion.identity;
                    Collider[] overlaps = Physics.OverlapBox(new Vector2(x, y), new Vector3(this.rectDimensions.x * sizeX, this.rectDimensions.y * sizeY, 1) / 2f, rotation);
                    if (overlaps.Length == 0)
                    {
                        RectangleDart rect = Instantiate<RectangleDart>(this.rectTemplet);
                        rect.transform.position = new Vector2(x, y);
                        rect.transform.rotation = rotation;
                        rect.transform.localScale = new Vector3(this.rectDimensions.x * sizeX, this.rectDimensions.y * sizeY, 1);
                        rect.Dimentions = this.rectDimensions;
                        this.rects.Add(rect);
                        return;
                    }

                    currentAttempt++;
                }

                Debug.Log("Unable to place dart after " + currentAttempt + " attempts.");
            }
        }
    }
}
