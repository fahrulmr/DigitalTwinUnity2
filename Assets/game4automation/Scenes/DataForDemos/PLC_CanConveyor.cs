
namespace game4automation
{

    using UnityEngine;

    [SelectionBase]
    //! PLC Script for the game4automation demo model
    public class PLC_CanConveyor : ControlLogic
    {
        public bool On = true;

        [Header("References")] public PLCOutputBool StartConveyor;
        public PLCInputBool SensorOccupied;
        public PLCInputBool ButtonConveyorOn;
        public PLCOutputBool LampCanAtPosition;

        // Call this when all Updates are done
        void FixedUpdate()
        {

            if (SensorOccupied.Value == false && On && ButtonConveyorOn.Value == true)
            {
                StartConveyor.Value = true;
            }
            else
            {
                StartConveyor.Value = false;
            }

            LampCanAtPosition.Value = SensorOccupied.Value;
        }
    }
}
