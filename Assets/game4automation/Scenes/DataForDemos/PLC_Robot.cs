namespace game4automation
{

    using UnityEngine;

    [SelectionBase]
    //! PLC Script for the game4automation demo model
    public class PLC_Robot : ControlLogic
    {
        public bool On = true;


        [Header("Control")] public bool StartCycle = false;
        public bool CycleActive = false;
        public string StatusRobot = "waiting";

        [Header("References")] public Animator RobotAnimator;
        public Sensor SensorBox;
        public Grip Gripper;
        public GameObject RobotAxis6;
        public PLCOutputBool GripperOpen;
        public PLCInputBool GripperOpened;
        public PLCOutputBool GripperClose;
        public PLCInputBool GripperClosed;

        public PLCOutputBool PLCStartCycle;
        public PLCInputBool PLCCycleActive;

        public float Axis6Rotation;
        private bool _lastStartCycle = false;
        private float _startunload;
        public int CounterUnloaded = 0;

        void Start()
        {
            StatusRobot = "waiting";
            GripperOpen.Value = true;
            GripperClose.Value = false;
        }


        private bool AnimationFinished(string name)
        {
            if ((RobotAnimator.GetCurrentAnimatorStateInfo(0).IsName(name) &&
                 RobotAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Call this when all Updates are done
        void Update()
        {
            Vector3 rot = RobotAxis6.transform.rotation.eulerAngles;

            Axis6Rotation = rot.x;

            if (PLCStartCycle != null)
                StartCycle = PLCStartCycle.Value;


            if (!On) return;

            if (StartCycle && !_lastStartCycle && !CycleActive)
            {
                CycleActive = true;
            }

            if (CycleActive)
            {
                switch (StatusRobot)
                {
                    case "waiting":
                        if (PLCStartCycle != null)
                            PLCStartCycle.Value = false;
                        StatusRobot = "DriveToPick";
                        RobotAnimator.Play("MoveToPick", 0, 0);
                        break;
                    case "DriveToPick":
                        if (AnimationFinished("MoveToPick"))
                        {
                            StatusRobot = "CloseGripper";
                            GripperClose.Value = true;
                            GripperOpen.Value = false;
                        }

                        break;
                    case "CloseGripper":
                        if (GripperClosed.Value)
                        {
                            StatusRobot = "DroppingCans";
                            RobotAnimator.Play("LiftAndDump", 0, 0);
                            _startunload = Time.time;

                        }

                        break;
                    case "DroppingCans":

                        if (_startunload > 0 && Axis6Rotation > 70 && Axis6Rotation < 200)
                        {
                           
                            var box = Gripper.PickedMUs[0].GetComponent<MU>();
                            CounterUnloaded = CounterUnloaded + box.LoadedMus.Count;
                            box.UnloadAllMUs();
                         
                            _startunload = 0;
                        }

                        if (AnimationFinished("LiftAndDump"))
                        {
                            RobotAnimator.Play("PutBoxOnConveyor", 0, 0);
                            StatusRobot = "PlaceBox";
                        }

                        break;
                    case "PlaceBox":
                        if (AnimationFinished("PutBoxOnConveyor"))
                        {
                            StatusRobot = "OpeningGripper";
                            GripperClose.Value = false;
                            GripperOpen.Value = true;
                        }

                        break;
                    case "OpeningGripper":
                        if (GripperOpened.Value)
                        {
                            StatusRobot = "MovingToWaitPos";
                            RobotAnimator.Play("MoveOutFromPick", 0, 0);
                        }

                        break;
                    case "MovingToWaitPos":
                        if (AnimationFinished("MoveOutFromPick"))
                        {
                            StatusRobot = "waiting";
                            CycleActive = false;
                        }

                        break;
                }
            }

            if (PLCCycleActive != null)
                PLCCycleActive.Value = CycleActive;


            _lastStartCycle = StartCycle;
        }
    }
}
