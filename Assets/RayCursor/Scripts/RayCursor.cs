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
    public enum Mode
    {
        Manual, SemiAuto
    }

    public enum TransferFunction
    {
        Lerp, LerpDistance
    }

    public class RayCursor : MonoBehaviour
    {
        /*
         * Static part
         */
        public static RayCursor instance;

        public static bool Loaded { get { return instance != null; } }
        public static bool Enabled { get { return Loaded && instance.enabled; } }
        public static Material HighLightMaterial { get { return Loaded ? instance.highlightMaterial : null; } }






        /*
         * Instance part
         */

        public RayObject ray;
        public CursorObject cursor;
        
        public Material highlightMaterial;



        public Mode CurrentMode = Mode.SemiAuto; // this one is displayed in the inspector
        private Mode currentMode;
        internal Dictionary<Mode, CursorMode> modes;

        public TransferFunction currentTransfertFunction = TransferFunction.Lerp;
        internal Dictionary<TransferFunction, CursorTransferFunction> transferFunctions;


        public bool rayFiltered;



        // Start is called before the first frame update
        void Start()
        {
            if (instance != null && instance != this)
            {
                DestroyImmediate(instance.gameObject);
            }
            instance = this;
            
            modes = new Dictionary<Mode, CursorMode>();
            modes[Mode.Manual] = new ManualMode(this);
            modes[Mode.SemiAuto] = new SemiAutoMode(this);
            currentMode = CurrentMode;
            modes[currentMode].Init();

            transferFunctions = new Dictionary<TransferFunction, CursorTransferFunction>();
            transferFunctions[TransferFunction.Lerp] = new LerpTransfertFunction(this);
            transferFunctions[TransferFunction.LerpDistance] = new LerpDistanceTransfertFunction(this);
        }

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }


        private Selectable previousClosest = null;

        void Update()
        {

            if (CurrentMode != currentMode)
                SetMode(CurrentMode);
            
            GetCursorTransferFunction().Update();

            GetCursorMode().Update();

            if (rayFiltered != ray.FilterEnabled)
                ray.FilterEnabled = rayFiltered;
            
            Selectable closest = ClosestSelectable(cursor.Position);
            if (previousClosest != closest)
            {
                if (previousClosest != null)
                    previousClosest.Highlighted = false;
                if (closest != null)
                    closest.Highlighted = true;
                previousClosest = closest;
            }

            if (GetInputSelect() && closest != null)
            {
                closest.Select();
                GetCursorMode().OnSelect();
            }
        }


        public static Selectable ClosestSelectable(Vector3 p)
        {
            Selectable closest = null;
            float closestDist = float.MaxValue;
            foreach (Selectable s in Selectable.Enumerable)
            {
                float d = s.Distance(p);
                if (d < closestDist)
                {
                    closestDist = d;
                    closest = s;
                }
            }


            return closest;
        }




        private void SetMode(Mode type)
        {
            modes[currentMode].Deinit();
            currentMode = type;
            modes[currentMode].Init();
        }

        internal CursorMode GetCursorMode()
        {
            return modes[currentMode];
        }

        internal CursorTransferFunction GetCursorTransferFunction()
        {
            return transferFunctions[currentTransfertFunction];
        }


        /**
         * <summary>Return true only during the frame the selection button is pressed but was not pressed the frame before (like Input.GetButtonDown()).</summary>
         */
        internal bool GetInputSelect()
        {
            return Input.GetButtonDown("RayCursorSelect");
        }

        /**
         * <summary>Return true during all the frames the touchpad is touched.</summary>
         */
        internal bool GetInputTouch()
        {
            return Input.GetButton("RayCursorTouch");
        }

        internal float GetInputTouchY()
        {
            return Input.GetAxis("RayCursorTouchY");
        }
    }

    

    public abstract class CursorMode
    {
        protected RayCursor cursorManager;
        protected CursorObject cursor;
        protected RayObject ray;
        public CursorMode(RayCursor cM)
        {
            cursorManager = cM;
            cursor = cM.cursor;
            ray = cM.ray;
        }

        public abstract void Update();
        public abstract void Init();
        public abstract void Deinit();
        public abstract void OnSelect();
    }
    
    class ManualMode : CursorMode
    {

        public ManualMode(RayCursor cM) : base(cM)
        {
            CurrentDistance = 1;
        }

        public float CurrentDistance
        {
            get; protected set;
        }

        public override void Init()
        {

            ray.Distance = CurrentDistance;
            ray.HideRayAfterCursor = false;

            cursor.Distance = CurrentDistance;

            ray.Visible = true;
            cursor.Visible = true;

        }

        public override void Deinit()
        {
            ray.Visible = false;
            cursor.Visible = false;
        }


        public virtual void UpdateCurrentDistance()
        {
            CurrentDistance = Mathf.Clamp(cursorManager.GetCursorTransferFunction().ComputeDistance(CurrentDistance), 0, 100);
        }

        public override void Update()
        {
            UpdateCurrentDistance();
            ray.Distance = CurrentDistance;
            cursor.Distance = CurrentDistance;
        }

        public override void OnSelect()
        {
            // do nothing
        }
    }
    
    class SemiAutoMode : ManualMode
    {
        public const float FAR_DISTANCE = 1000;


        public SemiAutoMode(RayCursor cM) : base(cM) { }

        protected float timeoutTouch = 1;

        protected float lastTimeTouch = -1;

        protected Selectable hit;

        public override void Init()
        {
            lastTimeTouch = Time.time - timeoutTouch;
            base.Init();
        }

        public override void UpdateCurrentDistance()
        {
            float hitDist = GetPotentialHitDistance();
            if (cursorManager.GetCursorTransferFunction().GetInputActivation())
            {
                lastTimeTouch = Time.time;
                CurrentDistance = Mathf.Clamp(cursorManager.GetCursorTransferFunction().ComputeDistance(CurrentDistance), 0, 100);
            }


            if (Time.time - lastTimeTouch < timeoutTouch)
            {
                // manual mode
                ray.HideRayAfterCursor = false;
                cursor.SetVisibility(Mathf.InverseLerp(timeoutTouch, 0, Time.time - lastTimeTouch));
            }
            else
            {
                // automatic mode
                if (!float.IsNaN(hitDist))
                {   // update cursor distance with collided object (raycasting mode)
                    // cursor visible
                    CurrentDistance = hitDist;
                    ray.HideRayAfterCursor = true;
                    cursor.SetVisibility(0.5f);
                }
                else
                {   // maintain cursor distance
                    // cursor visible
                    ray.HideRayAfterCursor = false;
                    cursor.SetVisibility(0.5f);
                }
            }


        }

        protected float GetPotentialHitDistance()
        {
            Transform t = ray.transform;
            Ray lRay = new Ray(t.position, t.forward);
            RaycastHit hit;
            if (Physics.Raycast(lRay, out hit) && hit.collider.GetComponent<Selectable>() != null)
            {
                this.hit = hit.collider.GetComponent<Selectable>();
                return hit.distance;
            }
            else
            {
                this.hit = null;
                return float.NaN;
            }
        }

        public override void Deinit()
        {
            base.Deinit();
            cursor.SetVisibility(1);
        }


        public override void OnSelect()
        {
            lastTimeTouch = Time.time - timeoutTouch;
        }

    }



    public abstract class CursorTransferFunction
    {
        protected RayCursor cursorManager;
        public CursorTransferFunction(RayCursor cM)
        {
            cursorManager = cM;
        }

        private float previousPadY = 0;
        private bool previousPadTouch = false;
        private float previousTime = Time.time;

        protected float ySpeed = 0;

        public void Update()
        {
            bool currentPadTouch = cursorManager.GetInputTouch();
            float currentPadY = cursorManager.GetInputTouchY();
            float currentTime = Time.time;
            if (!currentPadTouch || !previousPadTouch)
                ySpeed = 0;
            else
            {
                ySpeed = (currentPadY - previousPadY) * -0.02f / (currentTime - previousTime); // 0.02 is the radius of the touchpad in meters
            }

            previousPadTouch = currentPadTouch;
            previousPadY = currentPadY;
            previousTime = currentTime;
        }

        public float ComputeDistance(float previousDistance)
        {
            float curSpeed = TransfertFunction(ySpeed, previousDistance); // m/s cursor
            float curDelta = curSpeed * Time.deltaTime; // m/frame cursor
            return previousDistance + curDelta; // relative control
        }



        /// <param name="input">vitesse en y du doigt sur le pad (+ haut, - bas) en m/s</param>
        /// <param name="previousDistance">la distance manette-curseur actuelle, en m</param>
        /// <returns>la vitesse de déplacement du curseur (- se rapproche, + s'éloigne) en m/s</returns>
        public float TransfertFunction(float input, float previousDistance)
        {
            return input * GainFunction(input, previousDistance);
        }



        /// <param name="input">vitesse en y du doigt sur le pad (+ haut, - bas) en m/s</param>
        /// <param name="d_prev">la distance manette-curseur actuelle, en m</param>
        /// <returns>le gain de la vitesse de déplacement du curseur par rapport à la vitesse du doigt sur le pad</returns>
        public abstract float GainFunction(float input, float previousDistance);



        public bool GetInputActivation()
        {
            return previousPadTouch;
        }
    }
    
    class LerpTransfertFunction : CursorTransferFunction
    {
        public LerpTransfertFunction(RayCursor cM) : base(cM)
        {
            K1 = defaultK1;
            K2 = defaultK2;
            V1 = defaultV1;
            V2 = defaultV2;
        }

        public override float GainFunction(float input, float previousDistance)
        {
            input = Mathf.Abs(input);
            return
                input < V1 ? K1 :
                input > V2 ? K2 :
                (K1 + (input - V1) * (K2 - K1) / (V2 - V1)); //interpolation linéaire */
        }

        public const float defaultV1 = 0.05f, defaultV2 = 0.15f; // in m/s
        public const float defaultK1 = 30, defaultK2 = 150; // gain
        public static float K1 { get; set; }
        public static float K2 { get; set; }
        public static float V1 { get; set; }
        public static float V2 { get; set; }
        /*    M    T
         * K1 30   30
         * K2 150  150
         * V1 0.05 0.05
         * V2 0.15 0.15
         */

    }

    class LerpDistanceTransfertFunction : CursorTransferFunction
    {
        internal const float avgHMDCtrlDist = 0.55f;

        public LerpDistanceTransfertFunction(RayCursor cM) : base(cM)
        {
            K1 = defaultK1;
            K2 = defaultK2;
            V1 = defaultV1;
            V2 = defaultV2;
        }

        public override float GainFunction(float input, float previousDistance)
        {
            input = Mathf.Abs(input);
            return new Vector2(previousDistance, avgHMDCtrlDist).magnitude *
                (input < V1 ? K1 :
                input > V2 ? K2 :
                (K1 + (input - V1) * (K2 - K1) / (V2 - V1))); //interpolation linéaire */
        }

        public const float defaultV1 = 0.05f, defaultV2 = 0.15f; // in m/s
        public const float defaultK1 = 20, defaultK2 = 100; // gain
        public static float K1 { get; set; }
        public static float K2 { get; set; }
        public static float V1 { get; set; }
        public static float V2 { get; set; }
        /*    M    T    M2
         * K1 30   30   20
         * K2 150  150  100
         * V1 0.05 0.05 0.05
         * V2 0.15 0.15 0.15
         */

    }
    
}

