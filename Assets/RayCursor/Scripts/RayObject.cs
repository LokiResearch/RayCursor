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
using OneEuroFilter;
using UnityEngine;

namespace RayCursor
{
    [RequireComponent(typeof(LineRenderer))]
    public class RayObject : MonoBehaviour
    {
        public LineRenderer lineRenderer { get { return GetComponent<LineRenderer>(); } }

        private OneEuroFilter<Quaternion> oneEuroFilter = null;
        private float minCutoff = 0.1f, beta = 50;
        private GameObject parent = null;
        private float cursorDistance = 0;
        private bool hideRayAfterCursor = false;

        public void Start()
        {
            Color color = lineRenderer.startColor;
            color.a = 0;
            lineRenderer.endColor = color;

            if (transform.parent != null)
                parent = transform.parent.gameObject;
        }


        public GameObject Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                if (oneEuroFilter == null)
                {
                    transform.parent = parent ? parent.transform : null;
                    transform.localEulerAngles = Vector3.zero;
                    transform.localPosition = Vector3.zero;
                }
            }
        }




        public bool FilterEnabled
        {
            set
            {
                oneEuroFilter = value ? new OneEuroFilter<Quaternion>(90, minCutoff, beta) : null;
                transform.parent = value ? null : parent ? parent.transform : transform.parent;
                if (!value)
                {
                    transform.localEulerAngles = Vector3.zero;
                    transform.localPosition = Vector3.zero;
                }
            }
            get { return oneEuroFilter != null; }
        }


        public float FilterMinCutoff
        {
            get { return minCutoff; }
            set
            {
                minCutoff = value;
                if (oneEuroFilter != null)
                    oneEuroFilter.UpdateParams(oneEuroFilter.freq, value, oneEuroFilter.beta, oneEuroFilter.dcutoff);
            }
        }

        public float FilterBeta
        {
            get { return beta; }
            set
            {
                beta = value;
                if (oneEuroFilter != null)
                    oneEuroFilter.UpdateParams(oneEuroFilter.freq, oneEuroFilter.mincutoff, value, oneEuroFilter.dcutoff);
            }
        }



        public void Update()
        {
            if (oneEuroFilter != null)
            {
                transform.rotation = oneEuroFilter.Filter(parent.transform.rotation, Time.realtimeSinceStartup);
                transform.position = parent.transform.position;
            }
        }
        


        public float Distance
        {
            get { return cursorDistance; }
            set
            {
                cursorDistance = value;
                if (hideRayAfterCursor)
                    lineRenderer.SetPosition(1, Vector3.forward * value);
                else
                    lineRenderer.SetPosition(1, Vector3.forward * 1000);
            }
        }
        


        public bool HideRayAfterCursor
        {
            get { return hideRayAfterCursor; }
            set
            {
                hideRayAfterCursor = value;
                Distance = Distance; // just to update line length
            }
        }



        public bool Visible
        {
            get { return lineRenderer.enabled; }
            set
            {
                lineRenderer.enabled = value;
            }
        }

    }
}