namespace GraphicsResearch.DartThrowing
{
    using System.Collections.Generic;
    using UnityEngine;

    public class JitteredSampling : MonoBehaviour
    {
        [SerializeField]
        private bool showGrid;
        [SerializeField]
        private Transform center = null;
        [SerializeField]
        private Vector2 dimensions = Vector2.zero;
        [SerializeField]
        private Vector2 gridCount = Vector2.zero;
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
        public List<RectangleDart> Rects { get; private set; }
        
        private Vector2 boxSize;
        private Vector2 topLeft;

        private void Start()
        {
            this.Circles = new List<CircleDart>();
            this.Rects = new List<RectangleDart>();
            this.boxSize = new Vector2(this.dimensions.x / this.gridCount.x, this.dimensions.y / this.gridCount.y);
            this.topLeft = new Vector2(this.center.position.x, this.center.position.y) - this.dimensions / 2f + this.boxSize / 2f;

            for (int r = 0; r < this.gridCount.x; r++)
            {
                for (int c = 0; c < this.gridCount.y; c++)
                {
                    Vector2 boxCenter = this.topLeft + new Vector2(this.boxSize.x * r, this.boxSize.y * c);
                    if (this.onlyCircles)
                        Sample(true, boxCenter);
                    else
                        Sample(Random.Range(0f, 1f) < .5f, boxCenter);
                }
            }
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                Repulse();
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                foreach (CircleDart d in this.Circles)
                {
                    d.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
            }
        }

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

        private void Sample(bool isCircle, Vector2 boxCenter)
        {
            if(isCircle)
            {
                float x = Random.Range(boxCenter.x - this.boxSize.x / 2f, boxCenter.x + this.boxSize.x / 2f);
                float y = Random.Range(boxCenter.y - this.boxSize.y / 2f, boxCenter.y + this.boxSize.y / 2f);
                float size = this.resize ? Random.Range(this.sizeBound.x, this.sizeBound.y) : 1f;
                CircleDart circle = Instantiate<CircleDart>(this.circleTemplet);
                circle.transform.position = new Vector2(x, y);
                circle.OriginalPosition = circle.transform.position;
                circle.transform.localScale = Vector3.one * this.circleRadius * 2f * size;
                circle.Radius = this.circleRadius * size;
                this.Circles.Add(circle);
            }
            else
            {
                float x = Random.Range(boxCenter.x - this.boxSize.x / 2f, boxCenter.x + this.boxSize.x / 2f);
                float y = Random.Range(boxCenter.y - this.boxSize.y / 2f, boxCenter.y + this.boxSize.y / 2f);
                float sizeX = this.resize ? Random.Range(this.sizeBound.x, this.sizeBound.y) : 1f;
                float sizeY = this.resize ? Random.Range(this.sizeBound.x, this.sizeBound.y) : 1f;
                Quaternion rotation = this.rotate ? Quaternion.Euler(new Vector3(1, 1, Random.Range(0f, 90f))) : Quaternion.identity;
                RectangleDart rect = Instantiate<RectangleDart>(this.rectTemplet);
                rect.transform.position = new Vector2(x, y);
                rect.OriginalPosition = rect.transform.position;
                rect.transform.rotation = rotation;
                rect.transform.localScale = new Vector3(this.rectDimensions.x * sizeX, this.rectDimensions.y * sizeY, 1);
                rect.Dimentions = this.rectDimensions;
                this.Rects.Add(rect);
            }
        }

        private void Repulse()
        {
            foreach (CircleDart d in this.Circles)
            {
                Collider[] things = Physics.OverlapSphere(d.transform.position, d.Radius * 2f);
                foreach (Collider c in things)
                {
                    if (c.gameObject != d.gameObject && c.gameObject.GetComponent<CircleDart>() != null)
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
                    if (c.gameObject != d.gameObject && c.gameObject.GetComponent<CircleDart>() != null)
                    {
                        Vector3 ray = c.gameObject.transform.position - d.transform.position;
                        c.gameObject.GetComponent<Rigidbody>().AddForce(ray.normalized * (1f / ray.magnitude));
                    }
                }
            }
        }
    }
}
