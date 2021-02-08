// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEngine;
using UnityEngine.UI;


namespace game4automation
{
    //! Displays a message box with a text field in the middle of the gameview
    public class UIMessageBox : MonoBehaviour
    {
        public Text TextBox; //!< A pointer to the Unity UI text box

        
        private void DestroyMessage()
        {
            Destroy(gameObject);
        }

        //! Displays the message on the middle of the gameview
        public void DisplayMessage(string message, bool autoclose, float closeafterseconds)
        {
            
            TextBox.text = message;
            if (autoclose)
            {
                Invoke("DestroyMessage", closeafterseconds);
            }
        }
    }
}
