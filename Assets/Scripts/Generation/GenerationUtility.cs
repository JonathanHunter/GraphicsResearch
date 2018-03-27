namespace GraphicsResearch.Generation
{
    using UnityEngine;

    public class GenerationUtility
    {
        /// <summary> Returns the intersection point of a line and a circle. </summary>
        /// <param name="begin"> The start of the line. </param>
        /// <param name="end"> the end of the line. </param>
        /// <param name="center"> The center of the circle. </param>
        /// <param name="radius"> The radius of the circle. </param>
        /// <returns> The position of intesection. </returns>
        public static Vector2 CircleLineIntersection(Vector3 begin, Vector3 end, Vector3 center, float radius)
        {
            float x1 = begin.x - center.x;
            float y1 = begin.y - center.y;
            float x2 = end.x - center.x;
            float y2 = end.y - center.y;
            float dx = x2 - x1;
            float dy = y2 - y1;
            float dr = Mathf.Sqrt(dx * dx + dy * dy);
            float D = x1 * y2 - x2 * y1;
            float incedent = radius * radius * dr * dr - D * D;
            if (incedent < 0)
                return Vector2.zero;

            float rx1 = (D * dy + Mathf.Sign(dy) * dx * Mathf.Sqrt(incedent)) / (dr * dr) + center.x;
            float ry1 = (-D * dx + Mathf.Abs(dy) * Mathf.Sqrt(incedent)) / (dr * dr) + center.y;
            float rx2 = (D * dy - Mathf.Sign(dy) * dx * Mathf.Sqrt(incedent)) / (dr * dr) + center.x;
            float ry2 = (-D * dx - Mathf.Abs(dy) * Mathf.Sqrt(incedent)) / (dr * dr) + center.y;

            Vector2 centerPoint = Vector2.Lerp(begin, end, .5f);
            Vector2 a = new Vector2(rx1, ry1);
            Vector2 b = new Vector2(rx2, ry2);

            if (Vector2.Distance(centerPoint, a) < Vector2.Distance(centerPoint, b))
                return a;
            else
                return b;
        }

        /// <summary> Returns the intersection point of a line and box. </summary>
        /// <param name="begin"> The start of the line. </param>
        /// <param name="end"> the end of the line. </param>
        /// <param name="tl"> The top left of the box. </param>
        /// <param name="tr"> The top right of the box. </param>
        /// <param name="bl"> The bottom left of the box. </param>
        /// <param name="br"> the bottom right of the box. </param>
        /// <returns> The position of intesection. </returns>
        public static Vector2 BoxLineIntersection(Vector3 begin, Vector3 end, Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br)
        {
            Vector2 t = LineLineIntersection(begin, end, tl, tr);
            if (t != Vector2.zero)
                return t;

            Vector2 b = LineLineIntersection(begin, end, bl, br);
            if (b != Vector2.zero)
                return b;

            Vector2 l = LineLineIntersection(begin, end, tl, bl);
            if (l != Vector2.zero)
                return l;

            Vector2 r = LineLineIntersection(begin, end, tr, br);
            if (r != Vector2.zero)
                return r;

            return Vector2.zero;
        }

        /// <summary> Returns the intersection point between two lines. </summary>
        /// <param name="p1"> A point on line 1. </param>
        /// <param name="p2"> A point on line 1. </param>
        /// <param name="p3"> A point on line 2. </param>
        /// <param name="p4"> A point on line 2. </param>
        /// <returns> The position of intesection. </returns>
        public static Vector2 LineLineIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            float x = ((p1.x * p2.y - p1.y * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.y - p3.y * p4.x)) /
                ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));
            float y = ((p1.x * p2.y - p1.y * p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x * p4.y - p3.y * p4.x)) /
                ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));

            return new Vector2(x, y);
        }

        public static bool PointInCircle(Vector3 point, Vector3 center, float radius)
        {
            return Vector3.Distance(point, center) <= radius;
        }

        public static bool PointInBox(Vector3 point, Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br)
        {
            bool b1 = Side(point, tl, tr) <= 0f;
            bool b2 = Side(point, tr, br) <= 0f;
            bool b3 = Side(point, br, bl) <= 0f;
            bool b4 = Side(point, bl, tl) <= 0f;

            return ((b1 == b2) && (b2 == b3) && (b3 == b4));
        }

        /// <summary> Checks if the given line and hollow circle are intersecting. </summary>
        /// <param name="begin"> The start of the line. </param>
        /// <param name="end"> the end of the line. </param>
        /// <param name="center"> The center of the circle. </param>
        /// <param name="radius"> The radius of the circle. </param>
        /// <returns> True if the line is intersecting. </returns>
        public static bool CheckCircleLineIntersection(Vector3 begin, Vector3 end, Vector3 center, float radius)
        {
            if (PointInCircle(begin, center, radius) != PointInCircle(end, center, radius))
                return true;

            float x1 = begin.x - center.x;
            float y1 = begin.y - center.y;
            float x2 = end.x - center.x;
            float y2 = end.y - center.y;
            float dx = x2 - x1;
            float dy = y2 - y1;
            float dr = Mathf.Sqrt(dx * dx + dy * dy);
            float D = x1 * y2 - x2 * y1;
            float incedent = radius * radius * dr * dr - D * D;
            if (incedent < 0)
                return false;

            float rx1 = (D * dy + Mathf.Sign(dy) * dx * Mathf.Sqrt(incedent)) / (dr * dr) + center.x;
            float ry1 = (-D * dx + Mathf.Abs(dy) * Mathf.Sqrt(incedent)) / (dr * dr) + center.y;
            float rx2 = (D * dy - Mathf.Sign(dy) * dx * Mathf.Sqrt(incedent)) / (dr * dr) + center.x;
            float ry2 = (-D * dx - Mathf.Abs(dy) * Mathf.Sqrt(incedent)) / (dr * dr) + center.y;

            Vector2 centerPoint = Vector2.Lerp(begin, end, .5f);
            Vector2 a = new Vector2(rx1, ry1);
            Vector2 b = new Vector2(rx2, ry2);
            float dist = Vector2.Distance(begin, end);
            if (Vector2.Distance(begin, a) <= dist &&
                Vector2.Distance(end, a) <= dist)
                return true;

            if (Vector2.Distance(begin, b) <= dist &&
                Vector2.Distance(end, b) <= dist)
                return true;

            return false;
        }

        /// <summary> Checks if the given line and hollow box are intersecting. </summary>
        /// <param name="begin"> The start of the line. </param>
        /// <param name="end"> the end of the line. </param>
        /// <param name="tl"> The top left of the box. </param>
        /// <param name="tr"> The top right of the box. </param>
        /// <param name="bl"> The bottom left of the box. </param>
        /// <param name="br"> the bottom right of the box. </param>
        /// <returns> True if the line is intersecting. </returns>
        public static bool CheckBoxLineIntersection(Vector3 begin, Vector3 end, Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br)
        {
            if (PointInBox(begin, tl, tr, bl, br) != PointInBox(end, tl, tr, bl, br))
                return true;

            if (CheckLineLineIntersection(begin, end, tl, tr))
                return true;
            
            if (CheckLineLineIntersection(begin, end, bl, br))
                return true;
            
            if (CheckLineLineIntersection(begin, end, tl, bl))
                return true;
            
            if (CheckLineLineIntersection(begin, end, tr, br))
                return true;

            return false;
        }

        /// <summary> Checks if 2 lines are intersecting. </summary>
        /// <param name="p1"> A point on line 1. </param>
        /// <param name="p2"> A point on line 1. </param>
        /// <param name="p3"> A point on line 2. </param>
        /// <param name="p4"> A point on line 2. </param>
        /// <returns> True if the lines intertect. </returns>
        public static bool CheckLineLineIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            float x = ((p1.x * p2.y - p1.y * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.y - p3.y * p4.x)) /
                ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));
            float y = ((p1.x * p2.y - p1.y * p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x * p4.y - p3.y * p4.x)) /
                ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));

            Vector2 ret = new Vector2(x, y);

            float dist = Vector2.Distance(p1, p2);
            float dist2 = Vector2.Distance(p3, p4);

            if (Vector2.Distance(p1, ret) <= dist &&
                Vector2.Distance(p2, ret) <= dist &&
                Vector2.Distance(p3, ret) <= dist2 &&
                Vector2.Distance(p4, ret) <= dist2)
                return true;

            return false;
        }

        private static float Side(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        public static float PercentBetween(Vector2 start, Vector2 end, Vector2 point)
        {
            float totalDist = Vector2.Distance(start, end);
            float dist = Vector2.Distance(start, point);
            float percent = dist / totalDist;
            if (percent < .2f)
                return 0;
            if (percent > .8f)
                return 1;

            return (Mathf.Cos((5f * percent) + (.7f * Mathf.PI)) + 1f) / 2f;
            //return (Mathf.Pow((2.5f * percent - 1.25f), 3f) + 1f) / 2f;
        }

        public static void BoxBounds(Vector3 start, Vector3 end, float width, out Vector3 tl, out Vector3 tr, out Vector3 bl, out Vector3 br)
        {
            Vector3 es = Vector3.Normalize(start - end);
            Vector3 left = new Vector3(-es.y, es.x, es.z);
            tl = start + left * width / 2f;
            tr = start - left * width / 2f;
            bl = end + left * width / 2f;
            br = end - left * width / 2f;
        }
    }
}
