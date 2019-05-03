/* Copyright 2019 Marc Baloup, Géry Casiez, Thomas Pietrzak
               (Université de Lille, Inria, France)

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Point = UnityEngine.Vector3;
using Line = UnityEngine.Ray;
using Sphere = UnityEngine.BoundingSphere;
using System;

namespace RayCursor
{
    public static class DistanceUtil
    {
        /// <summary>Calcule la distance euclidienne entre les deux points</summary>
        public static float Dist(Point p1, Point p2)
        {
            return (p2 - p1).magnitude;
        }


        /// <summary>Calcule la plus petite distance entre la droite et le point</summary>
        public static float Dist(Point p, Line l) { return Dist(l, p); }
        /// <summary>Calcule la plus petite distance entre la droite et le point</summary>
        public static float Dist(Line l, Point p)
        {
            return Point.Cross((p - l.origin), l.direction).magnitude / l.direction.magnitude;
        }

        /// <summary>Calcule distance minimum entre les deux droites</summary>
        public static float Dist(Line x, Line y)
        {
            Point n = Point.Cross(x.direction.normalized, y.direction.normalized);
            if (n.magnitude == 0) // if parallel
                return Dist(x, y.origin);
            return Mathf.Abs(Point.Dot(n.normalized, (y.origin - x.origin)));
        }



        /// <summary>Calcule la distance signée entre un plan et un point. Le signe dépend du côté duquel se trouve le point par rapport au plan.</summary>
        public static float DistSigned(Point pt, Plane pl) { return DistSigned(pl, pt); }
        /// <summary>Calcule la distance signée entre un plan et un point. Le signe dépend du côté duquel se trouve le point par rapport au plan.</summary>
        public static float DistSigned(Plane pl, Point pt)
        {
            return pl.GetDistanceToPoint(pt);
        }

        /// <summary>Calcule la distance absolue entre un plan et un point</summary>
        public static float Dist(Point pt, Plane pl) { return Dist(pl, pt); }
        /// <summary>Calcule la distance absolue entre un plan et un point</summary>
        public static float Dist(Plane pl, Point pt)
        {
            return Mathf.Abs(DistSigned(pl, pt));
        }


        /// <summary>Calcule la distance signée entre une droite et un plan. Le signe dépend du côté duquel se trouve la droite par rapport au plan. Vaut 0 si ils ne sont pas parallèles.</summary>
        public static float DistSigned(Line l, Plane p) { return DistSigned(p, l); }
        /// <summary>Calcule la distance signée entre une droite et un plan. Le signe dépend du côté duquel se trouve la droite par rapport au plan. Vaut 0 si ils ne sont pas parallèles.</summary>
        public static float DistSigned(Plane p, Line l)
        {
            Point d = l.direction, n = p.normal;
            if (Point.Dot(d, n) == 0) // plane parallel to ray
                return DistSigned(p, l.origin);
            return 0;
        }

        /// <summary>Calcule la distance absolue entre une droite et un plan. Vaut 0 si ils ne sont pas parallèles.</summary>
        public static float Dist(Line l, Plane p) { return Dist(p, l); }
        /// <summary>Calcule la distance absolue entre une droite et un plan. Vaut 0 si ils ne sont pas parallèles.</summary>
        public static float Dist(Plane p, Line l)
        {
            return Math.Abs(DistSigned(p, l));
        }

        /// <summary>Calcule la distance signée entre deux plans. Le signe dépend du côté duquel se trouve le plan p2 par rapport au plan p1.</summary>
        public static float DistSigned(Plane p1, Plane p2)
        {
            if (Point.Cross(p1.normal, p2.normal).magnitude == 0) // parallel
                return DistSigned(p1, p2.ClosestPointOnPlane(Point.zero));
            return 0;
        }

        /// <summary>Calcule la distance entre deux plans. Vaut 0 si ils ne sont pas parallèles.</summary>
        public static float Dist(Plane p1, Plane p2)
        {
            return Mathf.Abs(DistSigned(p1, p2));
        }


        /// <summary>Calcule la distance entre un point et une sphère. Peut être négatif : entre 0 si c'est sur la surface, et -rayon si le point est au centre de la sphère.</summary>
        public static float Dist(Point p, Sphere s) { return Dist(s, p); }
        /// <summary>Calcule la distance entre un point et une sphère. Peut être négatif : entre 0 si c'est sur la surface, et -rayon si le point est au centre de la sphère.</summary>
        public static float Dist(Sphere s, Point p)
        {
            return Dist(s.position, p) - s.radius;
        }

        /// <summary>Calcule la distance entre une droite et une sphère. Peut être négatif : entre 0 si la droite est tangeante à la surface de la sphère, et -rayon si la droite traverse le centre de la sphère.</summary>
        public static float Dist(Line l, Sphere s) { return Dist(s, l); }
        /// <summary>Calcule la distance entre une droite et une sphère. Peut être négatif : entre 0 si la droite est tangeante à la surface de la sphère, et -rayon si la droite traverse le centre de la sphère.</summary>
        public static float Dist(Sphere s, Line l)
        {
            return Dist(s.position, l) - s.radius;
        }

        /// <summary>Calcule la distance entre un plan et une sphère. Peut être négatif : entre 0 si le plan est tangeant à la surface de la sphère, et -rayon si le centre de la sphère est sur le plan.</summary>
        public static float Dist(Plane p, Sphere s) { return Dist(s, p); }
        /// <summary>Calcule la distance entre un plan et une sphère. Peut être négatif : entre 0 si le plan est tangeant à la surface de la sphère, et -rayon si le centre de la sphère est sur le plan.</summary>
        public static float Dist(Sphere s, Plane p)
        {
            return Dist(s.position, p) - s.radius;
        }

        /// <summary>Calcule la distance minimum entre deux sphère. Peut être négatif : entre 0 si les deux sphères n'ont qu'un seul point en commun, et -(s1.rayon + s2.rayon) si les deux centres sont confondus.</summary>
        public static float Dist(Sphere s1, Sphere s2)
        {
            return Dist(s1.position, s2.position) - s1.radius - s2.radius;
        }



        public static Sphere ToSphere(SphereCollider s)
        {
            return new Sphere(s.transform.position + s.transform.TransformVector(s.center), s.radius * s.transform.localScale.x);
        }

        /// <summary>Calcule la distance entre un point et une sphère. Si c'est négatif, indique la profondeur du point dans la sphère.</summary>
        public static float Dist(Point p, SphereCollider s) { return Dist(s, p); }
        /// <summary>Calcule la distance entre un point et une sphère. Si c'est négatif, indique la profondeur du point dans la sphère.</summary>
        public static float Dist(SphereCollider s, Point p) { return Dist(ToSphere(s), p); }

        /// <summary>Calcule la distance entre une droite et une sphère. Si c'est négatif, indique la profondeur de la droite dans la sphère.</summary>
        public static float Dist(Line l, SphereCollider s) { return Dist(s, l); }
        /// <summary>Calcule la distance entre une droite et une sphère. Si c'est négatif, indique la profondeur de la droite dans la sphère.</summary>
        public static float Dist(SphereCollider s, Line l) { return Dist(ToSphere(s), l);}

        /// <summary>Calcule la distance entre un plan et une sphère. Peut être négatif : entre 0 si le plan est tangeant à la surface de la sphère, et -rayon si le centre de la sphère est sur le plan.</summary>
        public static float Dist(Plane p, SphereCollider s) { return Dist(s, p); }
        /// <summary>Calcule la distance entre un plan et une sphère. Peut être négatif : entre 0 si le plan est tangeant à la surface de la sphère, et -rayon si le centre de la sphère est sur le plan.</summary>
        public static float Dist(SphereCollider s, Plane p) { return Dist(ToSphere(s), p); }

        /// <summary>Calcule la distance minimum entre deux sphère. Peut être négatif : entre 0 si les deux sphères n'ont qu'un seul point en commun, et -(s1.rayon + s2.rayon) si les deux centres sont confondus.</summary>
        public static float Dist(SphereCollider s1, SphereCollider s2) { return Dist(ToSphere(s2), ToSphere(s2)); }



        /// <summary>Calcule la distance minimum entre un plan et un tableau de point composant un objet. Si c'est nagatif, indique la hauteur du plus petit morceaux de l'objet coupé par le plan.</summary>
        public static float Dist(Plane p, Point[] pts) { return Dist(pts, p); }
        /// <summary>Calcule la distance minimum entre un plan et un tableau de point composant un objet. Si c'est nagatif, indique la hauteur du plus petit morceaux de l'objet coupé par le plan.</summary>
        public static float Dist(Point[] pts, Plane p)
        {
            float minDist, maxDist;
            minDist = maxDist = DistSigned(p, pts[0]);
            for (int i = 1; i < pts.Length; i++)
            {
                float d = DistSigned(p, pts[i]);
                if (d < minDist)
                    minDist = d;
                if (d > maxDist)
                    maxDist = d;
            }

            if (minDist < 0 && maxDist < 0)
                return -maxDist; // dont collide and all box in negative side
            if (minDist >= 0 && maxDist >= 0)
                return minDist; // dont collide and all box in positive side
            return Mathf.Max(minDist, -maxDist); // collide, get littlest part cutted by plane
        }





        /// <summary>Calcule la distance minimum entre un point et une boite. Si c'est négatif, indique la profondeur du point dans la boite.</summary>
        public static float Dist(Point p, BoxCollider b) { return Dist(b, p); }
        /// <summary>Calcule la distance minimum entre un point et une boite. Si c'est négatif, indique la profondeur du point dans la boite.</summary>
        public static float Dist(BoxCollider b, Point p)
        {
            float d = Dist(p, b.ClosestPoint(p));
            if (d < 0.000001)
            {   // p inside b
                float maxD = float.MinValue;
                foreach (Plane pl in b.GetPlanes())
                {
                    float currD = DistSigned(pl, p);
                    if (currD > maxD)
                        maxD = currD;
                }
                d = Mathf.Min(0, maxD);
            }
            return d;
        }

        /// <summary>Calcule la distance minimum entre un plan et une boite. Si c'est nagatif, indique la hauteur du plus petit morceaux de la boite coupé par le plan.</summary>
        public static float Dist(Plane p, BoxCollider b) { return Dist(b, p); }
        /// <summary>Calcule la distance minimum entre un plan et une boite. Si c'est nagatif, indique la hauteur du plus petit morceaux de la boite coupé par le plan.</summary>
        public static float Dist(BoxCollider b, Plane p)
        {
            return Dist(b.GetVertices(), p);
        }

        /// <summary>Calcule la distance minimum entre une sphère et une boite. Si c'est nagatif, indique la profondeur de la sphère dans la boite.</summary>
        public static float Dist(Sphere s, BoxCollider b) { return Dist(b, s); }
        /// <summary>Calcule la distance minimum entre une sphère et une boite. Si c'est nagatif, indique la profondeur de la sphère dans la boite.</summary>
        public static float Dist(BoxCollider b, Sphere s)
        {
            return Dist(b, s.position) - s.radius;
        }

        /// <summary>Calcule la distance minimum entre une sphère et une boite. Si c'est nagatif, indique la profondeur de la sphère dans la boite.</summary>
        public static float Dist(BoxCollider b, SphereCollider s) { return Dist(s, b); }
        /// <summary>Calcule la distance minimum entre une sphère et une boite. Si c'est nagatif, indique la profondeur de la sphère dans la boite.</summary>
        public static float Dist(SphereCollider s, BoxCollider b) { return Dist(ToSphere(s), b); }

        /// <summary>Calcule approximativement la distance minimum entre deux boites. <b>Elle est basée sur la distance de chaque point par rapport à l'autre boite.</b></summary>
        public static float Dist(BoxCollider b1, BoxCollider b2)
        {
            Point[] pts = b1.GetVertices();
            float closestDist = Dist(b2, pts[0]);
            for (int i = 1; i < pts.Length; i++)
            {
                float d = Dist(b2, pts[i]);
                if (d < closestDist)
                    closestDist = d;
            }
            foreach (Point pt in b2.GetVertices())
            {
                float d = Dist(b1, pt);
                if (d < closestDist)
                    closestDist = d;
            }
            return closestDist;
        }

        /// <summary>Calcule la distance minimum entre un point et un mesh. Si c'est nagatif, indique la profondeur du point dans le mesh.</summary>
        public static float Dist(Point p, MeshCollider m) { return Dist(m, p); }
        /// <summary>Calcule la distance minimum entre un point et un mesh. Si c'est nagatif, indique la profondeur du point dans le mesh.</summary>
        public static float Dist(MeshCollider m, Point p)
        {
            float max = float.MinValue;
            foreach (Plane pl in m.GetPlanes())
                max = Mathf.Max(DistSigned(pl, p), max);
            float d = (max > 0) ? Dist(m.ClosestPoint(p), p) : max;
            return d;
            //return Dist(m.ClosestPoint(p), p);
        }
        
        /// <summary>Calcule la distance minimum entre un plan et un mesh. Si c'est nagatif, indique la hauteur du plus petit morceaux du mesh coupé par le plan.</summary>
        public static float Dist(Plane p, MeshCollider m) { return Dist(m, p); }
        /// <summary>Calcule la distance minimum entre un plan et un mesh. Si c'est nagatif, indique la hauteur du plus petit morceaux du mesh coupé par le plan.</summary>
        public static float Dist(MeshCollider m, Plane p)
        {
            return Dist(m.GetVertices(), p);
        }

        /// <summary>Calcule la distance minimum entre une sphère et un mesh. Si c'est nagatif, indique la profondeur du mesh dans la sphère.</summary>
        public static float Dist(Sphere s, MeshCollider m) { return Dist(m, s); }
        /// <summary>Calcule la distance minimum entre une sphère et un mesh. Si c'est nagatif, indique la profondeur du mesh dans la sphère.</summary>
        public static float Dist(MeshCollider m, Sphere s)
        {
            return Dist(m, s.position) - s.radius;
        }

        /// <summary>Calcule la distance minimum entre une sphère et un mesh. Si c'est nagatif, indique la profondeur du mesh dans la sphère.</summary>
        public static float Dist(SphereCollider s, MeshCollider m) { return Dist(m, s); }
        /// <summary>Calcule la distance minimum entre une sphère et un mesh. Si c'est nagatif, indique la profondeur du mesh dans la sphère.</summary>
        public static float Dist(MeshCollider m, SphereCollider s) { return Dist(m, ToSphere(s)); }

        /// <summary>Calcule approximativement la distance minimum entre la boite et le mesh. <b>Elle est basée sur la distance de chaque point par rapport à l'autre objet.</b></summary>
        public static float Dist(BoxCollider b, MeshCollider m) { return Dist(m, b); }
        /// <summary>Calcule approximativement la distance minimum entre la boite et le mesh. <b>Elle est basée sur la distance de chaque point par rapport à l'autre objet.</b></summary>
        public static float Dist(MeshCollider m, BoxCollider b)
        {
            Point[] pts = m.GetVertices();
            float closestDist = Dist(b, pts[0]);
            for (int i = 1; i < pts.Length; i++)
            {
                float d = Dist(b, pts[i]);
                if (d < closestDist)
                    closestDist = d;
            }
            foreach (Point pt in b.GetVertices())
            {
                float d = Dist(m, pt);
                if (d < closestDist)
                    closestDist = d;
            }
            return closestDist;
        }

        /// <summary>Calcule approximativement la distance minimum entre les deux meshes. <b>Elle est basée sur la distance de chaque point par rapport à l'autre mesh.</b></summary>
        public static float Dist(MeshCollider m1, MeshCollider m2)
        {
            Point[] pts = m1.GetVertices();
            float closestDist = Dist(m2, pts[0]);
            for (int i = 1; i < pts.Length; i++)
            {
                float d = Dist(m2, pts[i]);
                if (d < closestDist)
                    closestDist = d;
            }
            foreach (Point pt in m2.GetVertices())
            {
                float d = Dist(m1, pt);
                if (d < closestDist)
                    closestDist = d;
            }
            return closestDist;
        }




    }


    
    



    public static class BoxColliderMethods
    {
        public static Bounds GetLocalBounds(this BoxCollider c)
        {
            return new Bounds(c.center, c.size);
        }

        /// <summary>
        /// Retourne les coordonnées des 8 points de ce box collider.
        /// </summary>
        public static Point[] GetVertices(this BoxCollider b)
        {
            Point[] pts = b.GetLocalBounds().GetVertices();
            for (int i = 0; i < pts.Length; i++)
                pts[i] = b.transform.TransformPoint(pts[i]);
            return pts;
        }

        /// <summary>
        /// Retourne les 6 plans de ce box collider. Les normales de ces plans sont orientés vers l'extérieur.
        /// </summary>
        public static Plane[] GetPlanes(this BoxCollider b)
        {
            Point[] pts = b.GetVertices();
            return new Plane[]
            {
                new Plane((pts[0] - pts[1]).normalized, pts[0]),
                new Plane((pts[1] - pts[0]).normalized, pts[1]),
                new Plane((pts[0] - pts[2]).normalized, pts[0]),
                new Plane((pts[2] - pts[0]).normalized, pts[2]),
                new Plane((pts[0] - pts[4]).normalized, pts[0]),
                new Plane((pts[4] - pts[0]).normalized, pts[4])
            };
        }
    }


    public static class MeshColliderMethods
    {
        public static Point[] GetVertices(this MeshCollider m)
        {
            Point[] pts = new Point[m.sharedMesh.vertices.Length];
            for (int i = 0; i < pts.Length; i++)
                pts[i] = m.transform.TransformPoint(m.sharedMesh.vertices[i]);
            return pts;
        }

        public static Plane[] GetPlanes(this MeshCollider m)
        {
            Point[] vertices = m.GetVertices();
            List<Plane> planes = new List<Plane>();
            for (int i = 0; i < m.sharedMesh.triangles.Length; i += 3)
            {
                int i0 = m.sharedMesh.triangles[i], i1 = m.sharedMesh.triangles[i + 1], i2 = m.sharedMesh.triangles[i + 2];
                Point v0 = vertices[i0], v1 = vertices[i1], v2 = vertices[i2];
                planes.Add(new Plane(v0, v1, v2));
            }
            return planes.ToArray();
        }
    }


    public static class BoundsMethods
    {
        /// <summary>
        /// Retourne les coordonnées des 8 points de cette bounding box.
        /// </summary>
        public static Point[] GetVertices(this Bounds b)
        {
            return new Point[]
            {
                new Point(b.min.x, b.min.y, b.min.z),
                new Point(b.min.x, b.min.y, b.max.z),
                new Point(b.min.x, b.max.y, b.min.z),
                new Point(b.min.x, b.max.y, b.max.z),
                new Point(b.max.x, b.min.y, b.min.z),
                new Point(b.max.x, b.min.y, b.max.z),
                new Point(b.max.x, b.max.y, b.min.z),
                new Point(b.max.x, b.max.y, b.max.z)
            };
        }
    }






}
