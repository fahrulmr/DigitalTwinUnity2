﻿﻿﻿﻿﻿﻿// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  
  
using game4automationtools;
using UnityEngine;

namespace game4automation
{
    [RequireComponent(typeof(Rigidbody))]
   
    //! Transport Surface - this class is needed together with Drives to model conveyor systems. The transport surface is transporting
    //! rigid bodies which are colliding with its surface
    [HelpURL("https://game4automation.com/documentation/current/transportsurface.html")]
    public class TransportSurface : BaseTransportSurface
    {
        #region Public Variables

        public Vector3 TransportDirection; //!< The direction in local coordinate system of Transport surface - is initialized normally by the Drive
        public float TextureScale = 10; //!< The texture scale what influcences the texture speed - needs to be set manually 
  
        public bool Radial = false;
   
        public float speed = 0; //!< the current speed of the transport surface - is set by the drive 
        [InfoBox("Standard Setting for layer is g4a Transport")]
        [OnValueChanged("RefreshReferences")] 
        public string Layer = "g4a Transport";
        [InfoBox("For Best performance unselect UseMeshCollider, for good transfer between conveyors select this")]
        [OnValueChanged("RefreshReferences")] 
        public bool UseMeshCollider = false;
        public bool FollowParent = false; //!< needs to be set to true if transport surface has above a drive for linear or rotational movement

        #endregion

        #region Private Variables

        private MeshRenderer _meshrenderer;
        private Rigidbody _rigidbody;
        private Collider _collider;
        private bool _isMeshrendererNotNull;
        private Transform _parent;
        private Vector3 _distancepos;
        private Quaternion _distancerot;

        #endregion

        #region Public Methods

        //! Gets a center point on top of the transport surface
        public Vector3 GetMiddleTopPoint()
        {
            
            var collider = gameObject.GetComponent<Collider>();
            if (collider!=null)
            {
                var vec = new Vector3(collider.bounds.center.x, collider.bounds.center.y + collider.bounds.extents.y,
                    collider.bounds.center.z);
                return vec;
            }
            else
                return Vector3.zero;
        }

        //! Sets the speed of the transport surface (used by the drive)
        public void SetSpeed(float _speed)
        {
            speed = _speed;
        }

        #endregion

        #region Private Methods

        private void RefreshReferences()
        {
            var _mesh = GetComponent<MeshCollider>();
            var _box = GetComponent<BoxCollider>();
            if (UseMeshCollider)
            {
                if (_box!=null)
               
                    DestroyImmediate(_box);
                if (_mesh==null)
                {
                    _mesh = gameObject.AddComponent<MeshCollider>();
                }
            }
            else
            {
                if (_mesh!=null)
                    DestroyImmediate(_mesh);
                if (_box==null)
                {
                    _box= gameObject.AddComponent<BoxCollider>();
                }
            }
            _rigidbody = gameObject.GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            _collider = gameObject.GetComponent<Collider>();
            _meshrenderer = gameObject.GetComponent<MeshRenderer>();
        }


        private void Reset()
        {
            gameObject.layer = LayerMask.NameToLayer(Layer);
            RefreshReferences();

            // Add transport surface to drive if a drive is existing in this or an upper object
            var drive = gameObject.GetComponentInParent<Drive>();
            if (drive != null)
                drive.AddTransportSurface(this);
        }
        
        [Button("Destroy Transport Surface")]
        private void DestroyTransportSurface()
        {
            var drive = gameObject.GetComponentInParent<Drive>();
            if (drive != null)
                drive.RemoveTransportSurface(this);
            Object.DestroyImmediate(this);
        }
        

        void Start()
        {
            RefreshReferences();
            _isMeshrendererNotNull = _meshrenderer != null;
            Reset();
            SetSpeed(speed);


            if (FollowParent)
            {
                // get relation to parent

                _parent = transform.parent;
                _distancepos = transform.position - _parent.position;
                _distancerot = Quaternion.Inverse(_parent.rotation) * transform.rotation;

                transform.parent = null;
            }
        }

        void Update()
        {
            if (speed != 0) 
            {
                Vector3 mov = TextureScale * TransportDirection * Time.time * speed *
                              Game4AutomationController.SpeedOverride / Game4AutomationController.Scale;
                Vector2 vector2 = new Vector2(mov.x, mov.y);
                if (_isMeshrendererNotNull)
                {
                    _meshrenderer.material.mainTextureOffset = vector2;
                }
            }
        }
     
        void FixedUpdate()
        {

                if (!Radial)
                {
                    Vector3 newpos, mov;
                    newpos = _rigidbody.position;

                    // Linear Conveyor
                    if (FollowParent)
                    {
                        newpos = _parent.position + _distancepos;
                        _rigidbody.MovePosition(_parent.position+_distancepos);
                        _rigidbody.MoveRotation(_parent.rotation);
                        mov = _parent.rotation * TransportDirection * Time.fixedDeltaTime * speed *
                              Game4AutomationController.SpeedOverride /
                              Game4AutomationController.Scale;

                        _rigidbody.position = (newpos - mov);
                        _rigidbody.MovePosition(newpos + mov);
                    }
                    else
                    {
                        if (speed != 0)
                        {
                            mov = TransportDirection * Time.fixedDeltaTime * speed *
                                  Game4AutomationController.SpeedOverride /
                                  Game4AutomationController.Scale;
                            _rigidbody.position = (_rigidbody.position - mov);
                            _rigidbody.MovePosition(_rigidbody.position + mov);
                        }
                    }
                }
                else
                {
                    Quaternion nextrot;
                    // Radial Conveyor
                    if (FollowParent)
                    {
                        // Not implemented for Radial Conveyor - necessary?
                    }
                    else
                    {
                        if (speed != 0)
                        {
                            _rigidbody.rotation = _rigidbody.rotation * Quaternion.AngleAxis(
                                                      -speed * Time.fixedDeltaTime *
                                                      Game4AutomationController.SpeedOverride, TransportDirection);
                            nextrot = _rigidbody.rotation * Quaternion.AngleAxis(
                                          +speed * Time.fixedDeltaTime * Game4AutomationController.SpeedOverride,
                                          TransportDirection);
                            _rigidbody.MoveRotation(nextrot);
                        }
                    }
                }
           
        }
        #endregion
    }
}