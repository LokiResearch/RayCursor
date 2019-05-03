using RayCursor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayCursor
{
    
    public class CursorObject : MonoBehaviour
    {
        private Color baseColor;
        private float baseLightIntensity;
        private float baseRadius;

        public Color autoColor;


        public void Start()
        {
            baseColor = Color;
            baseLightIntensity = LightIntensity;
            baseRadius = Radius;
        }
        

        public Vector3 Position
        {
            get { return transform.position; }
        }

        public float Distance
        {
            get { return transform.localPosition.z; }
            set { transform.localPosition = Vector3.forward * value; }
        }

        public float Radius
        {
            get { return transform.localScale.x / 2; }
            set { transform.localScale = Vector3.one * 2 * value; }
        }

        public float LightIntensity
        {
            get { return GetComponent<Light>().intensity; }
            set { GetComponent<Light>().intensity = value; }
        }

        public Color Color
        {
            get { return GetComponent<MeshRenderer>().material.color; }
            set
            {
                GetComponent<MeshRenderer>().material.color = value;
                GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", value * 0.75f);
            }
        }



        public bool Visible
        {
            get { return GetComponent<MeshRenderer>().enabled; }
            set
            {
                GetComponent<MeshRenderer>().enabled = value;
                GetComponent<Light>().enabled = value;
            }
        }



        public void SetVisibility(float visibility, bool transparent = false)
        {
            //cursor.Radius = Mathf.Lerp(baseCursorRadius * 0.1f, baseCursorRadius, visibility);
            Color c = Color.Lerp(autoColor, baseColor, visibility);
            c.a = transparent ? Mathf.Lerp(0, 1, visibility) : 1;
            Color = c;
            
            LightIntensity = Mathf.Lerp(0, baseLightIntensity, visibility);
            Visible = (visibility > 0);
        }
    }
}
