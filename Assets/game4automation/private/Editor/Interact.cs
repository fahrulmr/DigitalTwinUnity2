

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace game4automation
{
    
    [InitializeOnLoad]
    //! The class is automatically saving the scene when run is started in the Unity editor. It can be turned off by the toggle in the game4automation menu
    public class InteractMenuItem
    {
        
        public const string MenuName = "game4automation/Enable Interact (Pro)";
        private static bool isToggled;

        static InteractMenuItem()
        {
            EditorApplication.delayCall += () =>
            {
                isToggled = EditorPrefs.GetBool(MenuName, false);
                UnityEditor.Menu.SetChecked(MenuName, isToggled);
                SetMode();
            };
        }

        [MenuItem(MenuName, false, 500)]
        private static void ToggleMode()
        {
            isToggled = !isToggled;
            UnityEditor.Menu.SetChecked(MenuName, isToggled);
            EditorPrefs.SetBool(MenuName, isToggled);
            SetMode();
        }

        private static void SetMode()
        {
           
            if (isToggled)
            {
                if (!AssetDatabase.IsValidFolder("Assets/INTERACT"))
               {
                   EditorUtility.DisplayDialog("Error",
                     "Interact is not available in standard folder Assets/Interact, Interact is only available for Game4Automation Professional and requires an additional Interact license", "OK");
               }
               else
               {
                   Global.SetDefine("GAME4AUTOMATION_INTERACT");
               }
            }
            else
            {
                Global.DeleteDefine("GAME4AUTOMATION_INTERACT");
            }
        }

       
    }
}