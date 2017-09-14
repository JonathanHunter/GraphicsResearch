namespace GraphicsResearch.DartThrowing
{
    using UnityEngine;

    public class RectangleDart : MonoBehaviour
    {
        [SerializeField]
        private Vector2 dimentions;

        public Vector2 Dimentions { get { return this.dimentions; } internal set { this.dimentions = value; } }

        public Vector3 OriginalPosition { get; set; }

        public bool IsOverlapping(RectangleDart rect)
        {


            return false;
        }

        //private bool IsOverlappingDim(float min1, float max1, float min2, float max2)
        //{
        //}

        //public bool IsOverlapping(CircleDart circle)
        //{
        //    Vector3 edge = Vector3.MoveTowards(circle.transform.position, this.transform.position, circle.Radius);
        //    float dist = Vector3.Distance(edge, this.transform.position);
        //    if (dist < this.Radius)
        //        return true;

        //    edge = Vector3.MoveTowards(this.transform.position, circle.transform.position, this.Radius);
        //    dist = Vector3.Distance(edge, circle.transform.position);
        //    if (dist < circle.Radius)
        //        return true;

        //    return false;
        //}
    }
}
