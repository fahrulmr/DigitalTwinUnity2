// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using game4automationtools;
using UnityEngine;

namespace game4automation
{
    [RequireComponent(typeof(Drive))]
    public class StartDriveOnCondition : BehaviorInterface
    {
        public enum Condition {Greater, Smaller};
        [Header("Conditions")]
        public bool StartBasedOnOtherDrivesPositon;
        [ShowIf("StartBasedOnOtherDrivesPositon")]public Drive MonitoredDrive;
        [ShowIf("StartBasedOnOtherDrivesPositon")]public Condition ConditionToStartDrive;
        [ShowIf("StartBasedOnOtherDrivesPositon")]public float StartOnPosition;
        [ShowIf("StartBasedOnOtherDrivesPositon")]public float IncrementStartOnPositon;    
        [ShowIf("StartBasedOnOtherDrivesPositon")][ReadOnly]public float CurrentStartOnPositon;    
        
        public bool StartBasedOnSensor;
        [ShowIf("StartBasedOnSensor")] public Sensor MonitoredSensor;
        [ShowIf("StartBasedOnSensor")] public bool StartOnSensorHigh;
        
        [Header("Destination")] public float MoveThisDriveTo;
        public bool MoveIncremental;
        [ReadOnly] public float CurrentTarget;
    
            
        private bool monitoredDriveNotNull;
        private bool sensorNotNull;
        private bool lastsensor;
        private float lastdrivepos;
        private Drive thisdrive;
        
        // Start is called before the first frame update
        void Start()
        {
            thisdrive = GetComponent<Drive>();
            monitoredDriveNotNull = MonitoredDrive != null;
            sensorNotNull = MonitoredSensor != null;
            if (monitoredDriveNotNull)
                lastdrivepos = MonitoredDrive.CurrentPosition;
            if (sensorNotNull)
                lastsensor = MonitoredSensor.Occupied;
            CurrentStartOnPositon = StartOnPosition;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (MoveIncremental)
                CurrentTarget = thisdrive.CurrentPosition + MoveThisDriveTo;
            else
                CurrentTarget = MoveThisDriveTo;
            
            if (StartBasedOnOtherDrivesPositon && monitoredDriveNotNull)
            {
               
                
                if (ConditionToStartDrive == Condition.Greater)
                {
                    if (MonitoredDrive.CurrentPosition >= CurrentStartOnPositon &&
                        lastdrivepos < CurrentStartOnPositon)
                    {
                   
                        thisdrive.DriveTo(CurrentTarget);
                        if (IncrementStartOnPositon!=0)
                        {
                            CurrentStartOnPositon += IncrementStartOnPositon;
                        }
                    }
                }
                
                if (ConditionToStartDrive == Condition.Smaller)
                {
                    if (MonitoredDrive.CurrentPosition <= CurrentStartOnPositon &&
                        lastdrivepos > CurrentStartOnPositon)
                    {
          
                        thisdrive.DriveTo(CurrentTarget);
                        if (IncrementStartOnPositon!=0)
                        {
                            CurrentStartOnPositon += IncrementStartOnPositon;
                        }
                    }
                }
                lastdrivepos = MonitoredDrive.CurrentPosition;
            }


            if (StartBasedOnSensor && sensorNotNull)
            {

                if (MonitoredSensor.Occupied == StartOnSensorHigh && lastsensor != StartOnSensorHigh)
                {
                    thisdrive.DriveTo(CurrentTarget);
                }

                lastsensor = MonitoredSensor.Occupied;
            }
        }
    }

}

