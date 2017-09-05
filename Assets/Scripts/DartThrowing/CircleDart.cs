namespace GraphicsResearch.DartThrowing
{
    using UnityEngine;

    public class CircleDart : MonoBehaviour
    {
        [SerializeField]
        private float radius;

        public float Radius { get { return this.radius; } internal set { this.radius = value; } }

        public bool IsOverlapping(CircleDart circle)
        {
            Vector3 edge = Vector3.MoveTowards(circle.transform.position, this.transform.position, circle.Radius);
            float dist = Vector3.Distance(edge, this.transform.position);
            if (dist < this.Radius)
                return true;

            edge = Vector3.MoveTowards(this.transform.position, circle.transform.position, this.Radius);
            dist = Vector3.Distance(edge, circle.transform.position);
            if (dist < circle.Radius)
                return true;

            return false;
        }

        //public bool IsOverlapping(RectangleDart rect)
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
