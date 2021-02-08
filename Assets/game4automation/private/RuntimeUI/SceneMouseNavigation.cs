﻿﻿﻿// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

   using System;
   using UnityEngine;
#if ((!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR)
using SpaceNavigatorDriver;
#endif
#if CINEMACHINE
   using Cinemachine;
#endif

  namespace game4automation
{
    public class SceneMouseNavigation : Game4AutomationBehavior
    {
        public bool FirstPersonControllerActive = true;
        public FirstPersonController FirstPersonController;
        public CameraPos LastCameraPosition;
        public Transform target;
        public Vector3 targetOffset;
        public float distance = 5.0f;
        public float maxDistance = 20;
        public float minDistance = .6f;
        public float xSpeed = 200.0f;
        public float ySpeed = 200.0f;
        public int yMinLimit = -80;
        public int yMaxLimit = 80;
        public int zoomRate = 40;
        public float panSpeed = 0.3f;
        public float zoomDampening = 5.0f;
        public float StartDemoOnInactivity = 5.0f;
        public GameObject DebugObj;

        [Header("Touch")] public TouchInteraction Touch;
        public float TouchPanSpeed = 200f;
        public float TouchRotationSpeed = 200f;
        public float TouchTiltSpeed = 200f;
        public float TouchZoomSpeed = 10f;

        [Header("SpaceNavigator")] 
        public bool EnableSpaceNavigator = true;
        public float SpaceNavTransSpeed = 1;
        private float xDeg = 0.0f;
        private float yDeg = 0.0f;
        [Header("Distance and Rotation")] 
        public float currentDistance;
        public float desiredDistance;
        public Quaternion currentRotation;
        public Quaternion desiredRotation;
        private Quaternion rotation;
        private Vector3 position;
        private Camera mycamera;
        private float _lastmovement;
        private bool _demostarted;
        private Vector3 _pos;
        private bool touch;
        private bool cinemachineactive = false;

        void Start()
        {
            Init();
        }

        void OnEnable()
        {
            Touch.oneTouchPanEvent += OneFingerMoveHandler;
            Touch.twoTouchPanZoomRotDelegate += TwoFingerTransformHandler;
            Touch.threeTouchPanDelegate += ThreeFingerMoveHandler;
            Init();   
            
        }

        private void OnApplicationQuit()
        {
            if (LastCameraPosition!=null)
                LastCameraPosition.SaveCameraPosition(this);
        }
        
        private void OnDisable()
        {
            Touch.oneTouchPanEvent -= OneFingerMoveHandler;
            Touch.twoTouchPanZoomRotDelegate -= TwoFingerTransformHandler;
            Touch.threeTouchPanDelegate = ThreeFingerMoveHandler;
        }

        public void OnViewButton(GenericButton button)
        {
            if (button.IsOn && FirstPersonController != null)
            {
                if (LastCameraPosition!=null)
                    LastCameraPosition.SaveCameraPosition(this);
                if (cinemachineactive)
                    ActivateCinemachine(false);
                FirstPersonControllerActive = true;
                FirstPersonController.SetActive(true);

            }
            else
            {
                FirstPersonControllerActive = false;
                FirstPersonController.SetActive(false);
                if (LastCameraPosition!=null)
                    SetNewCameraPosition(LastCameraPosition.TargetPos, LastCameraPosition.CameraDistance, LastCameraPosition.CameraRot);
            }
        }


        private void MoveCam(Vector3 deltatrans, Vector3 deltarot, float deltadistance)
        {
            // Set Values
            target.rotation = transform.rotation;
            currentDistance = currentDistance + deltadistance;
            desiredDistance = currentDistance;
            target.transform.Translate(deltatrans);
            transform.Rotate(deltarot);
            rotation = transform.rotation;
            position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
            desiredDistance = currentDistance; 
            transform.position = position;
         
            touch = true;

        }
        private void OneFingerMoveHandler(Vector2 pos, Vector2 pan)
        {
          
            Vector3 trans = new Vector3(-pan.x/Screen.width,pan.y/Screen.height,0);
            var targetdeltatrans = trans*TouchPanSpeed*6;
            MoveCam(targetdeltatrans, new Vector3(0,0,0 ), 0 );
    
        }

       private void ThreeFingerMoveHandler(Vector2 pos, Vector2 pan)
        {
            Vector3 trans =  new Vector3(-pan.x/Screen.width,pan.y/Screen.height,0);
            // set camera rotation 
            var rot = new Vector3 (-trans.y*TouchTiltSpeed*100, -trans.x*TouchTiltSpeed*100, 0);
            MoveCam(new Vector3(0,0,0), rot, 0 );
        }
        
        private void TwoFingerTransformHandler(Vector2 pos, Vector2 pan, float zoom, float rot)
        {
            var targetdeltatrans = new Vector3(0,0,0);
            var camdeltarot = new Vector3(0,0,0);
            // translation
           
            Vector3 trans = new Vector3(-pan.x/Screen.width,pan.y/Screen.height,0);
            targetdeltatrans = trans*TouchPanSpeed*6;
           
            // Rotation
            camdeltarot = new Vector3(0,0,rot*TouchRotationSpeed*2);

            // Scale
            var deltadistance = -zoom/Screen.width*TouchZoomSpeed*20;
           
            // Set Values
            MoveCam(targetdeltatrans,camdeltarot, deltadistance);    
    
        } 

        public void SetNewCameraPosition(Vector3 targetpos, float camdistance, Vector3 camrotation)
        {
            // End first person controller if it is on
            if (FirstPersonControllerActive)
            {
                FirstPersonController.SetActive(false);
                FirstPersonControllerActive = false;
            }
            if (target == null)
                return;
            desiredDistance = camdistance;
            currentDistance = camdistance;
            target.position = targetpos;
            desiredRotation = Quaternion.Euler(camrotation);
            currentRotation = Quaternion.Euler(camrotation);
            rotation = Quaternion.Euler(camrotation);
            transform.rotation = Quaternion.Euler(camrotation);
        }
        
        
        public void SetViewDirection(Vector3 camrotation)
        {
            
            desiredRotation = Quaternion.Euler(camrotation);
            currentRotation = Quaternion.Euler(camrotation);
            rotation = Quaternion.Euler(camrotation);
            transform.rotation = Quaternion.Euler(camrotation);
        }

        public void ActivateCinemachine(bool activate)
        {
#if CINEMACHINE
            CinemachineBrain brain;
            brain = GetComponent<CinemachineBrain>();
            if (brain == null)
                return;
            
            if (!activate)
            {
                if (brain.ActiveVirtualCamera != null)
                {
                    Quaternion camrot = brain.ActiveVirtualCamera.VirtualCameraGameObject.transform.rotation;
                    Vector3 rot = camrot.eulerAngles;
                    distance = Vector3.Distance(transform.position, target.position);
                    Vector3 tarpos = brain.ActiveVirtualCamera.VirtualCameraGameObject.transform.position +
                                     (camrot * Vector3.forward * distance + targetOffset);
                    SetNewCameraPosition(tarpos, distance, rot);
                }
            }
            if (brain != null)
            {
                if (activate)
                {
                    brain.enabled = true;

                }
                else
                {
                    brain.enabled = false;

                }
            }

            cinemachineactive = activate;
         
                
#endif
        }

        #if CINEMACHINE
        public void ActivateCinemachineCam(CinemachineVirtualCamera vcam)
        {
            vcam.enabled = true;
            vcam.Priority = 100;
            if (cinemachineactive==false)
                ActivateCinemachine(true);
            
            // Set low priority to all other vcams
            var vcams = GameObject.FindObjectsOfType(typeof(CinemachineVirtualCamera));
            foreach (CinemachineVirtualCamera vc in vcams)
            {
                if (vc != vcam)
                    vc.Priority = 10;
            }
        }
        #endif
        
        public void Init()
        {
#if CINEMACHINE
            ActivateCinemachine(false);
          
#endif
            //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
            if (!target)
            {
                GameObject go = new GameObject("Cam Target");
                go.transform.position = transform.position + (transform.forward * distance);
                target = go.transform;
            }

            mycamera = GetComponent<Camera>();

            distance = Vector3.Distance(transform.position, target.position);
            currentDistance = distance;
            desiredDistance = distance;

            //be sure to grab the current rotations as starting points.
            position = transform.position;
            rotation = transform.rotation;
            currentRotation = transform.rotation;
            desiredRotation = transform.rotation;

            xDeg = Vector3.Angle(Vector3.right, transform.right);
            yDeg = Vector3.Angle(Vector3.up, transform.up);
          
            
            if (LastCameraPosition!=null && !FirstPersonControllerActive)
                SetNewCameraPosition(LastCameraPosition.TargetPos, LastCameraPosition.CameraDistance, LastCameraPosition.CameraRot);


            if (FirstPersonController != null)
            {
                if (FirstPersonControllerActive)
                {
                    FirstPersonController.SetActive(true);
                }
                else
                {
                    FirstPersonController.SetActive(false);
                }
            }

        }

        void CameraTransform(Vector3 direction)
        {
            target.rotation = transform.rotation;
            target.Translate(direction * panSpeed);
            _lastmovement = Time.realtimeSinceStartup;
        }

        void CamereSetDirection(Vector3 direction)
        {
            desiredDistance = 10f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

    
        /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
        void LateUpdate()
        {


            if (FirstPersonControllerActive)
                return;
          
            if (cinemachineactive)
            {
                var scroll = Input.GetAxis("Mouse ScrollWheel");
                if (Input.GetMouseButton(2) || Input.GetMouseButton(3) || Input.GetMouseButton(1) 
                    || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl) ||
                    Input.GetKey(KeyCode.RightControl)
                    || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) ||
                    Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow) || Math.Abs(scroll)>0.001f||  Input.GetKey(KeyCode.Escape))
                {
                    ActivateCinemachine(false);
                }
            }

    
       
            // If Control and Middle button? ZOOM!
            if (Input.GetMouseButton(2) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && !touch)
            {
                _lastmovement = Time.realtimeSinceStartup;
                desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f *
                                   Mathf.Abs(desiredDistance);
         
            }
            // If right mous is selected ORBIT
           else if (Input.GetMouseButton(1) && !touch)
            {
                _lastmovement = Time.realtimeSinceStartup;
                xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                ////////OrbitAngle

                //Clamp the vertical axis for the orbit
                yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
                // set camera rotation 
                desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
                currentRotation = transform.rotation;

                rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
                transform.rotation = rotation;
            }
            // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace*/
            else if (Input.GetMouseButton(2) && !touch)
            {
                _lastmovement = Time.realtimeSinceStartup;
                //grab the rotation of the camera so we can move in a psuedo local XY space
                target.rotation = transform.rotation;
                target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
                target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
            }

            ////////Orbit Position

            // affect the desired Zoom distance if we roll the scrollwheel
            var mousescroll = Input.GetAxis("Mouse ScrollWheel");
            desiredDistance -= mousescroll * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);


            //clamp the zoom min/max
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);


            // Key Navigation
            var shift = false;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                shift = true;
            var control = false;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                control = true;
            // Key 3D Navigation
            if (Input.GetKey(KeyCode.UpArrow) && shift && !control)
                CameraTransform(Vector3.forward);

            if (Input.GetKey(KeyCode.DownArrow) && shift && !control)
                CameraTransform(Vector3.back);

            if (Input.GetKey(KeyCode.UpArrow) && !shift && !control)
                CameraTransform(Vector3.down);

            if (Input.GetKey(KeyCode.DownArrow) && !shift && !control)
                CameraTransform(Vector3.up);

            if (Input.GetKey(KeyCode.RightArrow) && !control)
                CameraTransform(Vector3.left);

            if (Input.GetKey(KeyCode.LeftArrow) && !control)
                CameraTransform(Vector3.right);

            if (Input.GetKey(KeyCode.LeftArrow) && control)
                CamereSetDirection(Vector3.left);

            if (Input.GetKey(KeyCode.T))
            {    
                SetViewDirection(new Vector3(90,90,0));
         
            }
            if (Input.GetKey(KeyCode.F))
            {
                SetViewDirection(new Vector3(0, 90, 0));
      
            }
            if (Input.GetKey(KeyCode.B))
            {
                SetViewDirection(new Vector3(270,90,0));
        
            }
            if (Input.GetKey(KeyCode.L))
            {
                SetViewDirection(new Vector3(0,180,0));
      
            }
            if (Input.GetKey(KeyCode.R))
            {
                SetViewDirection(new Vector3(0,0,0));
        
            }
                
            if (mycamera.orthographic)
            {
                mycamera.orthographicSize += mousescroll * mycamera.orthographicSize;
                desiredDistance = 0;
            }

            #if ((!UNITY_IOS && !UNITY_ANDROID) || UNITY_EDITOR)
            // Space Navigator
            if (EnableSpaceNavigator)
            {
                if (SpaceNavigator.Translation != Vector3.zero)
                {
                    target.rotation = transform.rotation;
                    var spacetrans = SpaceNavigator.Translation;
                    var newtrans = new Vector3(-spacetrans.x, spacetrans.y, -spacetrans.z) * SpaceNavTransSpeed;
                    target.Translate(newtrans, Space.Self);
                }

                if (SpaceNavigator.Rotation.eulerAngles != Vector3.zero)
                {
                   
                    transform.Rotate(-SpaceNavigator.Rotation.eulerAngles);
                    rotation = transform.rotation;
                }
            }
            #endif
       
           
            // For smoothing of the zoom, lerp distance
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

            // calculate position based on the new currentDistance 
            position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
            if (position != transform.position)
            {
                transform.position = position;
            }

            
            // 
            touch = false;
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}