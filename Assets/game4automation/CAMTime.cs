using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using game4automationtools;
using UnityEngine.UIElements;


namespace game4automation
{
    [HelpURL("https://game4automation.com/documentation/current/cam.html")]
    //! CAM for moving drives based on CAM profiles 
    public class CAMTime : BaseCAM
    {
        public class campoint
        {
            public float master;
            public float slave;
        }
        [Header("Scaling and Offset")]
        public float CAMTimeScale=1; //!< An scale of the Time makes CAM faster or slowér
        public float CAMAxisScale=1; //!< The scale of the CAM axis. It will scale the values of the CAM curve.
        public float CAMAxisOffset=0; //!< The offset of the CAM axis. It will be a offset to the design position
        
        [Header("Start by Master Drive")]
        public Drive MasterDrive;  //!< The master drive this slave drive is attached to
        public float StartOnMasterPosGreaterThan;
        
        [Header("CAM Curve")]
        public AnimationCurve CamCurve; //!< The Animation Curve which is defining the slave drive position in relation to the master drive position
        
        private char lineSeperater = '\n'; //!< It defines line seperate character
        private char fieldSeperator = ','; //!< It defines field seperate chracter

        public TextAsset CamDefintion;  //!< A text assed containing the CAM definition. This asset is a table with optional headers and columns describing the master axis position and the slave axis position.
        
        public bool UseColumnNames; //!< If true the Column Names are used to define the data to import
        [ShowIf("UseColumnNames")]
        public string MasterTime; //!< The master axis column name
        [ShowIf("UseColumnNames")]
        public string AxisColumn; //!< The slave axis column name
        
        public bool UseColumnNumbers; //!< If true the Column Numbers (starting with 1 for the 1st column) are used to define the data to import
        [ShowIf("UseColumnNumbers")]
        public int TimeColumnNum=1;  //!< The master axis column number
        [ShowIf("UseColumnNumbers")]
        public int AxisColumnNum=2; //!< The slave axis column number

        public bool CamDefinitionWithHeader = true; //!< if true during import a column header is expected and first line of the imported data should be a header
        public bool ImportOnStart = false; //! if true text asset is always imported on simulation start
        private List<campoint> camdata;

        [Header("Start next CAM")] public CAMTime StartCamWhenFinished;
        
        [Header("CAM IO's")] public bool StartCAM;
        [ReadOnly] public bool IsActive;
        [ReadOnly] public bool IsFinished;
        [ReadOnly] public float CurrentCAMTime;

     
        [Header("PLC IO's")] public PLCOutputBool PLCStartCAM;
        public PLCOutputFloat PLCScaleCAM;
        public PLCInputBool PLCCAMIsRunning;
        public PLCInputBool PLCCAMIsFinished;
        public PLCInputFloat PLCCurrentCAMTime;
        
        private Drive _slave;
        private Drive _master;
        private bool _mastercontrolled;
        private float starttime = 0;
        private float laststoppedmaster = 0;
        private bool _startbefore;
        private bool _isPLCStartCamNotNull;
        private bool _isPLCCAMIsRunningNotNull;
        private bool _isPLCCurrentCAMTimeNotNull;
        private bool _isPLCCAMIsFinishedNotNull;

        [Button("Import CAM")]
        public void ImportCam()
        {
            CamCurve = new AnimationCurve();
            ImportCAMFile();
            
            foreach (var campoint in camdata)
            {
                CamCurve.AddKey(campoint.master, campoint.slave);
            }
            
            // Set the tangents
            for (int i = 1; i < CamCurve.keys.Length-1; i++)
            {
                Keyframe key = CamCurve[i];
                key.inTangent = 1;
                key.outTangent = 0;
                CamCurve.MoveKey(i,key);
            }
        
        }
        
        public static float GetFloat(string value, float defaultValue)
        {
            float result;

            if (!float.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = defaultValue;
            }
            
            
            return result;
        }
      
        private void ImportCAMFile()
        {
            string[] header = new[] {""};
            camdata = new List<campoint>();
            
            var csvFile = CamDefintion.text;
            
            if (csvFile=="")
                csvFile = System.Text.Encoding.UTF8.GetString(CamDefintion.bytes);
            
            string[] lines = csvFile.Split (lineSeperater);
            var isheader = true;
            foreach (string line in lines)
            {
                if (isheader && CamDefinitionWithHeader)
                {
                    header = line.Split(fieldSeperator);
                    // Clean Header
                    for (int i = 0; i < header.Length; i++)
                    {
                        header[i] = header[i].Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                    }

                  
                    isheader = false;
                }
                else
                {
                    var campoint = new campoint();
                    
                    string[] fields = line.Split(fieldSeperator);
                    var fieldcol = 0;
                    foreach (var field in fields)
                    {
                        try
                        {
                            if (UseColumnNames && CamDefinitionWithHeader)
                            {
                                if (header[fieldcol] == MasterTime)
                                {
                                    campoint.master = GetFloat(field,0);
                                }

                                if (header[fieldcol] == AxisColumn)
                                {
                                    campoint.slave = GetFloat(field,0);
                                }
                            }

                            if (UseColumnNumbers)
                            {
                                if (fieldcol+1 == TimeColumnNum)
                                {
                                    campoint.master = GetFloat(field,0);
                                }

                                if (fieldcol+1 == AxisColumnNum)
                                {
                                    campoint.slave = GetFloat(field,0);
                                }
                            }

                            fieldcol++;
                        }
                        catch (Exception e)
                        {
                            Error(e.Message);
                        }
                    }
                    camdata.Add(campoint);
                }
                
            }
        }

        public void StartTimeCAM()
        {
            IsActive = true;
            starttime = Time.time;
            IsFinished = false;
         
        }
        
        void StopTimeCAM()
        {
            IsActive = false;
            starttime = 0;
            IsFinished = true;
            if (_mastercontrolled)
                 laststoppedmaster = MasterDrive.CurrentPosition;
        }

        new void Awake()
        {
            _slave = GetComponent<Drive>();
            _isPLCStartCamNotNull =  PLCStartCAM != null;
            _isPLCCAMIsRunningNotNull =  PLCCAMIsRunning != null;
            _isPLCCurrentCAMTimeNotNull =  PLCCurrentCAMTime != null;
            _isPLCCAMIsFinishedNotNull =  PLCCAMIsFinished != null;
                
            if (MasterDrive != null)
                _mastercontrolled = true;
            else
                _mastercontrolled = false;
            if (ImportOnStart)
                ImportCam();
            IsActive = false;
            laststoppedmaster = 9999999f;
            base.Awake();
        }
        
        // Update is called once per frame
        void FixedUpdate()
        {
            // Set PLCOutputs if available
            if (_isPLCStartCamNotNull)
                StartCAM = PLCStartCAM.Value;
            
            if (_mastercontrolled && !IsActive)
            {
                if (MasterDrive.CurrentPosition < laststoppedmaster)
                {
                    if ((MasterDrive.CurrentPosition >= StartOnMasterPosGreaterThan)) 
                    {
                        StartTimeCAM();
                    }
                }
            }

            if (StartCAM && !_startbefore)
            {
                StartTimeCAM();
            }

            if (IsActive)
            {
                CurrentCAMTime = Time.time - starttime;
                CurrentCAMTime = CurrentCAMTime * CAMTimeScale;
                var Length = CamCurve.length;
                var lastkey = CamCurve.keys[Length-1];
                var maxtime = lastkey.time;

                if (CurrentCAMTime >= maxtime)
                {
                    _slave.CurrentPosition = lastkey.value;
                   StopTimeCAM();
                   if (StartCamWhenFinished!=null)
                       StartCamWhenFinished.StartTimeCAM();
                }
                else
                {
                    _slave.CurrentPosition = CamCurve.Evaluate(CurrentCAMTime)*CAMAxisScale+CAMAxisOffset;
                }
            }

            _startbefore = StartCAM;
            
            // Set PLCInputs if a vailable
            if (_isPLCCAMIsRunningNotNull)
                PLCCAMIsRunning.Value = IsActive;
            if (_isPLCCurrentCAMTimeNotNull)
                PLCCurrentCAMTime.Value = CurrentCAMTime;
            if (_isPLCCAMIsFinishedNotNull)
                PLCCAMIsFinished.Value = IsFinished;

        }
    }
}
