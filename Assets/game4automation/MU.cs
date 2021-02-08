// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using System.Collections.Generic;
using game4automationtools;
using UnityEngine;

namespace game4automation
{
    [SelectionBase]
    //! Base class for free movable Unity (MU). MUs can be picked or loaded and they can be placed on Transport Surfaces. MUs are created by a Source and deleted by a Sink.
    [HelpURL("https://game4automation.com/documentation/current/mu.html")]
    public class MU : Game4AutomationBehavior
    {
        #region Public Attributes
        [ReadOnly] public int ID; //!<  ID of this MU (increases on each creation on the MU source
        [ReadOnly] public int GlobalID; //!< Global ID, increases for each MU independent on the source
        [ReorderableList] public List<GameObject> MUAppearences; //!< List of MU appearances for PartChanger
        [ReadOnly] public FixedJoint FixedToJoint; //!< Current Joint the part is fixed to
        [ReadOnly] public GameObject GrippedBy; //!< Current Gripper which is picking the part
        [ReadOnly] public GameObject LoadedOn; //!< Current Part the MU is loaded on
        [ReadOnly] public GameObject StandardParent; //!< The standard parent Gameobject of the MU
        [ReadOnly] public GameObject ParentBeforeGrip; //!< The parent of the MU before the last Grip
        [ReadOnly] public List<Sensor> CollidedWithSensors; //!< The current Sensors the Part is colliding with
        [ReadOnly] public List<MU> LoadedMus; //!< List of MUs whcih are loaded on this MU
        #endregion
        
        
        private Rigidbody _rigidbody;
        // Deletes all MUs which are loaded on MU as Subcomponent 
        // (but not RigidBodies which are standing on this MU)

        #region Public Methods

        //  Places the part with the Bottom on top of the defined position
        public void PlaceMUOnTopOfPosition(Vector3 position)
        {
            Bounds bounds = new Bounds(transform.position,new Vector3(0,0,0));
            
            // Calculate Bounds
         
            Renderer[] renderers = GetComponentsInChildren<Renderer> ();
            foreach (Renderer renderer in renderers)
            {
                   bounds.Encapsulate (renderer.bounds);
            }
            
            // get bottom center
            var center = new Vector3(bounds.min.x+bounds.extents.x,bounds.min.y,bounds.min.z+bounds.extents.z);
            
            // get distance from center to bounds
            var distance = transform.position - center;

            transform.position = position + distance;
        }
        
        //! Load the named MU on this mu
        public void LoadMu(MU mu)
        {
            mu.transform.SetParent(this.transform);
            mu.EventMuLoad();
            LoadedMus.Add(mu);
        }
        
        //! Event that this called when MU enters sensor
        public void EventMUEnterSensor(Sensor sensor)
        {
            CollidedWithSensors.Add(sensor);
        }
        
        //! Event that this called when MU enters sensor
        public void EventMUExitSensor(Sensor sensor)
        {
            CollidedWithSensors.Remove(sensor);
        }


        //! Event that this MU is loaded onto another
        public void EventMuLoad()
        {
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            LoadedOn = transform.parent.gameObject;
        }

        //! Event that this MU is unloaded from another
        public void EventMUUnLoad()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            transform.parent = StandardParent.transform;
            LoadedOn = null;
            _rigidbody.WakeUp();
        }

        //  Init the MU wi MUName and IDs
        public void InitMu(string muname, int localid, int globalid)
        {
            ID = localid;
            GlobalID = globalid;
            name = muname;
            if (transform.parent != null)
            {
                StandardParent = transform.parent.gameObject;
            }
        }


        // !Unloads one of the MUs which are loaded on this MU
        public void UnloadOneMu(MU mu)
        {
            mu.EventMUUnLoad();
            LoadedMus.Remove(mu);
        }

        // !Unloads all  of the MUs which are loaded on this MU
        public void UnloadAllMUs()
        {
            var tmploaded = LoadedMus.ToArray();
            foreach (var mu in tmploaded)
            {
                UnloadOneMu(mu);
            }
        }
        #endregion
        
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }
}