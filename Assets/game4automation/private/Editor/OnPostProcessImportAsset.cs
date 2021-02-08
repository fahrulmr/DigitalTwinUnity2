// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEditor;
using UnityEngine;
using System.IO;

#if !UNITY_CLOUD_BUILD
namespace game4automation
{
    class MyAllPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool Game4AutomationImport = false;

            foreach (string str in importedAssets)
            {
                if (str.Contains("Assets/game4automation/private/Editor"))
                {

                    Game4AutomationImport = true;
                }

            }

#if !DEV
            if (Game4AutomationImport)
            {
                Debug.Log("Updating Game4Automation");
                // Disable Interact
                string MenuName = "game4automation/Enable Interact (Pro)";
                EditorPrefs.SetBool(MenuName, false);
                Game4AutomationToolbar.SetStandardSettings(false);

                var window = ScriptableObject.CreateInstance<HelloWindow>();
                window.Open();
                
                // Delete old QuickToggle Location if existant
                if (Directory.Exists("Assets/game4automation/private/Editor/QuickToggle"))
                {
                    Directory.Delete("Assets/game4automation/private/Editor/QuickToggle",true);
                }
            }
#endif

        }
    }
}
#endif