// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace game4automation
{
    [System.Serializable]
    public class Game4AutomationEventMUSensor : UnityEvent<MU, bool>
    {
    }


    [SelectionBase]
    [RequireComponent(typeof(BoxCollider))]
    //! The sensor is used for tetecting MUs.
    //! Sensors are using Box Colliders for detecting MUs. The Sensor should be on Layer *g4aSensor* if the standard Game4Automation
    //! Layer settings are used. A behavior component (e.g. *Sensor_Standard*) must be added to the Sensor for providing connection to PLCs Input and 
    //! outputs.
    [HelpURL("https://game4automation.com/documentation/current/sensor.html")]
    public class Sensor : BaseSensor
    {
        // Public - UI Variables 
        [Header("Settings")] public string
            LimitSensorToTag; //!< Limits the function of the sensor to a certain MU tag - also MU names are working

        public bool DisplayStatus = true; //!<  Display the status of the sensor by changing the material (color).
        public Material MaterialOccupied; //!<  Material for displaying the occupied status.
        public Material MaterialNotOccupied; //!<  Material for displaying the not occupied status.
        public bool PauseOnSensor = false; //!<  Pause simulation if sensor is getting high - can be used for debuging

        [Header("Events")] public Game4AutomationEventMUSensor
            EventMUSensor; //!<  Unity event which is called for MU enter and exit. On enter it passes MU and true. On exit it passes MU and false.

        [Header("Sensor IO's")] public bool Occupied = false; //!<  True if sensor is occupied.
        public GameObject LastTriggeredBy; //!< Last MU which has triggered the sensor.
        public int LastTriggeredID; //!< Last MUID which has triggered the sensor.
        public int LastTriggeredGlobalID; //!<  Last GloabalID which has triggerd the sensor.
        public int Counter;
        public List<MU> CollidingMus; // Currently colliding MUs with the sensor.

        public List<GameObject>
            CollidingObjects; // Currently colliding GameObjects with the sensor (which can be more than MU because a MU can contain several GameObjects.


        public delegate void
            OnEnterDelegate(GameObject obj); //!< Delegate function for GameObjects entering the Sensor.

        public event OnEnterDelegate EventEnter;

        public delegate void OnExitDelegate(GameObject obj); //!< Delegate function for GameObjects leaving the Sensor.

        public event OnExitDelegate EventExit;


        // Private Variables
        private bool _occupied = false;
        private MeshRenderer _meshrenderer;
        private BoxCollider _boxcollider;


        //! Delete all MUs in Sensor Area.
        public void DeleteMUs()
        {
            var tmpcolliding = CollidingObjects;
            foreach (var obj in tmpcolliding.ToArray())
            {
                var mu = GetTopOfMu(obj);
                if (mu != null)
                {
                    Destroy(mu.gameObject);
                }

                CollidingObjects.Remove(obj);
            }
        }


        // Use this when Script is inserted or Reset is pressed
        private void Reset()
        {
            if (MaterialOccupied == null)
            {
                MaterialOccupied = UnityEngine.Resources.Load("SensorOccupiedRed", typeof(Material)) as Material;
            }

            if (MaterialNotOccupied == null)
            {
                MaterialNotOccupied = UnityEngine.Resources.Load("SensorNotOccupied", typeof(Material)) as Material;
            }

            GetComponent<BoxCollider>().isTrigger = true;
        }

        // Use this for initialization
        private void Start()
        {
            _boxcollider = GetComponent<BoxCollider>();
            _meshrenderer = _boxcollider.gameObject.GetComponent<MeshRenderer>();
        }


        // Shows Status of Sensor
        private void ShowStatus()
        {
            if (CollidingObjects.Count == 0)
            {
                LastTriggeredBy = null;
                LastTriggeredID = 0;
                LastTriggeredGlobalID = 0;
            }
            else
            {
                GameObject obj = CollidingObjects[CollidingObjects.Count - 1];
                if (!ReferenceEquals(obj, null))
                {
                    var LastTriggeredByMU = GetTopOfMu(obj);
                    if (!ReferenceEquals(LastTriggeredByMU, null))
                        LastTriggeredBy = LastTriggeredByMU.gameObject;
                    else
                        LastTriggeredBy = obj;

                    if (LastTriggeredByMU != null)
                    {
                        LastTriggeredID = LastTriggeredByMU.ID;
                        LastTriggeredGlobalID = LastTriggeredByMU.GlobalID;
                    }
                }
            }

            if (CollidingObjects.Count > 0)
            {
                _occupied = true;
                if (DisplayStatus && _meshrenderer != null)
                {
                    _meshrenderer.material = MaterialOccupied;
                }
            }
            else
            {
                _occupied = false;
                if (DisplayStatus && _meshrenderer != null)
                {
                    _meshrenderer.material = MaterialNotOccupied;
                }
            }

            Occupied = _occupied;
        }

        // ON Collission Enter
        private void OnTriggerEnter(Collider other)
        {
            GameObject obj = other.gameObject;
            var tmpcolliding = CollidingObjects;
            var muobj = GetTopOfMu(obj);

            if ((LimitSensorToTag == "" || ((muobj.tag == LimitSensorToTag) || muobj.Name == LimitSensorToTag)))
            {
                if (PauseOnSensor)
                    Debug.Break();
                if (!CollidingObjects.Contains(obj))
                    CollidingObjects.Add(obj);
            
            
                ShowStatus();

                if (EventEnter != null)
                    if (muobj != null)
                        EventEnter(muobj.gameObject);
                    else
                        EventEnter(obj);

                if (muobj != null)
                {
                    if (!CollidingMus.Contains(muobj))
                    {
                        if (EventEnter != null)
                            EventEnter(muobj.gameObject);
                        Counter++;
                        muobj.EventMUEnterSensor(this);
                        CollidingMus.Add(muobj);
                        EventMUSensor.Invoke(muobj, true);
                       
                    }
                }
            }
        }
        
        public void OnMUPartsDestroyed(GameObject obj)
        {
            CollidingObjects.Remove(obj);
        }

        // ON Collission Exit
        private void OnTriggerExit(Collider other)
        {
            GameObject obj = other.gameObject;
            if (!ReferenceEquals(obj, null))
            {
                var muobj = GetTopOfMu(obj);
                var tmpcolliding = CollidingObjects;
                var dontdelete = false;
                CollidingObjects.Remove(obj);

                // Check if remaining colliding objects belong to same mu
                foreach (var thisobj in CollidingObjects)
                {
                    var thismuobj = GetTopOfMu(thisobj);
                    if (thismuobj == muobj)
                    {
                        dontdelete = true;
                    }
                }

                if (!dontdelete)
                {
                    if (muobj != null)
                    {
                        CollidingMus.Remove(muobj);
                        if (EventExit != null)
                            EventExit(muobj.gameObject);
                        EventMUSensor.Invoke(muobj, false);
                        muobj.EventMUExitSensor(this);
                    }
                }
                
                ShowStatus();
            }
        }
    }
}