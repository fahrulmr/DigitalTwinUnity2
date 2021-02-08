

using UnityEngine;
#if CINEMACHINE
using Cinemachine;
#endif

namespace game4automation
{
    public class VirtualCameraController : Game4AutomationBehavior
    {
#if CINEMACHINE
  
        private GameObject CameraLoader;
        private GameObject CameraRobot;
        private GameObject CameraTrack;
        private GameObject MainCamera;
        public PLCOutputBool PLCErrorMessage;
        public PLC_Handling PLCHandling;
        public PLC_BoxConveyor PLCBoxConveyor;

        public float ErrorMessageAllSeconds = 20;

        public int NextCycleNumber = 2;
        public int StartTrackCycle = 1;

        private bool _boxchange;
        private float _lasterrormessage;
        private CinemachineBrain _brain;
    
        private CinemachineVirtualCamera _CameraLoader;
        private CinemachineVirtualCamera _CameraRobot;
        private CinemachineVirtualCamera _CameraTrack;
        private CinemachineVirtualCamera _MainCamera;

        // Start is called before the first frame update
        void Start()
        {
            
         
            
            CameraRobot = GetChildByName("CameraRobot");
            CameraLoader = GetChildByName("CameraLoader");
            CameraTrack = GetChildByName("CameraTrack");
            _CameraRobot = CameraRobot.GetComponent<CinemachineVirtualCamera>();
            _CameraLoader = CameraLoader.GetComponent<CinemachineVirtualCamera>();
            _CameraTrack = CameraTrack.GetComponent<CinemachineVirtualCamera>();
      
            if (_CameraLoader != null)
            {
                _CameraLoader.enabled = false;
            }

            if (_CameraRobot != null)
            {
                _CameraRobot.enabled = false;
            }
            _brain = GameObject.Find("Main Camera").GetComponent<CinemachineBrain>();
        
            if (_CameraTrack != null)
            {
                _CameraTrack.enabled = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (PLCBoxConveyor.BoxChangeCycle && _boxchange == false)
            {
                _CameraLoader.enabled = false;
                _CameraTrack.enabled = false;
                _CameraRobot.enabled = true;
                _boxchange = true;
            }

            if (!PLCBoxConveyor.BoxChangeCycle && _boxchange == true)
            {
                _CameraTrack.enabled = true;
                _CameraLoader.enabled = false;
                _CameraRobot.enabled = false;
                NextCycleNumber = PLCHandling.CycleNumber;
                _boxchange = false;
            }

            if (!_boxchange)
            {
                if (PLCHandling.CycleNumber >= NextCycleNumber && _CameraTrack.enabled == true)
                {
                    _CameraLoader.enabled = true;
                    _CameraTrack.enabled = false;
                    StartTrackCycle = NextCycleNumber + 2;
                    NextCycleNumber = NextCycleNumber + 4;
                }

                if (PLCHandling.CycleNumber >= StartTrackCycle && _CameraTrack.enabled == false)
                {
                    _CameraLoader.enabled = false;
                    _CameraTrack.enabled = true;
                }
            }

            // if Animation enabled show Error Messages
            if (_brain.enabled && !_CameraRobot.enabled)
            {
                if (Time.time - _lasterrormessage > 1)
                {
                    PLCErrorMessage.Value = false;
                }

                if (Time.time - _lasterrormessage > ErrorMessageAllSeconds)
                {
                    PLCErrorMessage.Value = true;
                    _lasterrormessage = Time.time;
                }
            }
        }
#endif
    }
}