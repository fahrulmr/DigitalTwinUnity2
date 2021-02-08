using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace game4automation
{


    //! Controls touch interaction during game4automation simulation / play mode
    public class TouchInteraction : MonoBehaviour
    {
        [Header("Settings")] public bool EnableZoom = true;
        public bool EnableRotation = true;
        public bool EnableOneFingerPan = true;
        public bool EnableTwoFingerPan = true;
        public bool AllowTouchOverUI = false;
        public bool EnableTilt = true;
        public float MinRot = 0.1f;
        public bool EnableTest = true;
        public bool DebugLog = true;

        [Header("Status")] public Vector3 Pan;
        public float Zoom;
        public float Rot;
        public Vector3 Tilt;
        public Vector2 FirstTouch;
        public Vector2 SecondTouch;
        public Vector2 ThirdTouch;


        private bool _test = false;
        private Vector2 _firstbefore;
        private Vector2 _secondbefore;
        private Vector2 _firstdeltapos;
        private Vector2 _seconddeltapos;
        private int _tapcount;
        private float _doubleTapTimer;

        public delegate void OneTouchPanDelegate(Vector2 pos, Vector2 pan);

        public OneTouchPanDelegate oneTouchPanEvent;

        public delegate void TwoTouchPanZoomRotDelegate(Vector2 pos, Vector2 pan, float zoom, float rot);

        public TwoTouchPanZoomRotDelegate twoTouchPanZoomRotDelegate;


        public delegate void ThreeTouchPanDelegate(Vector2 pos, Vector2 pan);

        public ThreeTouchPanDelegate threeTouchPanDelegate;

        public delegate void DoubleTouchDelegate(Vector2 pos);

        public DoubleTouchDelegate doubleTouchDelegate;

        // Update is called once per frame
        void Update()
        {

            // Double touch
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _tapcount++;
            }

            if (_tapcount > 0)
            {
                _doubleTapTimer += Time.deltaTime;
            }

            //Double Tap Detected
            if (_tapcount >= 2)
            {
                _doubleTapTimer = 0.0f;
                _tapcount = 0;
                if (doubleTouchDelegate != null)
                {
                    doubleTouchDelegate(Input.GetTouch(0).position);
                }
            }

            if (_doubleTapTimer > 0.5f)
            {
                _doubleTapTimer = 0f;
                _tapcount = 0;
            }


            Zoom = 0;
            Rot = 0;
            Pan = Vector3.zero;
            Tilt = Vector3.zero;

            if (_test == false)
            {
                FirstTouch = Vector2.zero;
                SecondTouch = Vector3.zero;
                ThirdTouch = Vector3.zero;
            }

            // Set Values for test
            if (EnableTest)
            {

                if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftAlt))
                {
                    FirstTouch = Input.mousePosition;
                    _test = true;
                }

                if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.LeftControl))
                {
                    SecondTouch = Input.mousePosition;
                    _test = true;
                }


                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0))
                {
                    FirstTouch = Input.mousePosition;
                    SecondTouch = Input.mousePosition;
                    ThirdTouch = Input.mousePosition;
                    _test = true;
                }

                if (_test)
                {
                    if (_firstbefore != Vector2.zero)
                    {
                        _firstdeltapos = FirstTouch - _firstbefore;
                        _seconddeltapos = SecondTouch - _secondbefore;
                    }

                    _firstbefore = FirstTouch;
                    _secondbefore = SecondTouch;
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        _test = false;
                        _firstdeltapos = Vector2.zero;
                        _seconddeltapos = Vector2.zero;
                        _firstbefore = Vector2.zero;
                        _secondbefore = Vector2.zero;
                    }
                }
            }

            if (AllowTouchOverUI == false)
            {
                foreach (Touch touch in Input.touches)
                {
                    int id = touch.fingerId;
                    if (EventSystem.current.IsPointerOverGameObject(id))
                    {
                        return;
                    }
                }
            }


            // Set Values if no test
            if (Input.touchCount == 1)
            {
                Touch First = Input.GetTouch(0);
                FirstTouch = First.position;
                _firstdeltapos = First.deltaPosition;
            }

            if (Input.touchCount == 2)
            {
                Touch First = Input.GetTouch(0);
                Touch Second = Input.GetTouch(1);
                FirstTouch = First.position;
                _firstdeltapos = First.deltaPosition;
                SecondTouch = Second.position;
                _seconddeltapos = Second.deltaPosition;
            }

            if (Input.touchCount == 3)
            {
                Touch First = Input.GetTouch(0);
                Touch Second = Input.GetTouch(1);
                Touch Third = Input.GetTouch(2);
                FirstTouch = First.position;
                _firstdeltapos = First.deltaPosition;
                SecondTouch = Second.position;
                _seconddeltapos = Second.deltaPosition;
                ThirdTouch = Third.position;

            }


            // Set actions
            if (Input.touchCount == 1 || (_test && SecondTouch == Vector2.zero))
            {
                if (EnableOneFingerPan)
                {
                    Pan = _firstdeltapos;
                    if (DebugLog)
                        Debug.Log("One Finger Touch Pan: " + Pan);

                    // Call Delegates
                    if (oneTouchPanEvent != null)
                    {
                        oneTouchPanEvent(FirstTouch, Pan);
                    }
                }
            }

            if (Input.touchCount == 2 || (_test && SecondTouch != Vector2.zero && ThirdTouch == Vector2.zero))
            {
                // Find the position in the previous frame of each touch
                var action = false;
                Vector2 firstTouchPrevPos = FirstTouch - _firstdeltapos;
                Vector2 secondTouchPrevPos = SecondTouch - _seconddeltapos;
                float prevTouchDeltaMag = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
                float touchDeltaMag = (FirstTouch - SecondTouch).magnitude;
                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                if (EnableZoom)
                {
                    Zoom = -deltaMagnitudeDiff;
                    action = true;
                }

                // Pan
                if (EnableTwoFingerPan)
                {
                    Pan = _firstdeltapos;
                    action = true;
                }

                // Rotation
                if (EnableRotation)
                {
                    // Delta Degrees between vectors between two fingers
                    Vector2 vectorprevpos = firstTouchPrevPos - secondTouchPrevPos;
                    Vector2 vectorpos = FirstTouch - SecondTouch;

                    Rot = Vector2.SignedAngle(vectorpos, vectorprevpos);
                    if (Math.Abs(Rot) > MinRot)
                    {
                        action = true;
                    }
                    else
                    {
                        Rot = 0;
                    }
                }

                if (DebugLog)
                    Debug.Log("Two Finger Touch Pan: " + Pan + " Zoom: " + Zoom + "  Rot: " + Rot);

                // Call Delegates
                if (twoTouchPanZoomRotDelegate != null && action)
                {
                    var MidPos = FirstTouch + (SecondTouch - FirstTouch) / 2;

                    twoTouchPanZoomRotDelegate(MidPos, Pan, Zoom, Rot);
                }
            }


            if (Input.touchCount == 3 || (_test && ThirdTouch != Vector2.zero && SecondTouch != Vector2.zero))
            {

                if (EnableTilt)
                {
                    Tilt = _firstdeltapos;
                    if (DebugLog)
                        Debug.Log("Three Finger Tilt Touch: " + Tilt);
                    if (threeTouchPanDelegate != null)
                    {
                        var MidPos = FirstTouch + (ThirdTouch - FirstTouch) / 2;

                        threeTouchPanDelegate(MidPos, Tilt);
                    }
                }


            }


        }
    }
}