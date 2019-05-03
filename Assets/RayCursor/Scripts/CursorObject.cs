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
