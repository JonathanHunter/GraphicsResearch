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
        [SerializeField]
        private bool jitteredSampling;
        [SerializeField]
        private Vector2 gridCount = Vector2.zero;
        [SerializeField]
        [Range(0f, 1f)]
        private float pullToCenter;
        [SerializeField]
        private bool showGrid;

        public List<CircleDart> Circles { get; private set; }
        public List<RectangleDart> Rects { get; private set; }
        public GameObject[,] Samples { get; private set; }

        private Vector2 globalVerticalBounds;
        private Vector2 globalHorizontalBounds;
        private Vector2 boxSize;
        private Vector2 topLeft;

        private void Start()
        {
            this.Circles = new List<CircleDart>();
            this.Rects = new List<RectangleDart>();
            this.Samples = new GameObject[(int)gridCount.x, (int)gridCount.y];
            this.globalVerticalBounds = new Vector2(this.center.position.y + this.dimensions.y / 2, this.center.position.y - this.dimensions.y / 2);
            this.globalHorizontalBounds = new Vector2(this.center.position.x - this.dimensions.x / 2, this.center.position.x + this.dimensions.x / 2);
            this.boxSize = new Vector2(this.dimensions.x / this.gridCount.x, this.dimensions.y / this.gridCount.y);
            this.topLeft = new Vector2(this.center.position.x, this.center.position.y) - this.dimensions / 2f + this.boxSize / 2f;
            if (!jitteredSampling)
            {
                for (int i = 0; i < numberOfDarts; i++)
                {
                    if (this.onlyCircles)
                        RayCastDart(true, this.globalVerticalBounds, this.globalHorizontalBounds);
                    else
                        RayCastDart(Random.Range(0f, 1f) < .5f, this.globalVerticalBounds, this.globalHorizontalBounds);
                }
            }
            else
            {
                for (int r = 0; r < this.gridCount.x; r++)
                {
                    for (int c = 0; c < this.gridCount.y; c++)
                    {
                        Vector2 boxCenter = this.topLeft + new Vector2(this.boxSize.x * r, this.boxSize.y * c);
                        Vector2 verticalBounds = new Vector2(boxCenter.y - this.boxSize.y / 2f, boxCenter.y + this.boxSize.y / 2f);
                        Vector2 horizontalBounds = new Vector2(boxCenter.x - this.boxSize.x / 2f, boxCenter.x + this.boxSize.x / 2f);
                        if (this.onlyCircles)
                            this.Samples[r,c] = RayCastDart(true, verticalBounds, horizontalBounds);
                        else
                            this.Samples[r, c] = RayCastDart(Random.Range(0f, 1f) < .5f, verticalBounds, horizontalBounds);
                    }
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                RayCastDart(true, this.globalVerticalBounds, this.globalHorizontalBounds);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                RayCastDart(false, this.globalVerticalBounds, this.globalHorizontalBounds);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                Repulse();
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                foreach (CircleDart d in this.Circles)
                {
                    d.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    d.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }

                foreach (RectangleDart d in this.Rects)
                {
                    d.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    d.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }
            }
            else if (Input.GetKeyUp(KeyCode.J))
            {
                for (int r = 0; r < this.gridCount.x; r++)
                {
                    for (int c = 0; c < this.gridCount.y; c++)
                    {
                        Vector2 boxCenter = this.topLeft + new Vector2(this.boxSize.x * r, this.boxSize.y * c);
                        if (this.Samples[r, c].GetComponent<CircleDart>() != null)
                        {
                            CircleDart circle = this.Samples[r, c].GetComponent<CircleDart>();
                            circle.transform.position = Vector3.Lerp(circle.OriginalPosition, boxCenter, this.pullToCenter);
                        }
                        else
                        {
                            RectangleDart rect = this.Samples[r, c].GetComponent<RectangleDart>();
                            rect.transform.position = Vector3.Lerp(rect.OriginalPosition, boxCenter, this.pullToCenter);
                        }
                    }
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                foreach(CircleDart c in this.Circles)
                    c.gameObject.SetActive(!c.gameObject.activeSelf);

                foreach(RectangleDart r in this.Rects)
                    r.gameObject.SetActive(!r.gameObject.activeSelf);
            }
        }

        private void OnDrawGizmos()
        {
            if (this.showGrid)
            {
                for (int r = 0; r < this.gridCount.x; r++)
                {
                    for (int c = 0; c < this.gridCount.y; c++)
                    {
                        Vector2 boxCenter = this.topLeft + new Vector2(this.boxSize.x * r, this.boxSize.y * c);
                        Gizmos.DrawWireCube(boxCenter, boxSize);
                    }
                }
            }
        }

        private GameObject RayCastDart(bool isCircle, Vector2 verticalBounds, Vector2 horizontalBounds)
        {
            int currentAttempt = 0;
            if (isCircle)
            {
                while (currentAttempt < this.numberOfAttempts)
                {
                    float x = Random.Range(horizontalBounds.x, horizontalBounds.y);
                    float y = Random.Range(verticalBounds.y, verticalBounds.x);
                    float size = this.resize? Random.Range(this.sizeBound.x, this.sizeBound.y) : 1f;
                    Collider[] overlaps = Physics.OverlapSphere(new Vector2(x, y), this.circleRadius * size);
                    if (overlaps.Length == 0)
                    {
                        CircleDart circle = Instantiate<CircleDart>(this.circleTemplet);
                        circle.transform.position = new Vector2(x, y);
                        circle.OriginalPosition = circle.transform.position;
                        circle.transform.localScale = Vector3.one * this.circleRadius * 2f * size;
                        circle.Radius = this.circleRadius * size;
                        this.Circles.Add(circle);
                        return circle.gameObject;
                    }

                    currentAttempt++;
                }
            }
            else
            {
                while (currentAttempt < this.numberOfAttempts)
                {
                    float x = Random.Range(horizontalBounds.x, horizontalBounds.y);
                    float y = Random.Range(verticalBounds.y, verticalBounds.x);
                    float sizeX = this.resize ? Random.Range(this.sizeBound.x, this.sizeBound.y) : 1f;
                    float sizeY = this.resize ? Random.Range(this.sizeBound.x, this.sizeBound.y) : 1f;
                    Quaternion rotation = this.rotate ? Quaternion.Euler(new Vector3(1, 1, Random.Range(0f, 90f))) : Quaternion.identity;
                    Collider[] overlaps = Physics.OverlapBox(new Vector2(x, y), new Vector3(this.rectDimensions.x * sizeX, this.rectDimensions.y * sizeY, 1) / 2f, rotation);
                    if (overlaps.Length == 0)
                    {
                        RectangleDart rect = Instantiate<RectangleDart>(this.rectTemplet);
                        rect.transform.position = new Vector2(x, y);
                        rect.OriginalPosition = rect.transform.position;
                        rect.transform.rotation = rotation;
                        rect.transform.localScale = new Vector3(this.rectDimensions.x * sizeX, this.rectDimensions.y * sizeY, 1);
                        rect.Dimentions = this.rectDimensions;
                        this.Rects.Add(rect);
                        return rect.gameObject;
                    }

                    currentAttempt++;
                }
            }

            Debug.Log("Unable to place dart after " + currentAttempt + " attempts.");
            return null;
        }

        private void Repulse()
        {
            foreach (CircleDart d in this.Circles)
            {
                Collider[] things = Physics.OverlapSphere(d.transform.position, d.Radius * 2f);
                foreach (Collider c in things)
                {
                    if (c.gameObject != d.gameObject && 
                        (c.gameObject.GetComponent<CircleDart>() != null || c.gameObject.GetComponent<RectangleDart>() != null))
                    {
                        Vector3 ray = c.gameObject.transform.position - d.transform.position;
                        c.gameObject.GetComponent<Rigidbody>().AddForce(ray.normalized * (1f / ray.magnitude));
                    }
                }
            }
            foreach (RectangleDart d in this.Rects)
            {
                Collider[] things = Physics.OverlapBox(d.transform.position, d.Dimentions * 2f);
                foreach (Collider c in things)
                {
                    if (c.gameObject != d.gameObject &&
                        (c.gameObject.GetComponent<CircleDart>() != null || c.gameObject.GetComponent<RectangleDart>() != null))
                    {
                        Vector3 ray = c.gameObject.transform.position - d.transform.position;
                        c.gameObject.GetComponent<Rigidbody>().AddForce(ray.normalized * (1f / ray.magnitude));
                    }
                }
            }
        }
    }
}
