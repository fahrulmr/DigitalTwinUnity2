// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEngine;

namespace game4automation
{

    public enum LightGroupEnum {Sun,FirstLight,SecondLight}
    
    [RequireComponent(typeof(Light))]
    //! The LightGroup is used to be able to set centralized multiple lights. You can attach a LightGroup to any light in the scene.
    //! Game4AutomationController will use the LightGroup to set the light intensity.
    public class LightGroup : MonoBehaviour
    {
        public LightGroupEnum Group;  //!< The group of the light

        //! Sets the intensity if the light is assigned to the LightGroup group
        public void SetIntensity(LightGroupEnum group, float intensity)
        {
            if (group == Group)
            {
                var light = GetComponent<Light>();
                light.intensity = intensity;
            }
        }
        
    }
}

