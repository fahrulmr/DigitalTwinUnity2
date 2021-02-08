using UnityEngine;

namespace game4automation
{
    

    [SelectionBase]
    //! PLC Script for the game4automation demo model
    public class PLC_BoxConveyor : ControlLogic
    {
        public bool On = true;


        [Header("Control from Handling")] public bool DriveNextRow=false;
        public float NextRowDistance;
        public bool BoxChangeCycle = false;
        [Header("References")] public PLC_Robot PLCRobot;
        public Sink SinkCans;
        public PLCOutputBool StartConveyor;
        public PLCInputBool SensorGantryOccupied;
        public PLCInputBool SensorRobotOccupied;
        public PLCInputFloat ConveyorPosition;
        public PLCOutputBool StartConveyorBackwards;
        public PLCOutputBool LampBoxAtPosition;
        public PLCOutputBool LampBoxChangeCycle;
    
        private bool _lastDriveNextCol = false;
        private float _dest;
        private bool _lastRobotCycle=false;
        private bool _cyclestarted =false;
        private bool _lastSensorRobot;
        void Start()
        {
            DriveNextRow = false;
            _dest = 0;
        }

        // Call this when all Updates are done
        void FixedUpdate()
        {
            if (!On) return;

            // Start Drive Next Col
            if (_lastDriveNextCol == false && DriveNextRow && !BoxChangeCycle)
            {
                _dest = ConveyorPosition.Value + NextRowDistance;
                StartConveyor.Value = true;
            }

            if (DriveNextRow && ConveyorPosition.Value >= _dest)
            {
                DriveNextRow = false;
                StartConveyor.Value = true;
                _dest = 0;
            }
        
            if (!DriveNextRow && BoxChangeCycle && !PLCRobot.StartCycle)
            {
                if (SensorRobotOccupied.Value == false && On)
                {
                    StartConveyorBackwards.Value = true;
                    StartConveyor.Value = false;
                    _cyclestarted = true;
                    _lastRobotCycle = false;
                }
                else
                {
            
                    StartConveyorBackwards.Value = false;
                    StartConveyor.Value = false;
                    PLCRobot.StartCycle = true;
                }
            }

            if (_lastRobotCycle==true && PLCRobot.CycleActive==false && _cyclestarted==true)
            {
                PLCRobot.StartCycle = false;
                BoxChangeCycle = false;
                _cyclestarted = false;
            }

            if (!DriveNextRow && !BoxChangeCycle)
            {
                if (SensorGantryOccupied.Value == false && On)
                {
                    StartConveyor.Value = true;
                    StartConveyorBackwards.Value = false;
                }
                else
                {
                    StartConveyor.Value = false;
                    StartConveyorBackwards.Value = false;
                }
            }

            _lastDriveNextCol = DriveNextRow;
            _lastSensorRobot = SensorRobotOccupied.Value;
            _lastRobotCycle = PLCRobot.CycleActive;
            LampBoxAtPosition.Value = SensorGantryOccupied.Value;
            LampBoxChangeCycle.Value = BoxChangeCycle;
        }
    }
}
