// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using game4automation;
using UnityEngine;
using UnityEngine.UI;

namespace game4automation
{
    [SelectionBase]

    //! UI Lamp component that can be connected to a PLCOutput.
    public class UILamp : Game4AutomationUI
    {
        [Header("Settings")] public string LampText; //!< The text below of the lamp
        public BUTTONCOLOR Color; //!< the color of the Lamp
        public bool NormallyOn = false; //!< true if the Lamp should be on if the PLCOut is false, standard = false
        [Header("Status")] public bool LampIsOn; //!< Status of the lamp, true if the lamp is on
    
        [Header("LAMP IO's")] public PLCOutputBool LampOn; //!< PLCOutput to turn the lamp on

        private GameObject _objon;
        private GameObject _objoff;
        private Image _spriteon;
        private Image _spriteoff;
  
        // Start is called before the first frame update

        void OnValidate()
        {
            var text = GetChildByNameAlsoHidden("Text");
            var imgname = Color.ToString();

            if (text != null)
            {
                   var te =   text.GetComponent<Text>();
                   if (te!=null && LampText!=null) 
                       te.text = LampText.Replace(";", "\n");
            }

            var off = GetChildByNameAlsoHidden("Off");
            Image image = null;
            if (off!=null) 
                image = off.GetComponent<Image>();
            var sprite = UnityEngine.Resources.Load<Sprite>("AutomationUI/LampOff"+imgname);
            if (image!=null)
                image.sprite = sprite;

            var on = GetChildByNameAlsoHidden("On");
            if (on!=null)
                 image = on.GetComponent<Image>();
            sprite =  UnityEngine.Resources.Load<Sprite>("AutomationUI/LampOn"+imgname);
            if (image!=null)
                image.sprite = sprite;

        }
        void Start()
        {
            _objon = GetChildByNameAlsoHidden("On");
            _spriteon = _objon.GetComponent<Image>();
            _objoff = GetChildByNameAlsoHidden("Off");
            _spriteoff = _objoff.GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            if (LampOn != null)
            {
                if (!NormallyOn)
                    LampIsOn = LampOn.Value;
                else
                    LampIsOn = !LampOn.Value;
            }

            _spriteon.enabled = LampIsOn;
            _spriteoff.enabled = !LampIsOn;
        }
    }
}