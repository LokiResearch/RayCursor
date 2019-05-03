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
using System.Collections.Generic;
using UnityEngine;

namespace RayCursor
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Selectable : MonoBehaviour
    {


        private static HashSet<Selectable> AllSelectables = new HashSet<Selectable>();
       
        
        public event System.Action OnSelect;
        

        public void OnEnable()
        {
            AllSelectables.Add(this);

            highlightable = SecondMaterial == null;


            if (GetComponent<MeshCollider>() != null)
            {
                MeshCollider mc = GetComponent<MeshCollider>();
                mc.cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation
                    | MeshColliderCookingOptions.EnableMeshCleaning
                    | MeshColliderCookingOptions.WeldColocatedVertices;
                mc.convex = true;
            }
        }

        public void OnDisable()
        {
            if (Highlighted)
                Highlighted = false;

            highlightable = false;
            AllSelectables.Remove(this);
        }


        internal void Select()
        {
            Debug.Log("Selected: " + gameObject.name);
            if (OnSelect != null)
                OnSelect();
        }


        public float Distance(Vector3 p)
        {
            if (GetComponent<BoxCollider>() != null)
                return DistanceUtil.Dist(GetComponent<BoxCollider>(), p);
            if (GetComponent<SphereCollider>() != null)
                return DistanceUtil.Dist(GetComponent<SphereCollider>(), p);
            if (GetComponent<MeshCollider>() != null)
                return DistanceUtil.Dist(GetComponent<MeshCollider>(), p);
            else
                return DistanceUtil.Dist(GetComponent<Collider>().ClosestPoint(p), p);
        }



        internal static IEnumerable<Selectable> Enumerable { get { return AllSelectables; } }







        private bool highlightable = false;
        public bool Highlighted
        {
            get { return highlightable && SecondMaterial != null; }
            set
            {
                if (!highlightable)
                    value = false;
                if (value == (SecondMaterial != null))
                    return;

                if (value)
                {
                    SecondMaterial = RayCursor.instance.highlightMaterial;
                }
                else
                {
                    SecondMaterial = null;
                }
            }
        }

        private Material SecondMaterial
        {
            set
            {
                if (value == null)
                    GetComponent<MeshRenderer>().materials = new Material[] { GetComponent<MeshRenderer>().materials[0] };
                else if (GetComponent<MeshRenderer>().materials.Length == 1)
                    GetComponent<MeshRenderer>().materials = new Material[] { GetComponent<MeshRenderer>().materials[0], value };
                else
                    GetComponent<MeshRenderer>().materials[1] = value;
            }
            get
            {
                if (GetComponent<MeshRenderer>().materials.Length > 1)
                    return GetComponent<MeshRenderer>().materials[1];
                return null;
            }
        }




    }
}

