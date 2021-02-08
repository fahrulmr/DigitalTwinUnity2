// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using System;
using game4automation;
using UnityEngine;

namespace game4automation
{
    [RequireComponent(typeof(Drive))]
    //! Behavior model of a cylinder movement which can be connected to a Drive.
    //! The cylinder is defined by a maximum (*MaxPos*) and minimum (*MinPos*) position in millimeters in relation to the zero position. The speed of the time for moving the cylinder out and in is defined in seconds by the properties *TimeOut* and *TimeIn*.
    [HelpURL("https://game4automation.com/documentation/current/drivebehaviour.html")]  
    public class Drive_Cylinder : BehaviorInterface
    {
       
        [Header("Settings")] public float MinPos = 0; //!< Minimum position in millimeters of the cylinder.
        public float MaxPos = 100; //!< Maximumposition in millimeters of the cylinder.
        public float TimeOut = 1; //!< Time for moving out from minimum position to maximum position in seconds.
        public float TimeIn = 1;  //!< Time for moving in from maximum position to minimum position in seconds.
        public Sensor StopWhenDrivingToMin; //!< Sensor for stopping the cylinder before reaching the min position (optional)
        public Sensor StopWhenDrivingToMax; //!< Sensor for stopping the cylinder before reaching the max position (optional)

        [Header("Behavior Signals")] public bool _out = false; //!< true for moving the cylinder out.
        public bool _in = false; //!< true for moving the cylinder in.
        public bool _isOut = false; //!< is true when cylinder is out or stopped by Max sensor.
        public bool _isIn = false; //!<  is true when cylinder is in or stopped by Min sensor.
        public bool _movingOut = false; //!<  is true when cylinder is currently moving out
        public bool  _movingIn = false; //!<  is true when cylinder is currently moving in
        public bool _isMax = false; //!< is true when cylinder is at maximum position.
        public bool _isMin = false; //!< is true when cylinder is at minimum position.
    
        [Header("PLC IOs")] public PLCOutputBool Out; //!< Signal for moving the cylinder out
        public PLCOutputBool In; //!< Signal for moving the cylinder in
        public PLCInputBool IsOut; //!<  Signal when the cylinder is out or stopped by Max sensor.
        public PLCInputBool IsIn; //!<  Signal when the cylinder is in or stopped by Max sensor.
        public PLCInputBool IsMax; //!< Signal is true when the cylinder is at Max position.
        public PLCInputBool IsMin; //!< Signal is true when the cylinder is at Min position.
        public PLCInputBool IsMovingOut; //!<  Signals is true when the cylinder is moving in.
        public PLCInputBool IsMovingIn; //!<  Signal is true when the cylinder is moving out.

        // Event Cylinder Reached Min Position
        public delegate void OnMinDelegate();   //!< Delegate function which is called when cylinder is at Min 
        public event OnMinDelegate EventOnMin;
        // Event Cylinder Reached Max Position
        public delegate void OnMaxDelegate();    //!< Delegate function which is called when cylinder is at Max.
        public event OnMaxDelegate EventOnMax;
        
        private float _oldposition;
        private Drive Cylinder;  
        private bool _oldin, _oldout;
        private bool _isIsInNotNull;
        private bool _isIsOutNotNull;
        private bool _isIsMinNotNull;
        private bool _isIsMaxNotNull;
        private bool _isIsMovingInNotNull;
        private bool _isIsMovingOutNotNull;
        private bool _isStopWhenDrivingToMaxNotNull;
        private bool _isStopWhenDrivingToMinNotNull;
        private bool _isInNotNull;
        private bool _isOutNotNull;

        // Use this for initialization
        void Start()
        {
            _isOutNotNull = Out != null;
            _isInNotNull = In != null;
            _isStopWhenDrivingToMinNotNull = StopWhenDrivingToMin != null;
            _isStopWhenDrivingToMaxNotNull = StopWhenDrivingToMax != null;
            _isIsMovingOutNotNull = IsMovingOut != null;
            _isIsMovingInNotNull = IsMovingIn != null;
            _isIsMaxNotNull = IsMax != null;
            _isIsMinNotNull = IsMin != null;
            _isIsOutNotNull = IsOut != null;
            _isIsInNotNull = IsIn != null;
            Cylinder = GetComponent<Drive>();
    
            Cylinder.CurrentPosition = MinPos;
            _isMin = true;
            _isMax = false;
            _isIn = true;
            _isOut = false;
            _movingIn = false;
            _movingOut = false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // Get external Signals
            if (_isInNotNull)
                _in = In.Value;
            if (_isOutNotNull)
                _out = Out.Value;
   
            // Moving Stopped at Min or Maxpos
            if (_movingOut && Cylinder.CurrentPosition == MaxPos)
            {
                _movingOut = false;
                _movingIn = false;
                _isOut = true;
            }
            if (_movingIn && Cylinder.CurrentPosition == MinPos)
            {
                _movingIn = false;
                _movingOut = false;
                _isIn = true;
            }
        
            // Stop on Collision
            if (_isStopWhenDrivingToMinNotNull && _movingIn)
            {
                if (StopWhenDrivingToMin.Occupied)
                {
                    _movingIn = false;
                    _movingOut = false;
                    _isIn = true;
                    Cylinder.Stop();
                }
            }
            if (_isStopWhenDrivingToMaxNotNull && _movingOut)
            {
                if (StopWhenDrivingToMax.Occupied)
                {
                    _movingIn = false;
                    _movingOut = false;
                    _isOut = true;
                    Cylinder.Stop();
                }
            }
        
            // At Maxpos
            if (Cylinder.CurrentPosition == MaxPos)
                _isMax = true;
            else
                _isMax = false;

            // At Minpos
            if (Cylinder.CurrentPosition == MinPos)
                _isMin = true;
            else
                _isMin = false;
        
            // EventMaxPos
            if (Cylinder.CurrentPosition == MaxPos && _oldposition != Cylinder.CurrentPosition && EventOnMax != null)
                EventOnMax();
        
            // EventMinPos
            if (Cylinder.CurrentPosition == MinPos && _oldposition != Cylinder.CurrentPosition && EventOnMin != null)
                EventOnMin();

        
            // Start to Move Cylinder
            if (!(_out && _in))
            {
                if (_out && !_isOut && !_movingOut)
                {
                    Cylinder.TargetSpeed = Math.Abs(MaxPos - MinPos) / TimeOut;
                    Cylinder.DriveTo(MaxPos);
                    _movingOut = true;
                    _movingIn = false;
                    _isIn = false;
                }
                if (_in && !_isIn && !_movingIn)
                {
                    Cylinder.TargetSpeed = Math.Abs(MaxPos - MinPos) / TimeIn;
                    Cylinder.DriveTo(MinPos);
                    _isOut = false;
                    _movingIn = true;
                    _movingOut = false;
                }
            }
        
            // Set external Signals
            if (_isIsInNotNull)
                IsIn.Value = _isIn;
            if (_isIsOutNotNull)
                IsOut.Value = _isOut;
            if (_isIsMinNotNull)
                IsMin.Value = _isMin;
            if (_isIsMaxNotNull)
                IsMax.Value = _isMax;
            if (_isIsMovingInNotNull)
                IsMovingIn.Value = _movingIn;
            if (_isIsMovingOutNotNull)
                IsMovingOut.Value = _movingOut;

            _oldposition = Cylinder.CurrentPosition;
            _oldout = _out;
            _oldin = _in;
        }
    }
}