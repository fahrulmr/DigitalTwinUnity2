

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace game4automation
{


    [InitializeOnLoad]
    //! The class is automatically saving the scene when run is started in the Unity editor. It can be turned off by the toggle in the game4automation menu
    public class AutoSaveOnRunMenuItem
    {


        public const string MenuName = "game4automation/Auto Save";
        private static bool isToggled;

        static AutoSaveOnRunMenuItem()
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
                EditorApplication.playModeStateChanged += AutoSaveOnRun;
            }
            else
            {
                EditorApplication.playModeStateChanged -= AutoSaveOnRun;
            }
        }

        private static void AutoSaveOnRun(PlayModeStateChange state)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                Debug.Log("Auto-Saving before entering Play mode");

                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
            }
        }
    }
}