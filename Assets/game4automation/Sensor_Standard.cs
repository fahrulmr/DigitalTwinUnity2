// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEngine;

namespace game4automation
{
    [RequireComponent(typeof(Sensor))]
    //! The Sensor_Standard component is providing the Sensor behavior and connection to the PLC inputs and outputs.
    public class Sensor_Standard : BehaviorInterface
    {
       
        [Header("Settings")] public bool NormallyClosed = false;  //!< Defines if sensor signal is *true* if occupied (*NormallyClosed=false*) of if signal is *false* if occupied (*NormallyClosed=true*)
        [Header("Interface Connection")] public PLCInputBool Occupied; //! Boolean PLC input for the Sensor signal.

        private Sensor Sensor;
        private bool _isOccupiedNotNull;

        // Use this for initialization
        void Start()
        {
            _isOccupiedNotNull = Occupied != null;
            Sensor = GetComponent<Sensor>();
        }

        // Update is called once per frame
        void Update()
        {
            bool occupied = false;

            // Set Behavior Outputs
            if (NormallyClosed)
            {
                occupied = !Sensor.Occupied;
            }
            else
            {
                occupied = Sensor.Occupied;
            }

            // Set external PLC Outputs
            if (_isOccupiedNotNull)
                Occupied.Value = occupied;

        }
    }
}