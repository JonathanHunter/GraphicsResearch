﻿namespace GraphicsResearch.Util
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Lib
    {
        public enum Direction { Up, Down, Left, Right }

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

        public static Vector2 BoxLineIntersection(Vector3 p1, Vector3 p2, Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br)
        {
            Vector2 t = LineLineIntersection(p1, p2, tl, tr);
            if (t != Vector2.zero)
                return t;

            Vector2 b = LineLineIntersection(p1, p2, bl, br);
            if (b != Vector2.zero)
                return b;

            Vector2 l = LineLineIntersection(p1, p2, tl, bl);
            if (l != Vector2.zero)
                return l;

            Vector2 r = LineLineIntersection(p1, p2, tr, br);
            if (r != Vector2.zero)
                return r;

            return Vector2.zero;
        }

        public static Vector2 LineLineIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            float x = ((p1.x * p2.y - p1.y * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.y - p3.y * p4.x)) / 
                ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));
            float y = ((p1.x * p2.y - p1.y * p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x * p4.y - p3.y * p4.x)) / 
                ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));
            
            Vector2 ret = new Vector2(x, y);

            float dist = Vector2.Distance(p1, p2);

            if (Vector2.Distance(p1, ret) <=  dist &&
                Vector2.Distance(p2, ret) <= dist)
                return ret;

            return Vector2.zero;
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

        private static float Side(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }


        public static void AddTriangle(List<int> triangles, int a, int b, int c, bool invert)
        {
            if (invert)
            {
                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(c);
            }
            else
            {
                triangles.Add(c);
                triangles.Add(b);
                triangles.Add(a);
            }
        }

        public static void AddSquare(List<int> triangles, int a, int b, int c, int d, bool invert)
        {
            AddTriangle(triangles, a, b, c, invert);
            AddTriangle(triangles, d, a, c, invert);
        }

        public static void AddPentagon(List<int> triangles, int a, int b, int c, int d, int e, bool invert)
        {
            AddTriangle(triangles, e, a, b, invert);
            AddTriangle(triangles, e, b, c, invert);
            AddTriangle(triangles, e, c, d, invert);
        }
    }
}