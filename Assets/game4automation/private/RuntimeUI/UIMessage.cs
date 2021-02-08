// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

#if CINEMACHINE
using Cinemachine;
#endif
using game4automation;
using UnityEngine;
using UnityEngine.UI;

namespace game4automation
{
    [SelectionBase]
    //! Modal UI Message box which can be opened by a PLC signal. Can be used for example for displaying warnings.
    public class UIMessage : Game4AutomationUI
    {
        [Header("Settings")] public string Header; //!<  The Header text of the message
        public string Message; //!< The message itself
        public float AutoCloseAfterSeconds; //!< Close the message after the defined time in seconds. If seconds is 0 the message is not automatically closed.
#if CINEMACHINE
        public CinemachineVirtualCamera VirtualCamera; //!< The cinemachine camera which will zoom to an object in the scene when the message is displayed (optional)
#endif
        public GameObject[] HightlightObjects; //!< A collection of objects which should be highlighed during the message.
        public float HighLightRedCol; //!< The highlight color in RGB - red.
        public float HighLightGreenCol; //!< The highlight color in RGB - green.
        public float HighLightBlueCol; //!<  The highlight color in RGB - blue.
        public float FlashDuration; //!< The duration of the highlight flash.
        public bool Display = false; //!< Displays the message
    
        [Header("Drive IO's")] public PLCOutputBool DisplayMessage; //!< The PLCOutput signal which is connected to the message.

        private float _timestartmessage;
        private GameObject _canvas;
        private Text _header;
        private Text _text;

        private bool _enabled;
        private bool _flashon;
        private bool _displaybefore;

        private float _startflash;
        // Start is called before the first frame update

        //! Close the message
        public void OKPressed()
        {
            enableMessage(false);
        }
    
        //! Enable the message or disable the message
        public void enableMessage(bool enable)
        {
            if (enable)
            {
                _header.text = Header;
                _enabled = true;
                _text.text = Message;
                _canvas.SetActive(true);
                _timestartmessage = Time.realtimeSinceStartup;
                _startflash = Time.time;
                _flashon = true;
#if CINEMACHINE
                if (VirtualCamera != null)
                {
                    VirtualCamera.enabled = true;
                }
#endif
                if (HightlightObjects != null)
                {
                    for (int i = 0; i < HightlightObjects.Length; i++)
                    {
                        if (HightlightObjects[i] != null)
                        {
                            var color = new Color(HighLightRedCol, HighLightGreenCol, HighLightBlueCol, 1);
                            SetColor(HightlightObjects[i], color);
                        }
                    }
                }
            }
            else
            {
                _timestartmessage = 0;
                _flashon = false;
                _enabled = false;
                _canvas.SetActive(false);
#if CINEMACHINE
                if (VirtualCamera != null)
                {
                    VirtualCamera.enabled = false;
                }
#endif
                
                for (int i = 0; i < HightlightObjects.Length; i++)
                {
                    if (HightlightObjects[i] != null)
                        ResetColor(HightlightObjects[i]);
                }
            }
        }


        void Start()
        {
            _canvas = transform.Find("Canvas").gameObject;
            _header = transform.Find("Canvas/Box/Header").GetComponent<Text>();
            _text = transform.Find("Canvas/Box/Message").GetComponent<Text>();
            enableMessage(false);
            _displaybefore = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Display  && _displaybefore == false)
            {
                enableMessage(true);
            }
            
            if (Display== false  && _displaybefore)
            {
                enableMessage(false);
            }

            if (DisplayMessage != null)
            {
                if (DisplayMessage.Value == true && _enabled == false)
                {
                    enableMessage(true);
                }
            }

            if (_enabled && AutoCloseAfterSeconds > 0 &&
                (Time.realtimeSinceStartup - _timestartmessage > AutoCloseAfterSeconds))
            {
                enableMessage(false);
            }

            // Flashing
            if (FlashDuration > 0 && _enabled)
            {
                if (Time.time - _startflash > FlashDuration)
                {
                    if (HightlightObjects != null)
                    {
                        for (int i = 0; i < HightlightObjects.Length; i++)
                        {
                            if (HightlightObjects[i] != null)
                            {
                                if (_flashon)
                                {
                                    var color = new Color(HighLightRedCol, HighLightGreenCol, HighLightBlueCol, 1);
                                    SetColor(HightlightObjects[i], color);
                                }
                                else
                                {
                                    ResetColor(HightlightObjects[i]);
                                }

                                _flashon = !_flashon;
                                _startflash = Time.time;
                            }
                        }
                    }
                }
            }

            _displaybefore = Display;
        }
    }
}