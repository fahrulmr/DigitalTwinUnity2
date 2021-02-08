// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEngine;
using UnityEngine.UI;

namespace game4automation
{
    public enum BUTTONCOLOR
    {
        White,
        Yellow,
        Green,
        Red,
    }

    [SelectionBase]
    //! Pushbutton that can be connected to a PLCInput
    public class UIButton: Game4AutomationUI
    {
        [Header("Settings")] public string ButtonText; //!< The text of the button 
        public BUTTONCOLOR color; //!< the color of the button
        public bool NormallyOpenend = true; //!< true if the PLCOutput should be true if the Button is pressed
        public bool IsToggle = false;
        
        [Header("Status")] 
        public bool IsOn; //! Shows the status of the Pushbutton. Can be changed externally to change the status
        
        [Header("Button Output")] 
        public PLCInputBool ButtonOn; //!< PLCOutput that is connected to the Button;
        
        private Button _button;
        private UIButtonClick _buttonclick;

        private Image _imageoff;
        private Image _imageon;
        private bool _lastpressed;
        // Start is called before the first frame update

        void OnValidate()
        {
            var text = GetChildByNameAlsoHidden("Text");
                var imgname = color.ToString();

                if (text!=null)
                {
                    var te = GetChildByNameAlsoHidden("Text");
                    if (te!=null && ButtonText!=null)
                          te.GetComponent<Text>().text = ButtonText.Replace(";", "\n");
                 }

                var off = GetChildByNameAlsoHidden("Off");
                if (off!=null)
                    _imageoff = off.GetComponent<Image>();
                var on = GetChildByNameAlsoHidden("on");
                if (on!=null)
                  _imageon = on.GetComponent<Image>();
                var sprite = UnityEngine.Resources.Load<Sprite>("AutomationUI/ButtonOff" + imgname);
                if (_imageoff!=null)
                   _imageoff.sprite = sprite;

                sprite = UnityEngine.Resources.Load<Sprite>("AutomationUI/ButtonOn" + imgname);
                if (_imageon!=null)
                   _imageon.sprite = sprite;
            
        }

        private void SetStatus()
        {
            _imageon.gameObject.SetActive(IsOn);
            _imageoff.gameObject.SetActive(!IsOn);
            
            if (ButtonOn != null)
            {
                if (NormallyOpenend)
                    ButtonOn.Value = IsOn;
                else
                    ButtonOn.Value = !IsOn;
            }
        }


        public void OnMouseDown() // Event Method called when button image is pushed (pointer down)
        {
            if (!IsToggle) // If not toggle button
            {
                IsOn = true;
            }
            else
            {
                IsOn = !IsOn;
            }

            SetStatus();
        }

        public void OnMouseUp() // Event Method called when button image is not pushed (pointer up)
        {
            if (!IsToggle)
            {
                IsOn = false;
                SetStatus();
            }
        }


        void Start()
        {
            _imageoff = GetChildByNameAlsoHidden("Off").GetComponent<Image>();
            _imageon = GetChildByNameAlsoHidden("On").GetComponent<Image>();
            _lastpressed = !IsOn;
        }

        // Update is called once per frame
        void Update()
        {
            if (_lastpressed != IsOn)
            {
               SetStatus();
               _lastpressed = IsOn;
            }
        }
    }
}