namespace game4automation
{

    using UnityEngine;

    [SelectionBase]
    //! PLC Script for the game4automation demo model
    public class PLC_Handling : ControlLogic
    {
        public bool On = true;

        [Header("References")] public PLCOutputFloat GantryYDestination;
        public PLCOutputBool GantryYStart;
        public PLCInputBool GantryYAtDestination;
        public PLCInputBool GantryYDriving;
        public PLCOutputFloat GantryZDestination;
        public PLCOutputBool GantryZStart;
        public PLCInputBool GantryZAtDestination;
        public PLCInputBool GantryZDriving;
        public PLCOutputBool OpenGripper;
        public PLCOutputBool CloseGripper;
        public PLCInputBool GripperOpened;
        public PLCInputBool GripperClosed;
        public PLCInputBool SensorCan;
        public PLCInputBool ButtonStartChangeCycle;
        public PLCInputBool ButtonHandlingOn;

        [Header("PLCs")] public PLC_BoxConveyor PlcBoxConveyor;

        [Header("Loader Positions")] public float PickCanPosY;
        public float PickCanPosZ;
        public float PlaceCanPosZ;
        public float PlaceCanPosY;
        public float TransportCanPosZ;

        public int NumberOfRows = 2;
        public int NumberOfColums = 5;
        public float DistanceCol = 100;
        public float DistancerRow = 100;

        [Header("Status")] public string StatusLoader = "waiting";
        public int CurrentRowNumber = 0;
        public int CurrentColNumber = 0;
        public int CycleNumber = 0;
        private float placepos;

        private float _delaystart;
        // Use this for initialization


        void Start()
        {

            StatusLoader = "waiting";
            CurrentRowNumber = 1;
            CurrentColNumber = 1;
            CycleNumber = 0;
        }

        void DriveLoaderTo(float posy, float posz)
        {
            GantryYDestination.Value = posy;
            GantryZDestination.Value = posz;
            GantryYStart.Value = true;
            GantryZStart.Value = true;
            Invoke("ResetDriveStart",0.5f);
        }

        void ResetDriveStart()
        {
            GantryYStart.Value = false;
            GantryZStart.Value = false;
        }

        bool LoaderAtDestination()
        {
            if (GantryYAtDestination.Value && GantryZAtDestination.Value && !GantryYStart.Value && !GantryZStart.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (SensorCan == null)
                return;
            // Exit if PLC is not Running
            if (!On)
            {
                return;
            }

         

            if (ButtonStartChangeCycle.Value == true)
            {
                CurrentRowNumber = 1;
                CurrentColNumber = 1;
                PlcBoxConveyor.BoxChangeCycle = true;
            }

            if (ButtonHandlingOn.Value == false)
            {
                return;
            }

            /// Event Chain for Loader
            switch (StatusLoader)
            {
                case "waiting":
                    if (SensorCan.Value)
                    {
                        OpenGripper.Value = true;
                        CloseGripper.Value = false;
                        DriveLoaderTo(PickCanPosY, PickCanPosZ);
                        StatusLoader = "drivingtopick";

                    }

                    break;
                case "drivingtopick":
                    if (LoaderAtDestination())
                    {
                        StatusLoader = "atpickposition";
                    }

                    break;
                case "atpickposition":
                    OpenGripper.Value = false;
                    CloseGripper.Value = true;
                    StatusLoader = "closinggripper";
                    break;
                case "closinggripper":
                    if (GripperClosed.Value == true)
                    {
                        StatusLoader = "lifting";
                        DriveLoaderTo(PickCanPosY, TransportCanPosZ);
                    }

                    break;
                case "lifting":
                    if (LoaderAtDestination())
                    {
                        StatusLoader = "drivingtoplace";
                        placepos = PlaceCanPosY + (CurrentColNumber - 1) * DistanceCol;
                        DriveLoaderTo(placepos, TransportCanPosZ);
                    }

                    break;
                case "drivingtoplace":
                    if (LoaderAtDestination() && PlcBoxConveyor.SensorGantryOccupied.Value == true &&
                        PlcBoxConveyor.StartConveyor.Value == false)
                    {
                        StatusLoader = "lifttingdown";
                        DriveLoaderTo(placepos, PlaceCanPosZ);
                    }

                    break;
                case "lifttingdown":
                    if (LoaderAtDestination())
                    {
                        StatusLoader = "atplaceposition";
                        OpenGripper.Value = true;
                        CloseGripper.Value = false;
                        StatusLoader = "openinggripper";

                        break;
                    }

                    break;
                case "openinggripper":
                    if (GripperOpened.Value == true)
                    {
                        if (_delaystart == 0) _delaystart = Time.time;
                        if (Time.time - _delaystart > 0.5F)
                        {
                            _delaystart = 0;
                            StatusLoader = "drivingupafterplace";
                            DriveLoaderTo(placepos, TransportCanPosZ);
                        }
                    }

                    break;
                case "drivingupafterplace":
                    if (LoaderAtDestination())
                    {
                        StatusLoader = "drivingtowaitpos";
                        DriveLoaderTo(PickCanPosY, TransportCanPosZ);


                        CurrentColNumber++;
                        if (CurrentColNumber > NumberOfColums)
                        {
                            CurrentColNumber = 1;
                            CurrentRowNumber++;
                            PlcBoxConveyor.NextRowDistance = DistancerRow;
                            PlcBoxConveyor.DriveNextRow = true;
                        }

                        if (CurrentRowNumber > NumberOfRows)
                        {
                            PlcBoxConveyor.BoxChangeCycle = true;
                            CurrentRowNumber = 1;
                        }

                        CycleNumber = CycleNumber + 1;
                    }

                    break;
                case "drivingtowaitpos":
                    if (LoaderAtDestination())
                    {

                        StatusLoader = "waiting";
                    }

                    break;

            }
        }
    }
}