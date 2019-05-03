using OneEuroFilter;
using System.Collections;
using System.Collections.Generic;
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