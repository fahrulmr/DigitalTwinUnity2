// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEngine;
using UnityEngine.UI;

namespace game4automation
{
    public class UIPanelRow : MonoBehaviour
    {

        public bool ControlLayoutGroup = true;

        public int PaddingLeft;

        public int PaddingRight;

        public int PaddingTop;

        public int PadddingBottom;

        public int Spacing;

        public void OnValidate()
        {
            if (ControlLayoutGroup)
            {
                var group = GetComponent<HorizontalLayoutGroup>();

                group.padding.left = PaddingLeft;
                group.padding.right = PaddingRight;
                group.padding.top = PaddingTop;
                group.padding.bottom = PadddingBottom;

                group.spacing = Spacing;


            }
        }
    }
}
