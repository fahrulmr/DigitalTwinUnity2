// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using game4automation;
using UnityEngine;

namespace game4automation
{
    [RequireComponent(typeof(Drive))]
    //! Behavior model of a cylinder movement which can be connected to a Drive.
    //! The cylinder is defined by a maximum (*MaxPos*) and minimum (*MinPos*) position in millimeter
    [HelpURL("https://game4automation.com/documentation/current/drivebehaviour.html")]
    public class Drive_Simple : BehaviorInterface
    {
       
        [Header("Settings")] public float ScaleSpeed = 1;  //!< Scale factor for the input speed

        [Header("PLC IOs")] public PLCOutputFloat Speed; //!< PLCOutput for the speed of the drive in millimeter / second, can be scaled by Scale factor.
        public PLCOutputBool Forward; //!< Signal to move the drive forward
        public PLCOutputBool Backward; //!< Signal to move the drive backward
        public PLCInputFloat IsAtPosition; //!< Signal for current position of the drive (in millimeter).
        public PLCInputBool IsDriving; //!< Signal is true if Drive is driving.

        private Drive Drive;
        private bool _isSpeedNotNull;
        private bool _isIsAtPositionNotNull;
        private bool _isForwardNotNull;
        private bool _isBackwardNotNull;
        private bool _isIsDrivingNotNull;

        // Use this for initialization
        void Start()
        {
            _isIsDrivingNotNull = IsDriving != null;
            _isBackwardNotNull = Backward != null;
            _isForwardNotNull = Forward != null;
            _isIsAtPositionNotNull = IsAtPosition != null;
            _isSpeedNotNull = Speed != null;
            Drive = GetComponent<Drive>();
        }

        // Update is called once per frame
        void Update()
        {
            // Get external PLC Outputs
            if (_isSpeedNotNull)
                Drive.TargetSpeed  = Speed.Value* ScaleSpeed;
            if (_isForwardNotNull)
                Drive.JogForward = Forward.Value;
            if (_isBackwardNotNull)
                Drive.JogBackward = Backward.Value;
        
            // Set external PLC Outpits
            if (_isIsAtPositionNotNull)
                IsAtPosition.Value = Drive.CurrentPosition;
            if (_isIsDrivingNotNull)
                IsDriving.Value = Drive.IsRunning;
        }
    }
}